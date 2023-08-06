namespace Domain.Entities.UserEntities;

public enum JoiningState
{
    AskedFromMe = 0, // You asked to be friend but the other didn't accept yet
    Accepted, // Both are friends
    AskedFromHim, // The other asked to be friend but you didn't accept yet
}