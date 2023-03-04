using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Billing.Types;
using Microservices.Types;

namespace Microservices
{
    /// <summary>
    /// Приют котиков
    /// </summary>
    public interface ICatShelterService
    {
        /// <summary>
        /// Получить котиков из приюта
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="skip">Количество котиков, которое необходимо пропустить</param>
        /// <param name="limit">Максимальное количество котиков, которое необходимо вернуть</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список котиков из приюта. При отсутствии список пустой</returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<List<Cat>> GetCatsAsync(string sessionId, int skip, int limit, CancellationToken cancellationToken = default);


        /// <summary>
        /// Добавить котика в избранные
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="catId">ИД котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task AddCatToFavouritesAsync(string sessionId, Guid catId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить избранных котиков
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список избранных котиков. При отсутствии список пустой</returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<List<Cat>> GetFavouriteCatsAsync(string sessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить котика из списка избранных
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="catId">ИД котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task DeleteCatFromFavouritesAsync(string sessionId, Guid catId, CancellationToken cancellationToken = default);


        /// <summary>
        /// Купить котика
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="catId">ИД котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Счёт на покупку котика. Если другой пользователь получил счёт, но не оплатил его, возвращает новый счёт</returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<Bill> BuyCatAsync(string sessionId, Guid catId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавить котика в приют
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="request">Запрос на добавление котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns>ИД добавленного котика</returns>
        /// <exception cref="AuthorizationException">Пользователь не авторизован</exception>
        /// <exception cref="InvalidRequestException">Ошибка в запросе</exception>
        /// <exception cref="InternalErrorException">Внутренняя ошибка сервиса</exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<Guid> AddCatAsync(string sessionId, AddCatRequest request, CancellationToken cancellationToken = default);
    }
}