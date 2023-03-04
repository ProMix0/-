namespace Microservices.ExternalServices.Database
{
    /// <summary>
    /// База данных
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Получить коллекцию документов. Если коллекции не существует, она создаётся
        /// </summary>
        /// <param name="collectionName">Название коллекции</param>
        /// <typeparam name="TDocument">Тип документов</typeparam>
        /// <typeparam name="TId">Тип ИД документа</typeparam>
        /// <returns>Коллекция документов</returns>
        IDatabaseCollection<TDocument, TId> GetCollection<TDocument, TId>(string collectionName) where TDocument : class, IEntityWithId<TId>;
    }
}