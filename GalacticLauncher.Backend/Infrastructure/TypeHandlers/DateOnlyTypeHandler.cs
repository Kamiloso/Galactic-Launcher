using System.Data;
using Dapper;

namespace GalacticLauncher.Backend.Infrastructure.TypeHandlers;

public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        if (value is DateTime dt)
            return DateOnly.FromDateTime(dt);

        throw new InvalidCastException($"Cannot cast {value.GetType()} to DateOnly");
    }
}
