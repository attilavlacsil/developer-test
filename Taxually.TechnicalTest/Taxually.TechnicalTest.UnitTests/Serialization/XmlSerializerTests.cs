namespace Taxually.TechnicalTest.UnitTests.Serialization;

/// <remarks>Must be public because of XML serializer.</remarks>
public abstract partial class XmlSerializerTests
{
    public sealed record TestClass
    {
        public required int Id { get; init; }
        public required string Text { get; init; }
    }
}
