using System.Data;
using Microsoft.Extensions.Logging;
using Pervasive.Data.SqlClient;
using ZUHS_APIs.Intefaces;
using ZUHS_APIs.Interfaces;
using ZUHS_APIs.Services;

namespace ZUHS_APIs.Services
{
    public class PastelService : IPastelService
    {
        private readonly IDatabaseService _dbService;
        private readonly ILogger<PastelService> _logger;

        public PastelService(
            IDatabaseService dbService,
            ILogger<PastelService> logger)
        {
            _dbService = dbService;
            _logger = logger;
        }

        public DataTable GetCustomerDetails(string customerCode)
        {
            try
            {
                // Direct value passing in query (as per your requirement)
                string query = $@"select AccNumber from LedgerMaster where accNumber ='{customerCode}'";

                _logger.LogInformation($"Fetching customer {customerCode}");
                return _dbService.ExecuteDataTable(query);
            }
            catch (PsqlException ex)
            {
                _logger.LogError(ex, $"Pervasive SQL error while fetching customer {customerCode}");
                throw new ApplicationException("Database operation failed", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching customer {customerCode}");
                throw;
            }
        }

      
    }
}