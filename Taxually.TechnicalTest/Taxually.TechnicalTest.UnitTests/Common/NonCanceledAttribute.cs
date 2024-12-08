using System.Reflection;

namespace Taxually.TechnicalTest.UnitTests.Common;

public sealed class NonCanceledAttribute : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
        => new Customization();

    private sealed class Customization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new CancellationToken(canceled: false));
        }
    }
}
