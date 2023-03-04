using System;

namespace Microservices.ExternalServices.Database
{
    public interface IEntityWithId<TId>
    {
        TId Id { get; set; }
    }
}