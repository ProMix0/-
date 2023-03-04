using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.ExternalServices.Database
{
    /// <summary>
    /// Коллекция документов
    /// </summary>
    /// <typeparam name="TDocument">Тип документов</typeparam>
    /// <typeparam name="TId">Тип ИД документа</typeparam>
    public interface IDatabaseCollection<TDocument, TId> where TDocument : class, IEntityWithId<TId>
    {
        /// <summary>
        /// Найти документ
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Документ. Если документ не найден, возвращает null</returns>
        Task<TDocument> FindAsync(TId id, CancellationToken cancellationToken);

        /// <summary>
        /// Найти документы, удовлетворяющие условию
        /// </summary>
        /// <param name="filter">Условие поиска документов</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список документов. При отсутствии список пустой</returns>
        Task<List<TDocument>> FindAsync(Func<TDocument, bool> filter, CancellationToken cancellationToken);

        /// <summary>
        /// Сохранить документ. Если документ с данным ИД существует, он перезаписывается
        /// </summary>
        /// <param name="document">Документ</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteAsync(TDocument document, CancellationToken cancellationToken);

        /// <summary>
        /// Удалить документ. Если документ с данным ИД не существует, ничего не происходит
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(TId id, CancellationToken cancellationToken);
    }
}