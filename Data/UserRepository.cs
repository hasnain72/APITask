using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using UserApi.Models;

namespace UserApi.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> CreateUser(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO Users (Username, PasswordHash, FirstName, LastName, Device, IpAddress)
                VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Device, @IpAddress);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
            return await connection.ExecuteScalarAsync<long>(sql, user);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Users WHERE Username = @Username";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task LogLoginActivity(long userId, string ipAddress, string device, string browser)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO LoginActivities (UserId, IpAddress, Device, Browser)
                VALUES (@UserId, @IpAddress, @Device, @Browser)";
            await connection.ExecuteAsync(sql, new { UserId = userId, IpAddress = ipAddress, Device = device, Browser = browser });
        }
    }
}
