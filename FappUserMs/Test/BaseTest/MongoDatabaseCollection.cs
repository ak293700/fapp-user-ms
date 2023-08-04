namespace Test.BaseTest;

[CollectionDefinition(Name)]
public class MongoDatabaseCollection : ICollectionFixture<MongoDatabaseFixture>
{
    public const string Name = nameof(MongoDatabaseCollection);
}