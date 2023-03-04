using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.CatExchange.Types;

namespace Microservices.ExternalServices.CatExchange
{
    /// <summary>
    /// Биржа котиков
    /// </summary>
    public interface ICatExchangeService
    {
        /// <summary>
        /// Получить историю цены породы котиков
        /// </summary>
        /// <param name="breedId">ИД породы</param>
        /// <param name="cancellationToken"></param>
        /// <returns>История цены породы котиков. При отсутствии породы на бирже список цен пустой</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        Task<CatPriceHistory> GetPriceInfoAsync(Guid breedId, CancellationToken cancellationToken);

        /// <summary>
        /// Получить историю цены пород котиков
        /// </summary>
        /// <param name="breedIds">ИД пород</param>
        /// <param name="cancellationToken"></param>
        /// <returns>История цены пород котиков</returns>
        /// <exception cref="ConnectionException">Ошибка соединения. При отсутствии породы на бирже список цен пустой</exception>
        Task<Dictionary<Guid, CatPriceHistory>> GetPriceInfoAsync(Guid[] breedIds, CancellationToken cancellationToken);
    }
}