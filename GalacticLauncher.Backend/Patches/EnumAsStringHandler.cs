using Dapper;
using System.Data;

namespace GalacticLauncher.Backend.Patches;

public class EnumAsStringHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value.ToString().ToLowerInvariant();
        parameter.DbType = DbType.String;
    }

    public override T Parse(object value)
    {
        if (value is string stringValue)
        {
            if (Enum.TryParse<T>(stringValue, true, out var result))
            {
                return result;
            }
        }

        throw new ArgumentException($"Cannot parse '{value}' to enum {typeof(T).Name}");
    }
}
