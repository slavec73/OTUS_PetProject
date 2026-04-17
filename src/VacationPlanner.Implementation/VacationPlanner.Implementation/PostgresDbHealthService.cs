using Microsoft.Extensions.Configuration;
using Npgsql;
using VacationPlanner.Interfaces;

namespace VacationPlanner.Implementation
{
    public class PostgresDbHealthService : IDbHealthService
    {
        private readonly IConfiguration _config;

        public PostgresDbHealthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> CanConnectAsync()
        {
            try
            {
                var connString = _config.GetConnectionString("DefaultConnection");

                await using var connection = new NpgsqlConnection(connString);
                await connection.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT 1", connection);
                await cmd.ExecuteScalarAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
