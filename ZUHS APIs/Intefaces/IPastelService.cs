using System.Data;

namespace ZUHS_APIs.Intefaces
{
    public interface IPastelService
    {
        DataTable GetCustomerDetails(string customerCode);
    }
}
