using System.Collections.Concurrent;
using System.Reflection;

namespace LightMapper;

internal static class MappingCache
{
    private static readonly ConcurrentDictionary<(Type Source, Type Dest), PropertyMapping[]> _mappingCache = new();

    public static PropertyMapping[] GetMappings(Type sourceType, Type destType)
    {
        return _mappingCache.GetOrAdd((sourceType, destType), key =>
        {
            var sourceProps = PropertyCache.GetProperties(key.Source);
            var destProps = PropertyCache.GetProperties(key.Dest);

            var mappings = new List<PropertyMapping>();

            foreach (var destProp in destProps)
            {
                if (!destProp.CanWrite) continue;

                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == destProp.Name);

                if (sourceProp != null)
                {
                    // Exact type match
                    if (sourceProp.PropertyType == destProp.PropertyType)
                    {
                        mappings.Add(new PropertyMapping(sourceProp, destProp, MappingType.Direct));
                    }
                    // Nested object mapping
                    else if (TypeHelper.IsComplexType(sourceProp.PropertyType) && 
                             TypeHelper.IsComplexType(destProp.PropertyType) &&
                             TypeHelper.HasParameterlessConstructor(destProp.PropertyType))
                    {
                        mappings.Add(new PropertyMapping(sourceProp, destProp, MappingType.Nested));
                    }
                }
            }

            return mappings.ToArray();
        });
    }
}

internal enum MappingType
{
    Direct,
    Nested
}

internal record PropertyMapping(PropertyInfo SourceProperty, PropertyInfo DestProperty, MappingType Type);