using System.Data;
using Microsoft.Data.SqlClient;

namespace DotNetWebApiStarter.Data
{
    public class DatabaseContext : IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private SqlConnection? _connection;

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetSection("DatabaseSettings:ConnectionString").Value;
        }

        public string? ConnectionString { get; }

        public async Task<IDbConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(ConnectionString);
                await _connection.OpenAsync().ConfigureAwait(false);
            }
            return _connection;
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await _connection.DisposeAsync().ConfigureAwait(false);
            }
            else if (_connection != null)
            {
                await _connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}