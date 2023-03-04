using System;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Common.Exceptions;
using Microservices.ExternalServices.CatDb.Types;

namespace Microservices.ExternalServices.CatDb
{
    /// <summary>
    /// Энциклопедия котиков
    /// </summary>
    public interface ICatInfoService
    {
        /// <summary>
        /// Найти информацию по ИД породы котика
        /// </summary>
        /// <param name="id">ИД породы котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Информация о котиках данной породы</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        /// <exception cref="InvalidRequestException">Порода котика с данным ИД не существует</exception>
        Task<CatInfo> FindByBreedIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Найти информацию по ИД пород котиков
        /// </summary>
        /// <param name="ids">ИД пород котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Информация о котиках данных пород</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        /// <exception cref="InvalidRequestException">Порода котика с любым из данных ИД не существует</exception>
        Task<CatInfo[]> FindByBreedIdAsync(Guid[] ids, CancellationToken cancellationToken);

        /// <summary>
        /// Найти информацию по названию породы котика
        /// </summary>
        /// <param name="breed">Название породы котика</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Информация о котиках данной породы</returns>
        /// <exception cref="ConnectionException">Ошибка соединения</exception>
        /// <exception cref="InvalidRequestException">Порода котика с данным именем не существует</exception>
        Task<CatInfo> FindByBreedNameAsync(string breed, CancellationToken cancellationToken);
    }
}