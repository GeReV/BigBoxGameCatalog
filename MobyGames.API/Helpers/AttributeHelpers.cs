using System.ComponentModel;
using System.Reflection;

namespace MobyGames.API.Helpers;

public static class AttributeHelpers
{
    public static T GetPropertyAttribute<T>(this object o, string propertyName) where T : Attribute
    {
        var propertyInfo = o.GetType().GetProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new Exception($"Could not find property \"{propertyName}\"");
        }

        return GetPropertyAttribute<T>(o, propertyInfo);
    }

    public static T GetPropertyAttribute<T>(this object o, PropertyInfo propertyInfo) where T : Attribute
    {
        var attribute = propertyInfo.GetCustomAttribute<T>();

        if (attribute is null)
        {
            throw new Exception(
                $"Could not find attribute of type \"{typeof(T)}\" on property \"{propertyInfo.Name}\""
            );
        }

        return attribute;
    }

    public static bool IsDefault<TObject>(this TObject o, string propertyName) where TObject : notnull
    {
        var propertyInfo = typeof(TObject).GetProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new Exception($"Could not find property \"{propertyName}\"");
        }

        var value = propertyInfo.GetValue(o);

        var attribute = GetPropertyAttribute<DefaultValueAttribute>(o, propertyInfo);

        return Equals(attribute.Value, value);
    }
}
