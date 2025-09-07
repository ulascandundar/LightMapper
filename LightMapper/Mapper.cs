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

		var destination = new TDestination();

		var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		MappingRule? rule = config?.GetMap(typeof(TSource), typeof(TDestination));
		var custom = (rule as MappingRule<TSource, TDestination>) ?? null;

		foreach (var dProp in destProps)
		{
			if (!dProp.CanWrite) continue;

			// 1. Ignore kontrolü
			if (custom?.IgnoredMembers.Contains(dProp.Name) == true)
				continue;

			// 2. Custom mapping kontrolü
			if (custom != null && custom.CustomMappings.TryGetValue(dProp.Name, out var mapFunc))
			{
				var value = mapFunc(source!);
				dProp.SetValue(destination, value);
				continue;
			}

			// 3. Normal mapping (aynı isim + tip)
			var sProp = sourceProps.FirstOrDefault(p =>
				p.Name == dProp.Name &&
				p.PropertyType == dProp.PropertyType);

			if (sProp != null)
			{
				var value = sProp.GetValue(source);
				dProp.SetValue(destination, value);
			}
		}

		return destination;
	}
}