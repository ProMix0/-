using System;

namespace Microservices.ExternalServices.CatExchange.Types
{
    /// <summary>
    /// Информация о цене котика на бирже
    /// </summary>
    public class CatPriceInfo
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Цена в рублях
        /// </summary>
        public decimal Price { get; set; }
    }
}