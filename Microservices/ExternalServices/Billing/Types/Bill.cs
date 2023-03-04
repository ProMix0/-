using System;

namespace Microservices.ExternalServices.Billing.Types
{
    /// <summary>
    /// Счёт на оплату продукта
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// ИД счёта
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ИД продукта
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Цена продажи
        /// </summary>
        public decimal Price { get; set; }
    }
}