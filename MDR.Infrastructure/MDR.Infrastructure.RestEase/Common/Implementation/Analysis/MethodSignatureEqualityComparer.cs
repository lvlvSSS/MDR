namespace MDR.Infrastructure.RestEase.Common.Implementation.Analysis;

internal partial class MethodSignatureEqualityComparer : IEqualityComparer<MethodModel>
{
    public static MethodSignatureEqualityComparer Instance { get; } = new MethodSignatureEqualityComparer();
}