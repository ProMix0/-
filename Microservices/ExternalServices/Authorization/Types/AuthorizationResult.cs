using System;

namespace Microservices.ExternalServices.Authorization.Types
{
    /// <summary>
    /// Результат авторизации пользователя
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        /// Флаг, обозначающий успешную авторизацию
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public Guid UserId { get; set; }
    }
}