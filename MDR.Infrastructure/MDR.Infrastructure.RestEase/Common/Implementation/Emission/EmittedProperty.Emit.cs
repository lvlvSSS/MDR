using System.Reflection.Emit;
using MDR.Infrastructure.RestEase.Common.Implementation.Analysis;

namespace MDR.Infrastructure.RestEase.Common.Implementation.Emission;

internal partial class EmittedProperty
{
    public FieldBuilder FieldBuilder { get; }

    public EmittedProperty(PropertyModel propertyModel, FieldBuilder fieldBuilder)
    {
        this.PropertyModel = propertyModel;
        this.FieldBuilder = fieldBuilder;
    }
}