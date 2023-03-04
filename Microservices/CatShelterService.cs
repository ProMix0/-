using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Authorization;
using Microservices.ExternalServices.Authorization.Types;
using Microservices.ExternalServices.Billing;
using Microservices.ExternalServices.Billing.Types;
using Microservices.ExternalServices.CatDb;
using Microservices.ExternalServices.CatDb.Types;
using Microservices.ExternalServices.CatExchange;
using Microservices.ExternalServices.CatExchange.Types;
using Microservices.ExternalServices.Database;
using Microservices.Types;

namespace Microservices
{
    public class CatShelterService : ICatShelterService
    {
        //Consts for documents in database
        private const string Cats = "Cats";
        private const string Favourites = "Favourites:{0}";

        //Count of attemts to connect after getting ConnectionException
        //private const int disconnectRetryCount = 1;

        //Injected services
        private readonly IDatabase database;
        private readonly IAuthorizationService authorizationService;
        private readonly IBillingService billingService;
        private readonly ICatInfoService catInfoService;
        private readonly ICatExchangeService catExchangeService;

        public CatShelterService(
            IDatabase database,
            IAuthorizationService authorizationService,
            IBillingService billingService,
            ICatInfoService catInfoService,
            ICatExchangeService catExchangeService)
        {
            this.database = database;
            this.authorizationService = authorizationService;
            this.billingService = billingService;
            this.catInfoService = catInfoService;
            this.catExchangeService = catExchangeService;
        }

        public async Task<List<Cat>> GetCatsAsync(string sessionId, int skip, int limit, CancellationToken cancellationToken)
        {
            await MyAuthorizeAsync(sessionId, cancellationToken);

            IDatabaseCollection<CatWithID, Guid> catsDatabase = database.GetCollection<CatWithID, Guid>(Cats);

            List<Cat> catslist = (await MyGetProductsAsync(skip, limit, cancellationToken))
                                                    .Select(prod => catsDatabase
                                                        .FindAsync(prod.Id, cancellationToken).Result.Cat //Get cat
                                                        .EnsurePrice(catExchangeService, cancellationToken)) //Fill price properties
                                                    .ToList();

            return catslist;
        }


        public async Task AddCatToFavouritesAsync(string sessionId, Guid catId, CancellationToken cancellationToken)
        {
            AuthorizationResult authorization = await MyAuthorizeAsync(sessionId, cancellationToken);

            var favourites = database.GetCollection<SimpleId, Guid>(string.Format(Favourites, authorization.UserId));

            await favourites.WriteAsync(new(catId), cancellationToken);
        }


        public async Task<List<Cat>> GetFavouriteCatsAsync(string sessionId, CancellationToken cancellationToken)
        {
            AuthorizationResult authorization = await MyAuthorizeAsync(sessionId, cancellationToken);

            var favourites = database.GetCollection<SimpleId, Guid>(string.Format(Favourites, authorization.UserId));
            var cats = database.GetCollection<CatWithID, Guid>(Cats);


            return (await favourites
                                                        .FindAsync(id => MyGetProductAsync(id.Id, cancellationToken).Result != null, //Not solted cats
                                                            cancellationToken))
                                                        .Select(id => cats
                                                            .FindAsync(id.Id, cancellationToken).Result.Cat //Get cat
                                                            .EnsurePrice(catExchangeService, cancellationToken)) //Fill price properties
                                                        .ToList();
        }


        public async Task DeleteCatFromFavouritesAsync(string sessionId, Guid catId, CancellationToken cancellationToken)
        {
            AuthorizationResult authorization = await MyAuthorizeAsync(sessionId, cancellationToken);

            var favourites = database.GetCollection<SimpleId, Guid>(string.Format(Favourites, authorization.UserId));

            await favourites.DeleteAsync(catId, cancellationToken);
        }


        public async Task<Bill> BuyCatAsync(string sessionId, Guid catId, CancellationToken cancellationToken)
        {
            await MyAuthorizeAsync(sessionId, cancellationToken);

            Product product = await MyGetProductAsync(catId, cancellationToken);

            if (product == null) throw new InvalidRequestException();

            CatPriceHistory priceHistory = await MyGetPriceInfoAsync(product.BreedId, cancellationToken);

            return await MySellProductAsync(product.Id, priceHistory.GetPrice(), cancellationToken);
        }

        public async Task<Guid> AddCatAsync(string sessionId, AddCatRequest request, CancellationToken cancellationToken)
        {
            AuthorizationResult result = await MyAuthorizeAsync(sessionId, cancellationToken);


            CatInfo catInfo = await MyFindByBreedNameAsync(request.Breed, cancellationToken);
            if (request.Breed == null || request.Name == null) throw new InvalidRequestException();

            Cat cat = new()
            {
                AddedBy = result.UserId,
                Breed = request.Breed,
                CatPhoto = request.Photo,
                Name = request.Name,
                BreedId = catInfo.BreedId,
                BreedPhoto = catInfo.Photo,
                Id = Guid.NewGuid()
            };

            await MyAddProductAsync(new Product()
            {
                BreedId = cat.BreedId,
                Id = cat.Id
            }, cancellationToken);

            await database
                .GetCollection<CatWithID, Guid>(Cats)
                .WriteAsync(new(cat), cancellationToken);

            return cat.Id;
        }

        /// <inheritdoc cref="AuthorizationService.AuthorizeAsync(string, CancellationToken)"/>
        private async Task<AuthorizationResult> MyAuthorizeAsync(string sessionId, CancellationToken cancellationToken)
        {
            AuthorizationResult result = await RetryConnection(async () => await authorizationService.AuthorizeAsync(sessionId, cancellationToken));

            if (!result.IsSuccess) throw new AuthorizationException();

            return result;
        }

        /// <inheritdoc cref="ICatInfoService.FindByBreedNameAsync(string, CancellationToken)"/>
        private async Task<CatInfo> MyFindByBreedNameAsync(string breed, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await catInfoService.FindByBreedNameAsync(breed, cancellationToken));

        /// <inheritdoc cref="IBillingService.AddProductAsync(Product, CancellationToken)"/>
        private async Task MyAddProductAsync(Product product, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await billingService.AddProductAsync(product, cancellationToken));

        /// <inheritdoc cref="IBillingService.GetProductAsync(Guid, CancellationToken)"/>
        private async Task<Product> MyGetProductAsync(Guid id, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await billingService.GetProductAsync(id, cancellationToken));

        /// <inheritdoc cref="ICatExchangeService.GetPriceInfoAsync(Guid, CancellationToken)"/>
        private async Task<CatPriceHistory> MyGetPriceInfoAsync(Guid breedId, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await catExchangeService.GetPriceInfoAsync(breedId, cancellationToken));

        /// <inheritdoc cref="IBillingService.SellProductAsync(Guid, decimal, CancellationToken)"/>
        private async Task<Bill> MySellProductAsync(Guid id, decimal price, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await billingService.SellProductAsync(id, price, cancellationToken));

        /// <inheritdoc cref="IBillingService.GetProductsAsync(int, int, CancellationToken)"/>
        private async Task<List<Product>> MyGetProductsAsync(int skip, int limit, CancellationToken cancellationToken) =>
            await RetryConnection(async () => await billingService.GetProductsAsync(skip, limit, cancellationToken));

        /// <summary>
        /// Call <paramref name="request"/>. Call again on <see cref="ConnectionException"/>
        /// </summary>
        /// <typeparam name="T">Type of Task result</typeparam>
        /// <param name="request">Delegate to call</param>
        /// <returns>Task, represents result</returns>
        private async Task<T> RetryConnection<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (ConnectionException)
            {
                try
                {
                    return await request();
                }
                catch (ConnectionException)
                {
                    throw new InternalErrorException();
                }
            }
        }

        /// <summary>
        /// Call <paramref name="request"/>. Call again on <see cref="ConnectionException"/>
        /// </summary>
        /// <param name="request">Delegate to call</param>
        /// <returns>Task, represents operation</returns>
        private async Task RetryConnection(Func<Task> request)
        {
            try
            {
                await request();
            }
            catch (ConnectionException)
            {
                try
                {
                    await request();
                }
                catch (ConnectionException)
                {
                    throw new InternalErrorException();
                }
            }
        }
    }

    /// <summary>
    /// Class to save <see cref="Types.Cat"/> in database
    /// </summary>
    public class CatWithID : IEntityWithId<Guid>
    {
        public CatWithID(Cat cat)
        {
            Cat = cat;
        }

        public Cat Cat { get; }

        public Guid Id { get => Cat.Id; set => Cat.Id = value; }
    }

    /// <summary>
    /// Simple class to save Id in database
    /// </summary>
    public class SimpleId : IEntityWithId<Guid>
    {
        public SimpleId(Guid catId)
        {
            Id = catId;
        }

        public Guid Id { get; set; }
    }

    static class Utils
    {
        /// <summary>
        /// Ensure price properties in <see cref="Cat"/>
        /// </summary>
        /// <param name="cat">Cat to ensure</param>
        /// <param name="catExchangeService">Service to get price</param>
        /// <param name="cancellationToken">Token for cancel</param>
        /// <returns><see cref="Cat"/> with ensured price</returns>
        public static Cat EnsurePrice(this Cat cat, ICatExchangeService catExchangeService, CancellationToken cancellationToken)
        {
            CatPriceHistory priceHistory = catExchangeService.GetPriceInfoAsync(cat.BreedId, cancellationToken).Result;

            cat.Prices = priceHistory.Prices.Select(item => (item.Date, item.Price)).ToList();
            cat.Price = priceHistory.Prices.Count == 0 ? 1000 : priceHistory.Prices.Last().Price;

            return cat;
        }

        /// <summary>
        /// Calculate price
        /// </summary>
        /// <param name="priceHistory">History of prices</param>
        /// <returns>Last price or 1000 if no prices</returns>
        public static decimal GetPrice(this CatPriceHistory priceHistory)
        {
            return priceHistory.Prices.Count == 0 ? 1000 : priceHistory.Prices.Last().Price;
        }
    }
}