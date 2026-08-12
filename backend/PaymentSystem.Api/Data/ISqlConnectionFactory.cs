using System.Data;

namespace PaymentSystem.Api.Data;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}
