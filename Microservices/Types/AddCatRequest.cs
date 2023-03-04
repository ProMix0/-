namespace Microservices.Types
{
    /// <summary>
    /// Запрос на добавление котика в приют
    /// </summary>
    public class AddCatRequest
    {
        /// <summary>
        /// Имя котика
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Название породы
        /// </summary>
        public string Breed { get; set; }

        /// <summary>
        /// Фотография конкретного котика. При отсутствии равна null
        /// </summary>
        public byte[] Photo { get; set; }
    }
}