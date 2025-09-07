namespace LightMapper;

internal static class TypeHelper
{
    public static bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(decimal) || 
               type == typeof(DateTime) || 
               type == typeof(DateTimeOffset) || 
               type == typeof(TimeSpan) || 
               type == typeof(Guid) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                IsPrimitiveType(Nullable.GetUnderlyingType(type)!));
    }

    public static bool IsComplexType(Type type)
    {
        return !IsPrimitiveType(type) && 
               type != typeof(object) && 
               !type.IsEnum &&
               type.IsClass;
    }

    public static bool HasParameterlessConstructor(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) != null;
    }
}