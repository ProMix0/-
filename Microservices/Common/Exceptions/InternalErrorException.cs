using System;

namespace Microservices.Common.Exceptions
{
    /// <summary>
    /// Внутренняя ошибка сервиса
    /// </summary>
    public class InternalErrorException : Exception
    {
        public InternalErrorException() : this(null)
        {
        }

        public InternalErrorException(Exception innerException) : base(null, innerException)
        {
        }
    }
}