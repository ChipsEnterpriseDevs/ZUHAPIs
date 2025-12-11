using System.Data;

namespace ZUHS_APIs.Interfaces
{
    public interface IDatabaseService : IDisposable
    {
        DataTable ExecuteDataTable(string query);
        int ExecuteNonQuery(string query);
        object ExecuteScalar(string query);
    
    }
}