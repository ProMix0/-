using System;

namespace Microservices.ExternalServices.CatDb.Types
{
    /// <summary>
    /// Информация о котиках данной породы
    /// </summary>
    public class CatInfo
    {
        /// <summary>
        /// ИД породы котиков
        /// </summary>
        public Guid BreedId { get; set; }

        /// <summary>
        /// Название породы котиков
        /// </summary>
        public string BreedName { get; set; }

        /// <summary>
        /// Фотография породы котиков
        /// </summary>
        public byte[] Photo { get; set; }
    }
}