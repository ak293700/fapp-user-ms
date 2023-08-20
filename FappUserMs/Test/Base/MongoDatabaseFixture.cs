using FappCommon.Mongo4Test.Implementations._4Tests;

namespace Test.Base;

/// <summary>
/// Singleton share between all tests
/// </summary>
public class MongoDatabaseFixture : BaseMongoDatabaseFixture<MockMongoDbContext>
{
}