namespace Taxually.TechnicalTest.UnitTests.Extensions;

public static class FixtureExtensions
{
    /// <summary>
    /// Injects a mocked instance for a specific type, in order to make that instance a shared instance.
    /// </summary>
    public static Mock<T> InjectMocked<T>(this Fixture fixture, MockBehavior mockBehavior = MockBehavior.Default)
        where T : class
    {
        var mock = new Mock<T>(mockBehavior);
        fixture.Inject(mock.Object);
        return mock;
    }
}
