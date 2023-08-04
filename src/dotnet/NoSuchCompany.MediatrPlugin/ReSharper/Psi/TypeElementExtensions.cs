using JetBrains.ReSharper.Psi;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi
{
    internal static class TypeElementExtensions
    {
        public static string GetFullname(this ITypeElement typeElement)
        {
            return $"{typeElement.GetContainingNamespace().QualifiedName}.{typeElement.ShortName}";
        }
    }
}