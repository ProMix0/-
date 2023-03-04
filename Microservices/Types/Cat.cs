using System;
using System.Collections.Generic;

namespace Microservices.Types
{
    /// <summary>
    /// Котик
    /// </summary>
    public class Cat
    {
        /// <summary>
        /// ИД котика
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ИД породы котика
        /// </summary>
        public Guid BreedId { get; set; }

        /// <summary>
        /// ИД пользователя, добавившего котика
        /// </summary>
        public Guid AddedBy { get; set; }

        /// <summary>
        /// Название породы
        /// </summary>
        public string Breed { get; set; }

        /// <summary>
        /// Имя котика
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фотография конкретного котика. При отсутствии равна null
        /// </summary>
        public byte[] CatPhoto { get; set; }

        /// <summary>
        /// Фотография котика данной породы. Используется при отсутствии фотографии конкретного котика
        /// </summary>
        public byte[] BreedPhoto { get; set; }

        /// <summary>
        /// Текущая биржевая цена котика в рублях. При отсутствии породы на бирже равна 1000 рублям
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// История цены котика данной породы на бирже. При отсутствии породы на бирже список должен быть пустой
        /// </summary>
        public List<(DateTime Date, decimal Price)> Prices { get; set; }
    }
}