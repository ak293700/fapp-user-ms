using Domain.Entities.UserEntities;
using FappCommon.Mongo4Test.Interfaces;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext : IBaseMongoDbContext
{
    public IMongoCollection<User> Users { get; }
}