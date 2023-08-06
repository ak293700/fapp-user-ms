using Domain.Entities.UserEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq;

namespace Application.Repositories;

public class FriendshipRepository
{
    private readonly IApplicationDbContext _context;

    public FriendshipRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JoiningState?> GetFriendship(string user1Id, string user2Id,
        CancellationToken cancellationToken = default)
    {
        ProjectionDefinition<User>? projection = Builders<User>.Projection
            .ElemMatch(
                user => user.Friends,
                friends => friends.UserId == user2Id
            );
        BsonDocument? friendShipDocument = await _context.Users
            .Find(
                Builders<User>.Filter.Eq(u => u.Id, user1Id) &
                Builders<User>.Filter.ElemMatch(
                    user => user.Friends,
                    friends => friends.UserId == user2Id
                )
            )
            .Project(projection)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (friendShipDocument == null)
            return null;

        // Now convert the BsonDocument to User
        return BsonSerializer.Deserialize<User>(friendShipDocument)
            .Friends
            .First()
            .JoiningState;
    }

    public Task SetFriendship(string userId, string friendId, JoiningState joiningState,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.UpdateOneAsync(
            u => u.Id == userId,
            Builders<User>.Update.Push(u => u.Friends, new Friend
            {
                UserId = friendId,
                JoiningState = joiningState
            }),
            cancellationToken: cancellationToken
        );
    }

    public Task UpdateFriendship(string userId, string friendId, JoiningState joiningState,
        CancellationToken cancellationToken = default)
    {
        FilterDefinition<User>? filter = Builders<User>.Filter.Eq(u => u.Id, userId) // Select the user
                                         & Builders<User>.Filter // The item of the Friends Array
                                             .ElemMatch(x => x.Friends,
                                                 Builders<Friend>.Filter.Eq(x => x.UserId, friendId)
                                             );

        return _context.Users.UpdateOneAsync(
            filter,
            Builders<User>.Update.Set(u => u.Friends.FirstMatchingElement(), new Friend
            {
                UserId = friendId,
                JoiningState = joiningState
            }),
            cancellationToken: cancellationToken
        );
    }
}