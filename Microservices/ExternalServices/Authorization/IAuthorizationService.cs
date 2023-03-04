using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Authorization.Types;

namespace Microservices.ExternalServices.Authorization
{
    /// <summary>
    /// Сервис авторизации
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Авторизовать пользователя
        /// </summary>
        /// <param name="sessionId">ИД сессии пользователя</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Результат авторизации</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        Task<AuthorizationResult> AuthorizeAsync(string sessionId, CancellationToken cancellationToken);
    }
}