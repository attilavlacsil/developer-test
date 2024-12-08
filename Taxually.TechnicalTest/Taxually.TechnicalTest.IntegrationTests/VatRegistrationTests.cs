using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Taxually.TechnicalTest.Dtos;
using Taxually.TechnicalTest.Processing.Options;

namespace Taxually.TechnicalTest.IntegrationTests;

internal sealed class VatRegistrationTests
{
    private WebApplicationFactory<Program> factory;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        factory = new WebApplicationFactory<Program>();
    }

    [Test]
    [AutoData]
    public async Task CountryProcessedByHttp_ReturnsOk(
        Uri target,
        VatRegistrationRequest vatRegistrationRequest)
    {
        // Arrange
        var country = vatRegistrationRequest.Country;
        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.ProcessorName)}", Processors.HttpVatRegistration);
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.Target)}", target.ToString());
            })
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/VatRegistration", vatRegistrationRequest, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    [AutoData]
    public async Task CountryProcessedByCsvQueue_ReturnsAccepted(
        string queueName,
        VatRegistrationRequest vatRegistrationRequest)
    {
        // Arrange
        var country = vatRegistrationRequest.Country;
        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.ProcessorName)}", Processors.CsvQueueVatRegistration);
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.Target)}", queueName);
            })
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/VatRegistration", vatRegistrationRequest, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    [AutoData]
    public async Task CountryProcessedByXmlQueue_ReturnsAccepted(
        string queueName,
        VatRegistrationRequest vatRegistrationRequest)
    {
        // Arrange
        var country = vatRegistrationRequest.Country;
        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.ProcessorName)}", Processors.XmlQueueVatRegistration);
                builder.UseSetting($"VatRegistrationProcessing:{country}:{nameof(ProcessorOptions.Target)}", queueName);
            })
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/VatRegistration", vatRegistrationRequest, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    [AutoData]
    public async Task NoProcessorForCountry_ReturnsBadRequest(
        VatRegistrationRequest vatRegistrationRequest)
    {
        // Arrange
        var country = vatRegistrationRequest.Country;
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/VatRegistration", vatRegistrationRequest, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType.Should().Be(MediaTypeHeaderValue.Parse("application/problem+json; charset=utf-8"));
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(CancellationToken.None);
        Debug.Assert(problem is not null, $"application/problem+json should be parsed as {nameof(ProblemDetails)}");
        problem.Detail.Should().Be($"Invalid country: {country}");
    }
}
