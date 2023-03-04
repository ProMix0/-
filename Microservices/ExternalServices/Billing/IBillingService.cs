using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Billing.Types;

namespace Microservices.ExternalServices.Billing
{
    /// <summary>
    /// Биллинг. Сервис учёта продуктов
    /// </summary>
    public interface IBillingService
    {
        /// <summary>
        /// Добавить продукт
        /// </summary>
        /// <param name="product">Продукт</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        Task AddProductAsync(Product product, CancellationToken cancellationToken);

        /// <summary>
        /// Получить продукты
        /// </summary>
        /// <param name="skip">Количество продуктов, которое необходимо пропустить</param>
        /// <param name="limit">Максимальное количество продуктов, которое необходимо вернуть</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список продуктов. При отсутствии список пустой</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        Task<List<Product>> GetProductsAsync(int skip, int limit, CancellationToken cancellationToken);

        /// <summary>
        /// Получить продукт
        /// </summary>
        /// <param name="id">ИД продукта</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Продукт. Если продукт отсутствует или продан, возвращает null</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Продать продукт
        /// </summary>
        /// <param name="id">ИД продукта</param>
        /// <param name="price">Цена в рублях, за которую следует продать продукт</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Счёт на оплату</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        /// <exception cref="InvalidRequestException">Продукт отсутствует или продан</exception>
        Task<Bill> SellProductAsync(Guid id, decimal price, CancellationToken cancellationToken);
    }
}