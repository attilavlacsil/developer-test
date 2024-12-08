using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;
using Taxually.TechnicalTest.Services;
using Taxually.TechnicalTest.UnitTests.Common;
using Taxually.TechnicalTest.UnitTests.Extensions;

namespace Taxually.TechnicalTest.UnitTests.Services;

partial class VatRegistrationServiceTests
{
    [TestFixture]
    private sealed class RegisterAsync : VatRegistrationServiceTests
    {
        private Mock<IServiceProvider> serviceProviderMock;
        private Mock<IOptionsMonitor<ProcessorOptions>> processorOptionsMock;
        private Mock<ILogger<VatRegistrationService>> loggerMock;
        private VatRegistrationService sut;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            serviceProviderMock = fixture.InjectMocked<IServiceProvider>(MockBehavior.Strict);
            processorOptionsMock = fixture.InjectMocked<IOptionsMonitor<ProcessorOptions>>(MockBehavior.Strict);
            loggerMock = fixture.InjectMocked<ILogger<VatRegistrationService>>(MockBehavior.Loose);
            loggerMock
                .Setup(x => x.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);

            sut = fixture.Create<VatRegistrationService>();
        }

        [Test]
        [AutoData]
        public async Task ProcessorExistsForCountry_CallsProcessor_AndReturnsResult(
            ProcessorOptions processorOptions,
            VatRegistration vatRegistration,
            VatRegistrationResult.Success vatRegistrationResult,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange
            processorOptionsMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(processorOptions);

            var scopeServiceProviderMock = new Mock<IKeyedServiceProvider>(MockBehavior.Strict);
            var serviceScopeMock = new Mock<IServiceScope>(MockBehavior.Strict);
            serviceScopeMock
                .SetupGet(x => x.ServiceProvider)
                .Returns(scopeServiceProviderMock.Object);
            serviceScopeMock
                .As<IAsyncDisposable>()
                .Setup(x => x.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(serviceScopeFactoryMock.Object);

            var vatRegistrationProcessorMock = new Mock<IVatRegistrationProcessor>(MockBehavior.Strict);
            vatRegistrationProcessorMock
                .Setup(x => x.ProcessAsync(It.IsAny<VatRegistration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vatRegistrationResult);
            scopeServiceProviderMock
                .Setup(x => x.GetKeyedService(It.IsAny<Type>(), It.IsAny<object?>()))
                .Returns(vatRegistrationProcessorMock.Object);

            // Act
            var result = await sut.RegisterAsync(vatRegistration, cancellationToken);

            // Assert
            result.Should().Be(vatRegistrationResult);

            processorOptionsMock.Verify(x => x.Get(vatRegistration.Country.Value), Times.Once);
            serviceProviderMock.Verify(x => x.GetService(typeof(IServiceScopeFactory)), Times.Once);
            scopeServiceProviderMock.Verify(x => x.GetKeyedService(typeof(IVatRegistrationProcessor), processorOptions.ProcessorName), Times.Once);
            serviceScopeMock.VerifyGet(x => x.ServiceProvider, Times.Once);
            serviceScopeMock.As<IAsyncDisposable>().Verify(x => x.DisposeAsync(), Times.Once);
            vatRegistrationProcessorMock.Verify(x => x.ProcessAsync(vatRegistration, cancellationToken), Times.Once);
            processorOptionsMock.VerifyNoOtherCalls();
            serviceProviderMock.VerifyNoOtherCalls();
            loggerMock.VerifyNoOtherCalls();
            scopeServiceProviderMock.VerifyNoOtherCalls();
            serviceScopeMock.VerifyNoOtherCalls();
            vatRegistrationProcessorMock.VerifyNoOtherCalls();
        }

        [Test]
        [AutoData]
        public async Task ProcessorDoesNotExistForCountry_LogsError_AndReturnsFailureResult(
            ProcessorOptions processorOptions,
            VatRegistration vatRegistration,
            [NonCanceled] CancellationToken cancellationToken)
        {
            // Arrange
            processorOptionsMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(processorOptions);

            var scopeServiceProviderMock = new Mock<IKeyedServiceProvider>(MockBehavior.Strict);
            var serviceScopeMock = new Mock<IServiceScope>(MockBehavior.Strict);
            serviceScopeMock
                .SetupGet(x => x.ServiceProvider)
                .Returns(scopeServiceProviderMock.Object);
            serviceScopeMock
                .As<IAsyncDisposable>()
                .Setup(x => x.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(serviceScopeFactoryMock.Object);

            scopeServiceProviderMock
                .Setup(x => x.GetKeyedService(It.IsAny<Type>(), It.IsAny<object?>()))
                .Returns((object?)null);

            // Act
            var result = await sut.RegisterAsync(vatRegistration, cancellationToken);

            // Assert
            result.Should().BeOfType<VatRegistrationResult.Failure>()
                .Which.Error.Should().Be($"Invalid country: {vatRegistration.Country.Value}");

            processorOptionsMock.Verify(x => x.Get(vatRegistration.Country.Value), Times.Once);
            serviceProviderMock.Verify(x => x.GetService(typeof(IServiceScopeFactory)), Times.Once);
            scopeServiceProviderMock.Verify(x => x.GetKeyedService(typeof(IVatRegistrationProcessor), processorOptions.ProcessorName), Times.Once);
            serviceScopeMock.VerifyGet(x => x.ServiceProvider, Times.Once);
            serviceScopeMock.As<IAsyncDisposable>().Verify(x => x.DisposeAsync(), Times.Once);
            loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, _) => x.ToString() == $"No processor registered for country {vatRegistration.Country.Value}"), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            processorOptionsMock.VerifyNoOtherCalls();
            serviceProviderMock.VerifyNoOtherCalls();
            scopeServiceProviderMock.VerifyNoOtherCalls();
            serviceScopeMock.VerifyNoOtherCalls();
        }
    }
}
