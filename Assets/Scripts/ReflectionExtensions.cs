using System.Reflection;

namespace Reflections
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool includeChildClasses = false)
        {
            var atts = provider.GetCustomAttributes(typeof(T), includeChildClasses);
            return atts.Length > 0;
        }
    }
}
