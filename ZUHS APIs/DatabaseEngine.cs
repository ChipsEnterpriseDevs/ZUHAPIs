using Microsoft.Win32;
using Pervasive.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Escrow_Pastel_Integration
{
    class DatabaseEngine
    {
        public static PsqlConnection ConPartner { get; set; } = new PsqlConnection();

        public static void UpdateConnection(string cur)
        {
            string ServerName = "";
            string DatabaseName = "";
            if (cur != "USD")
            {
                ServerName = "";
                DatabaseName = "";
            }
            else
            {
                ServerName = "";
                DatabaseName = "";
            }
            ConPartner = new PsqlConnection(@"driver={Pervasive ODBC Client Interface};ServerName="+ServerName+";DBQ="+DatabaseName+";");
        }

        public static void ExecuteNonQuery(string query,string cur)
        {
            try
            {
                UpdateConnection(cur);
                PsqlCommand cmdQuery = new PsqlCommand(query, ConPartner);
                ConPartner.Open();
                cmdQuery.ExecuteNonQuery();
                ConPartner.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ConPartner != null)
                {
                    ConPartner.Close();
                }
            }
        }
        public static string ExecuteScalar(string query,string cur)
        {
            UpdateConnection(cur);
            string Result = "";
            try
            {
                PsqlCommand cmdQuery = new PsqlCommand(query, ConPartner);
                ConPartner.Open();
                Result = Convert.ToString(cmdQuery.ExecuteScalar());
                ConPartner.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ConPartner != null)
                {
                    ConPartner.Close();
                }
            }
            return Result;
        }
        public static DataTable ExecuteDataTable(string query)
        {
            UpdateConnection("");
            DataTable Result = new DataTable();
            try
            {
                PsqlCommand cmdQuery = new PsqlCommand(query, ConPartner);
                PsqlDataAdapter dtaResult = new PsqlDataAdapter(query, ConPartner);
                dtaResult.Fill(Result);
            }
            catch
            {
                throw;
            }
            return Result;
        }

        public static bool CustomerExistsInPastel(string CustomerCode,string cur)
        {
            string query = $"select count(*) from CustomerMaster where CustomerCode = '{CustomerCode}'";
            int count = Convert.ToInt32(ExecuteScalar(query,cur));
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public static string GetPeriod(string Date,string cur)
        {
            Date = Convert.ToDateTime(Date).ToString("yyyy-MM-dd");
            string query = $@"select isnull(max(period),-1) from
                            (
	                            select max(PerStartThis01) Date,1 Period from LedgerParameters
	                            union
	                            select max(PerStartThis02),2 from LedgerParameters
	                            union
	                            select max(PerStartThis03),3 from LedgerParameters
	                            union
	                            select max(PerStartThis04),4 from LedgerParameters
	                            union
	                            select max(PerStartThis05),5 from LedgerParameters
	                            union
	                            select max(PerStartThis06),6 from LedgerParameters
	                            union
	                            select max(PerStartThis07),7 from LedgerParameters
	                            union
	                            select max(PerStartThis08),8 from LedgerParameters
	                            union
	                            select max(PerStartThis09),9 from LedgerParameters
	                            union
	                            select max(PerStartThis10),10 from LedgerParameters
	                            union
	                            select max(PerStartThis11),11 from LedgerParameters
	                            union
	                            select max(PerStartThis12),12 from LedgerParameters
	                            union
	                            select max(PerStartThis13),13 from LedgerParameters
                            ) p
                            where 	month(p.Date) = month('{Date}')
                            and		year(p.Date) = year('{Date}')";
            string period = DatabaseEngine.ExecuteScalar(query,cur);
            return period;
        }
        public static string GetUserId(string Username, string cur)
        {
            string query = $@"select isnull(max(ID),-1) from AccountUser where Description = '{Username}'";
            string Id = DatabaseEngine.ExecuteScalar(query,cur);
            return Id;
        }
        
    }
}
