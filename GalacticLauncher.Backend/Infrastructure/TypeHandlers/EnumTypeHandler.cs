using Dapper;
using System.Data;

namespace GalacticLauncher.Backend.Infrastructure.TypeHandlers;

internal class EnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString().ToLowerInvariant();
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
