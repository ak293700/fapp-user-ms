using FappCommon.Mongo4Test;

namespace Test.Base;

/// <summary>
/// Singleton share between all tests
/// </summary>
public class MongoDatabaseFixture : BaseMongoDatabaseFixture<MockMongoDbContext>
{
}