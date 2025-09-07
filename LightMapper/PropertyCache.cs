using System.Collections.Concurrent;
using System.Reflection;

namespace LightMapper;

internal static class PropertyCache
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
    
    public static PropertyInfo[] GetProperties(Type type)
    {
        return _propertyCache.GetOrAdd(type, t => 
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
    }
}