namespace MDR.Infrastructure.RestEase.Common.Implementation.Emission;

internal class EmittedType
{
    public Type Type { get; }

    public EmittedType(Type type) => this.Type = type;
}