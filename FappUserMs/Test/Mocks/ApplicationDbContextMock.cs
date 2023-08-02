using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure;
using MongoDB.Driver;

namespace Test.Mocks;

public class ApplicationDbContextMock : IApplicationDbContext
{
    public IMongoCollection<User> Users { get; }


    public ApplicationDbContextMock(IMongoDatabase database)
    {
        Users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
    }
}