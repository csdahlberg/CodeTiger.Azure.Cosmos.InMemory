#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}

#endif
