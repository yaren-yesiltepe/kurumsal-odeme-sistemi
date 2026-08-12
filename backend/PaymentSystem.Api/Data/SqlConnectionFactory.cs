using System.Data;
using Microsoft.Data.SqlClient;

namespace PaymentSystem.Api.Data;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connStr;

    public SqlConnectionFactory(IConfiguration config)
    {
        _connStr = config.GetConnectionString("PaymentDb")
            ?? throw new InvalidOperationException("PaymentDb connection string is missing.");
    }

    public IDbConnection Create() => new SqlConnection(_connStr);
}
