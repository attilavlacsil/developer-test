using AutoMapper;

namespace Taxually.TechnicalTest.Mapping;

public sealed class VatRegistrationProfile : Profile
{
    public VatRegistrationProfile()
    {
        CreateMap<Dtos.VatRegistrationRequest, Models.VatRegistration>()
            .ConstructUsing((x, context) => new Models.VatRegistration(context.Mapper.Map<Models.Company>(x), context.Mapper.Map<Models.Country>(x)));

        CreateMap<Dtos.VatRegistrationRequest, Models.Company>()
            .ConstructUsing(x => new Models.Company(x.CompanyId, x.CompanyName));

        CreateMap<Dtos.VatRegistrationRequest, Models.Country>()
            .ConstructUsing(x => new Models.Country(x.Country));
    }
}
