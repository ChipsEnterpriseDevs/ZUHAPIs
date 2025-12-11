using System.Data;
using System.Data.Odbc;
using Microsoft.Extensions.Configuration;

namespace ZUHS_APIs.Models
{
    public class DatabaseConnection : IDisposable
    {
        public OdbcConnection Connection { get; }
        private readonly IConfiguration _configuration;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            Connection = new OdbcConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public void OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        public void Dispose()
        {
            CloseConnection();
            Connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}