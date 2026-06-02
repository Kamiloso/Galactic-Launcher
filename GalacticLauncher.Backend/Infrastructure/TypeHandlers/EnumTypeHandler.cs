using Dapper;
using System.Data;

namespace GalacticLauncher.Backend.Infrastructure.TypeHandlers;

internal class EnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        if (!Enum.IsDefined(typeof(T), value))
            value = default;

        parameter.DbType = DbType.String;
        parameter.Value = value.ToString().ToLowerInvariant();
    }

    public override T Parse(object value)
    {
        if (value is string str &&
            Enum.TryParse(str, true, out T result))
        {
            return result;
        }

        return default;
    }
}
