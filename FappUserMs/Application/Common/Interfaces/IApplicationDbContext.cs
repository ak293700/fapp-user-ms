using Domain.Entities;
using MongoDB.Driver;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public IMongoCollection<User> Users { get; }
}