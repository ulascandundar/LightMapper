using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LightMapper;

public abstract class MappingRule 
{
	internal abstract HashSet<string> GetIgnoredMembers();
	internal abstract object? GetCustomMapping(string propertyName, object source);
	internal abstract bool HasCustomMapping(string propertyName);
}

public class MappingRule<TSource, TDestination> : MappingRule
	where TDestination : new()
{
	internal Dictionary<string, Func<TSource, object?>> CustomMappings { get; } = new();
	internal HashSet<string> IgnoredMembers { get; } = new();

	internal override HashSet<string> GetIgnoredMembers() => IgnoredMembers;

	internal override object? GetCustomMapping(string propertyName, object source)
	{
		if (CustomMappings.TryGetValue(propertyName, out var mapFunc))
		{
			return mapFunc((TSource)source);
		}
		return null;
	}

	internal override bool HasCustomMapping(string propertyName)
	{
		return CustomMappings.ContainsKey(propertyName);
	}

	public MappingRule<TSource, TDestination> ForMember<TProperty>(
		Expression<Func<TDestination, TProperty>> destSelector,
		Func<TSource, object?> mapFunc)
	{
		if (destSelector.Body is MemberExpression member)
		{
			CustomMappings[member.Member.Name] = mapFunc;
		}
		return this;
	}

	public MappingRule<TSource, TDestination> Ignore<TProperty>(
		Expression<Func<TDestination, TProperty>> destSelector)
	{
		if (destSelector.Body is MemberExpression member)
		{
			IgnoredMembers.Add(member.Member.Name);
		}
		return this;
	}
}