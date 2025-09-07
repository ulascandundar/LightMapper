using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMapper;
public class MapperConfig
{
	private readonly Dictionary<(Type, Type), MappingRule> _mappings = new();

	public MappingRule<TSource, TDestination> CreateMap<TSource, TDestination>()
		where TDestination : new()
	{
		var rule = new MappingRule<TSource, TDestination>();
		_mappings[(typeof(TSource), typeof(TDestination))] = rule;
		return rule;
	}

	internal MappingRule? GetMap(Type source, Type dest)
	{
		_mappings.TryGetValue((source, dest), out var rule);
		return rule;
	}
}