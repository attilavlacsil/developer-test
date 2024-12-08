using Taxually.TechnicalTest.Serialization;
using Taxually.TechnicalTest.UnitTests.Common;

namespace Taxually.TechnicalTest.UnitTests.Serialization;

partial class CsvSerializerTests
{
    [TestFixture]
    private sealed class SerializeToStreamAsync
    {
        private CsvSerializer sut;

        [SetUp]
        public void SetUp()
            => sut = new CsvSerializer();

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
            var expected = $"{nameof(TestClass.Id)},{nameof(TestClass.Text)}\r\n{testInstance.Id},{testInstance.Text}\r\n";
            streamContent.Should().Be(expected);
        }
    }
}
