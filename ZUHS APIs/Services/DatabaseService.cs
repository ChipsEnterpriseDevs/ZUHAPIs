

using System.Data;

using System.Data.Odbc;

using Microsoft.Extensions.Logging;

using ZUHS_APIs.Interfaces;

using ZUHS_APIs.Models;



namespace ZUHS_APIs.Services

{

    public class DatabaseService : IDatabaseService

    {

        private readonly DatabaseConnection _dbConnection;

        private readonly ILogger<DatabaseService> _logger;



        public DatabaseService(DatabaseConnection dbConnection, ILogger<DatabaseService> logger)

        {

            _dbConnection = dbConnection;

            _logger = logger;

        }



        public DataTable ExecuteDataTable(string query)

        {

            var result = new DataTable();

            try

            {

                _logger.LogDebug("Executing query: {Query}", query);

                _dbConnection.OpenConnection();



                using var adapter = new OdbcDataAdapter(query, _dbConnection.Connection);

                adapter.Fill(result);



                _logger.LogDebug("Query executed successfully. Returned {RowCount} rows", result.Rows.Count);

                return result;

            }

            catch (OdbcException ex)

            {

                _logger.LogError(ex, "ODBC Error executing query: {Query}", query);

                throw new ApplicationException($"Database error: {SanitizeErrorMessage(ex.Message)}", ex);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Unexpected error executing query: {Query}", query);

                throw;

            }

            finally

            {

                _dbConnection.CloseConnection();

            }

        }



        public int ExecuteNonQuery(string query)

        {

            try

            {

                _logger.LogDebug("Executing non-query: {Query}", query);

                _dbConnection.OpenConnection();



                using var cmd = new OdbcCommand(query, _dbConnection.Connection);

                return cmd.ExecuteNonQuery();

            }

            catch (OdbcException ex)

            {

                _logger.LogError(ex, "ODBC Error executing non-query: {Query}", query);

                throw new ApplicationException($"Database error: {SanitizeErrorMessage(ex.Message)}", ex);

            }

            finally

            {

                _dbConnection.CloseConnection();

            }

        }



        public object ExecuteScalar(string query)

        {

            try

            {

                _logger.LogDebug("Executing scalar query: {Query}", query);

                _dbConnection.OpenConnection();



                using var cmd = new OdbcCommand(query, _dbConnection.Connection);

                return cmd.ExecuteScalar();

            }

            catch (OdbcException ex)

            {

                _logger.LogError(ex, "ODBC Error executing scalar query: {Query}", query);

                throw new ApplicationException($"Database error: {SanitizeErrorMessage(ex.Message)}", ex);

            }

            finally

            {

                _dbConnection.CloseConnection();

            }

        }




        private string SanitizeErrorMessage(string error)

        {

            return error.Replace(_dbConnection.Connection.ConnectionString, "[CONNECTION_STRING]");

        }



        public void Dispose()

        {

            _logger.LogInformation("Disposing database resources");

            _dbConnection?.Dispose();

            GC.SuppressFinalize(this);

        }

    }

}