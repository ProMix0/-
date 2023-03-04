using System;
using System.Collections.Generic;

namespace Microservices.ExternalServices.CatExchange.Types
{
    /// <summary>
    /// История цены породы котиков
    /// </summary>
    public class CatPriceHistory
    {
        /// <summary>
        /// ИД породы котиков
        /// </summary>
        public Guid BreedId { get; set; }
        
        /// <summary>
        /// Список цен
        /// </summary>
        public List<CatPriceInfo> Prices { get; set; }
    }
}