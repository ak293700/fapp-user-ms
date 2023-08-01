// See https://aka.ms/new-console-template for more information

using Domain.Entities;
using Infrastructure;

Console.WriteLine("Hello, World!");

ApplicationDbContext context = 
    new("mongodb://root:password@localhost:27017/");

await context.Users.InsertOneAsync(new User
{
    FirstName = "John",
    LastName = "Doe"
});