#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct)]
internal sealed class RequiredMemberAttribute : Attribute
{
}

#endif
