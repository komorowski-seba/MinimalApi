using Xunit;

namespace MinimalApi.Test.Integration;

[CollectionDefinition("ApiTest")]
public class AppTestSharedCollection : ICollectionFixture<AppTestFactory>
{ }