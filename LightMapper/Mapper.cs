using System.Reflection;

namespace LightMapper;

public static class Mapper
{
	public static TDestination Map<TSource, TDestination>(
		TSource source,
		MapperConfig? config = null)
		where TDestination : new()
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source));

		return (TDestination)MapInternal(source, typeof(TSource), typeof(TDestination), config)!;
	}

	private static object? MapInternal(object? source, Type sourceType, Type destType, MapperConfig? config)
	{
		if (source == null) return null;

		var destination = Activator.CreateInstance(destType)!;

		// Cache'den mapping bilgilerini al
		var mappings = MappingCache.GetMappings(sourceType, destType);
		var rule = config?.GetMap(sourceType, destType);

		// Önce cache'lenmiş mappingleri yap
		foreach (var mapping in mappings)
		{
			var propName = mapping.DestProperty.Name;

			// 1. Ignore kontrolü
			if (rule?.GetIgnoredMembers().Contains(propName) == true)
				continue;

			// 2. Custom mapping kontrolü
			if (rule?.HasCustomMapping(propName) == true)
			{
				var customValue = rule.GetCustomMapping(propName, source);
				mapping.DestProperty.SetValue(destination, customValue);
				continue;
			}

			// 3. Normal/Nested mapping
			var sourceValue = mapping.SourceProperty.GetValue(source);

			if (mapping.Type == MappingType.Direct)
			{
				mapping.DestProperty.SetValue(destination, sourceValue);
			}
			else if (mapping.Type == MappingType.Nested && sourceValue != null)
			{
				var nestedResult = MapInternal(sourceValue, mapping.SourceProperty.PropertyType,
					mapping.DestProperty.PropertyType, config);
				mapping.DestProperty.SetValue(destination, nestedResult);
			}
		}

		// Custom mappingleri işle (cache'de olmayan property'ler için)
		if (rule != null)
		{
			var destProps = PropertyCache.GetProperties(destType);

			foreach (var destProp in destProps)
			{
				if (!destProp.CanWrite) continue;

				var propName = destProp.Name;

				// Bu property zaten normal mapping'de işlenmişse skip et
				if (mappings.Any(m => m.DestProperty.Name == propName))
					continue;

				// Custom mapping var mı kontrol et
				if (rule.HasCustomMapping(propName))
				{
					var value = rule.GetCustomMapping(propName, source);
					destProp.SetValue(destination, value);
				}
			}
		}

		return destination;
	}


}