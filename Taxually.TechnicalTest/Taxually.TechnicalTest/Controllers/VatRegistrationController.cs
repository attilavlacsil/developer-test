using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Services;

namespace Taxually.TechnicalTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class VatRegistrationController : ControllerBase
{
    private readonly VatRegistrationService vatRegistrationService;
    private readonly IMapper mapper;

    public VatRegistrationController(VatRegistrationService vatRegistrationService, IMapper mapper)
    {
        this.vatRegistrationService = vatRegistrationService;
        this.mapper = mapper;
    }

    /// <summary>
    /// Registers a company for a VAT number in a given country
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] Dtos.VatRegistrationRequest request, CancellationToken cancellationToken)
    {
        // TODO add validation (e.g. FluentValidation)

        var vatRegistration = mapper.Map<VatRegistration>(request);

        var result = await vatRegistrationService.RegisterAsync(vatRegistration, cancellationToken);

        // TODO add more details to the response
        return result switch
        {
            VatRegistrationResult.Success { Async: true } => Accepted(),
            VatRegistrationResult.Success { Async: false } => Ok(),
            VatRegistrationResult.Failure failure => Problem(detail: failure.Error, statusCode: StatusCodes.Status400BadRequest),
            _ => throw new UnreachableException()
        };
    }
}
