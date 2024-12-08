using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing;
using Taxually.TechnicalTest.Processing.Options;
using Taxually.TechnicalTest.UnitTests.Common;
using Taxually.TechnicalTest.UnitTests.Extensions;

namespace Taxually.TechnicalTest.UnitTests.Processing;

partial class HttpVatRegistrationProcessorTests
{
    [TestFixture]
    private sealed class ProcessAsync : HttpVatRegistrationProcessorTests
    {
        private Mock<TaxuallyHttpClient> httpClientMock;
        private Mock<IOptionsMonitor<ProcessorOptions>> processorOptionsMock;
        private HttpVatRegistrationProcessor sut;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            httpClientMock = fixture.InjectMocked<TaxuallyHttpClient>(MockBehavior.Strict);
            processorOptionsMock = fixture.InjectMocked<IOptionsMonitor<ProcessorOptions>>(MockBehavior.Strict);

            sut = fixture.Create<HttpVatRegistrationProcessor>();
        }

        [Test]
        [AutoData]
        public async Task ParametersAreValid_SendsHttpRequest_AndReturnsSuccess(
            ProcessorOptions processorOptions,
            VatRegistration vatRegistration,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange
            processorOptionsMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(processorOptions);
            object payload = null!;
            httpClientMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback((string _, object x, CancellationToken _) => payload = x);

            // Act
            var result = await sut.ProcessAsync(vatRegistration, cancellationToken);

            // Assert
            result.Should().BeOfType<VatRegistrationResult.Success>()
                .Which.Async.Should().BeFalse();

            var expectedPayload = new { CompanyName = vatRegistration.Company.Name, CompanyId = vatRegistration.Company.Id, Country = vatRegistration.Country.Value };
            payload.Should().BeEquivalentTo(expectedPayload);

            processorOptionsMock.Verify(x => x.Get(vatRegistration.Country.Value), Times.Once);
            httpClientMock.Verify(x => x.PostAsync(processorOptions.Target, It.IsAny<It.IsAnyType>(), cancellationToken), Times.Once);
        }

        [Test]
        [AutoData]
        public async Task HttpClientThrows_DoesNotSwallow(
            Exception exception,
            ProcessorOptions processorOptions,
            VatRegistration vatRegistration,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange
            processorOptionsMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(processorOptions);
            httpClientMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            var act = () => sut.ProcessAsync(vatRegistration, cancellationToken);

            // Assert
            (await act.Should()
                .ThrowAsync<Exception>())
                .Which.Should().Be(exception);

            processorOptionsMock.Verify(x => x.Get(vatRegistration.Country.Value), Times.Once);
            httpClientMock.Verify(x => x.PostAsync(processorOptions.Target, It.IsAny<It.IsAnyType>(), cancellationToken), Times.Once);
        }

        [Test]
        [AutoData]
        public async Task HttpClientPostCanceled_Cancels(
            OperationCanceledException canceledException,
            ProcessorOptions processorOptions,
            VatRegistration vatRegistration,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange
            processorOptionsMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(processorOptions);
            httpClientMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(canceledException);

            // Act
            var act = () => sut.ProcessAsync(vatRegistration, cancellationToken);

            // Assert
            await act.Should()
                .ThrowAsync<OperationCanceledException>();

            processorOptionsMock.Verify(x => x.Get(vatRegistration.Country.Value), Times.Once);
            httpClientMock.Verify(x => x.PostAsync(processorOptions.Target, It.IsAny<It.IsAnyType>(), cancellationToken), Times.Once);
        }
    }
}
