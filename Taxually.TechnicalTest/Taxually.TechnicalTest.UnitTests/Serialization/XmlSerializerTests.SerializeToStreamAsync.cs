using Taxually.TechnicalTest.Serialization;
using Taxually.TechnicalTest.UnitTests.Common;

namespace Taxually.TechnicalTest.UnitTests.Serialization;

partial class XmlSerializerTests
{
    [TestFixture]
    private sealed class SerializeToStreamAsync
    {
        private XmlSerializer sut;

        [SetUp]
        public void SetUp()
            => sut = new XmlSerializer();

        [Test]
        [AutoData]
        public async Task InputItemIsValid_ReturnsSerializedToStream(
            TestClass testInstance,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange

            // Act
            using var result = await sut.SerializeToStreamAsync(testInstance, cancellationToken);

            // Assert
            using var reader = new StreamReader(result);
            var streamContent = await reader.ReadToEndAsync(CancellationToken.None);
            var expected = $"""
                            <?xml version="1.0" encoding="utf-8"?>
                            <{nameof(TestClass)} xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                              <{nameof(TestClass.Id)}>{testInstance.Id}</{nameof(TestClass.Id)}>
                              <{nameof(TestClass.Text)}>{testInstance.Text}</{nameof(TestClass.Text)}>
                            </{nameof(TestClass)}>
                            """;
            streamContent.Should().Be(expected);
        }
    }
}
