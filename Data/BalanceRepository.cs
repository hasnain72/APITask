using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class BalanceRepository
{
    private readonly string _connectionString;

    public BalanceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<decimal> GetBalance(long userId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT Balance FROM Balances WHERE UserId = @UserId";
        return await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { UserId = userId });
    }

    public async Task AddBalance(long userId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Balances WHERE UserId = @UserId)
            BEGIN
                UPDATE Balances SET Balance = Balance + @Amount WHERE UserId = @UserId
            END
            ELSE
            BEGIN
                INSERT INTO Balances (UserId, Balance) VALUES (@UserId, @Amount)
            END";
        await connection.ExecuteAsync(sql, new { UserId = userId, Amount = amount });
    }
}
