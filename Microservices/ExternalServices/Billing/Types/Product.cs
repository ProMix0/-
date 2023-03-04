using System;

namespace Microservices.ExternalServices.Billing.Types
{
    /// <summary>
    /// Продукт
    /// </summary>
    public class Product
    {
        /// <summary>
        /// ИД продукта
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ИД породы котика
        /// </summary>
        public Guid BreedId { get; set; }
    }
}