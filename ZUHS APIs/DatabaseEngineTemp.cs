using Microsoft.Win32;
using Pervasive.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Escrow_Pastel_Integration
{
    class DatabaseEngineTemp
    {
        public static PsqlConnection ConPartner { get; set; } = new PsqlConnection();

        public static void UpdateConnection()
        {
            string ServerName = "";
            string DatabaseName = "";
            ConPartner = new PsqlConnection(@"driver={Pervasive ODBC Client Interface};ServerName=" + ServerName + ";DBQ=" + DatabaseName + ";");

        }

        public static void ExecuteNonQuery(string query)
        {
            try
            {
                UpdateConnection();
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
        public static string ExecuteScalar(string query)
        {
            UpdateConnection();
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
            UpdateConnection();
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

        public static bool CustomerExistsInTemp(string CustomerId)
        {
            string query = $"select count(*) from IntegrationDebtors where CustomerId = {CustomerId}";
            int count = Convert.ToInt32(ExecuteScalar(query));
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public static bool PaymentExistsInTemp(string PaymentsId)
        {
            string query = $"select count(*) from IntegrationPayments where PaymentId = {PaymentsId}";
            int count = Convert.ToInt32(ExecuteScalar(query));
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public static bool InvoiceExistsInTemp(string InvoiceId)
        {
            string query = $"select count(*) from IntegrationInvoices where InvoiceId = {InvoiceId}";
            int count = Convert.ToInt32(ExecuteScalar(query));
            if (count != 0)
            {
                return true;
            }
            return false;
        }
        public static bool JournalExistsInTemp(string InvoiceId)
        {
            string query = $"select count(*) from IntegrationJournals where JournalId = {InvoiceId}";
            int count = Convert.ToInt32(ExecuteScalar(query));
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public static DataTable AccountMapping(string bnk, string typ)
        {
            string query = $"select * from EntryType where AccEscrow='{bnk}' and TranType='{typ}'";
            return ExecuteDataTable(query);
        }
        public static DataTable AccountMapping()
        {
            string query = "select * from AccountMapping";
            return ExecuteDataTable(query);
        }
        public static DataTable EntryTransaction()
        {
            string query = "select * from EntryType";
            return ExecuteDataTable(query);
        }


       

      
        public static void SaveEmails(string Email)
        {
            DatabaseEngineTemp.ExecuteNonQuery($"insert into Emails values('{Email}')");
        }
        public static void SaveMapping(string eacc, string pacc)
        {
            DatabaseEngineTemp.ExecuteNonQuery($"insert into AccountMapping values('{eacc}','{pacc}')");
        }
        public static void SaveEntryType(string eacc, string etyp, int ttyp)
        {
            DatabaseEngineTemp.ExecuteNonQuery($"insert into EntryType values('{eacc}','{etyp}',{ttyp})");
        }


        public static DataTable GetUnsendCustomersFromTemp()
        {
            return ExecuteDataTable("select CustomerCodePastel, CustomerName from IntegrationDebtors where Status = 'N'");
        }

        public static DataTable GetUnsendInvoicesPaymentsFromTemp()
        {
            string query = @"
                select 				i.InvoiceId
					                , i.Date
					                , i.Bank
					                , i.BankAmount
					                , i.LoanType
					                , i.SundryAmt
					                , i.InterestEarnedAmt
					                , i.InterestDeferredAmt
					                , d.CustomerCodePastel
                                    , d.CustomerName
                                    , i.Curr
                from                IntegrationInvoices i
                left join           IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode
                where               i.PaymtStatus in ('N','R')
                and 				date>'2023-12-01'
           
            ";
            return ExecuteDataTable(query);
        }
        public static DataTable GetUnsendInvoicesReceiptsFromTemp()
        {
            string query = @"
                select 				i.InvoiceId
					                , i.Date
					                , i.Bank
					                , i.BankAmount
					                , i.LoanType
					                , i.SundryAmt
					                , i.InterestEarnedAmt
					                , i.InterestDeferredAmt
					                , d.CustomerCodePastel
                                    , d.CustomerName
                                    , i.Curr
                from                IntegrationInvoices i
                left join           IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode
                where               i.RcptStatus in ('N','R')
                and 				i.date>'2023-12-01'
   
            ";
            return ExecuteDataTable(query);
        }

        public static DataTable GetUnsendPaymentsReceiptsFromTemp()
        {
            string query = @"
                select 			i.PaymentId
				                , i.Date 
				                , i.Description
                                , i.Bank
				                , i.BankAmount
				                , i.CommissionAmount
				                , d.CustomerCodePastel
				                , d.CustomerName
                                , i.Curr
                from            IntegrationPayments i
                left join       IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode
                where i.Status in ('N', 'R')
                and bank not in ('245/5','302/1')
                and CustomerCodePastel!=''
                
            ";
            return ExecuteDataTable(query);
        }
        public static DataTable GetEmails()
        {
            string query = @"select * from Emails ";
            return ExecuteDataTable(query);
        }
        public static DataTable JournalMappng(string acc)
        {
            return ExecuteDataTable($"select AccPastel from AccountMapping where AccEscrow='{acc}'");
        }
        public static DataTable GetUnsendGLJournalsFromTemp()
        {
            string query = @"
                select 				j.JournalId
					                , j.Date
					                , j.AccountCode
					                , j.Amount
					                , j.Reference
                                    , j.Description
                                    , isDebit 
                                    , d.CustomerCodePastel
                                    , d.CustomerName
                                    ,j.AccountCode
                                    ,j.Curr
                from                IntegrationJournals j
                left join           IntegrationDebtors d on d.CustomerCodeEscrow = j.AccountCode
                where               j.Status in ('N','R')
                Order by 			JournalId
            ";
            return ExecuteDataTable(query);
        }
        public static DataTable GetUnsendInvoicesCustomerJournalsFromTemp()
        {
            string query = @"
                select 				i.InvoiceId
					                , i.Date
					                , i.Bank
					                , i.BankAmount
					                , i.LoanType
					                , i.SundryAmt
					                , i.InterestEarnedAmt
					                , i.InterestDeferredAmt
					                , d.CustomerCodePastel
                                    , d.CustomerName
                                    , i.Curr
                from                IntegrationInvoices i
                left join           IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode
                where               i.IntEarnStatus in ('N','R')
            ";
            return ExecuteDataTable(query);
        }

        public static bool FlagInvoicePaymentInTemp(string InvoiceId, string Flag, string Message)
        {
            string query = $"update IntegrationInvoices set PaymtStatus = '{Flag}', PaymtMsg = '{Message}' where InvoiceId = {InvoiceId}";
            ExecuteNonQuery(query);
            return true;
        }
        public static bool FlagInvoiceReceiptInTemp(string InvoiceId, string Flag, string Message)
        {
            string query = $"update IntegrationInvoices set RcptStatus = '{Flag}', RcptMsg = '{Message}' where InvoiceId = {InvoiceId}";
            ExecuteNonQuery(query);
            return true;
        }
        public static bool FlagInvoiceCustomerJournalInTemp(string InvoiceId, string Flag, string Message)
        {
            string query = $"update IntegrationInvoices set IntEarnStatus = '{Flag}', IntEarnMsg = '{Message}' where InvoiceId = {InvoiceId}";
            ExecuteNonQuery(query);
            return true;
        }
        public static bool FlagPaymentReceiptInTemp(string PaymentId, string Flag, string Message)
        {
            string query = $"update IntegrationPayments set Status = '{Flag}', Message = '{Message}' where PaymentId = {PaymentId}";
            ExecuteNonQuery(query);
            return true;
        }
        public static bool FlagGLJournalInTemp(string refr, string Flag, string Message)
        {
            string query = $"update IntegrationJournals set Status = '{Flag}', Message = '{Message}' where Reference = '{refr}'";
            ExecuteNonQuery(query);
            return true;
        }

        public static DataTable GetReportData(string coStatus, string Status, string StartDate, string EndDate)
        {
            string query = $@"
                select * from 
                (
	                select 					p.PaymentId						TransactionId
							                , p.Date					TransactionDate
							                , p.CustomerCode                CustomerCodeEscrow
                                            , d.CustomerCodePastel          CustomerCodePastel
                                            , p.BankAmount                  Amount
                                            , p.Status                      Status
                                            , p.Message                     Message
                                            , 'Repayments'                  TransactionType
                    from                    IntegrationPayments p

                    left join               IntegrationDebtors d on d.CustomerCodeEscrow = p.CustomerCode

                    union
                    --Invoice Receipts

                    select i.InvoiceId
							                , i.Date
							                , i.CustomerCode
							                , d.CustomerCodePastel
							                , i.SundryAmt
							                , i.IntEarnStatus
							                , i.IntEarnMsg
							                , 'Sundry Receipt'

                    from IntegrationInvoices i

                    left
                    join IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode

                union all
                --Invoice Payments

                    select i.InvoiceId
							                , i.Date
							                , i.CustomerCode
							                , d.CustomerCodePastel
							                , i.SundryAmt + i.BankAmount
							                , i.PaymtStatus
							                , i.PaymtMsg
							                , 'Invoice Payment'

                    from IntegrationInvoices i

                    left
                    join IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode

                union all

                --Customer Journal

                    select i.InvoiceId
							                , i.Date
							                , i.CustomerCode
							                , d.CustomerCodePastel
							                , i.SundryAmt + i.BankAmount
							                , i.IntEarnStatus
							                , i.IntEarnMsg
							                , 'Customer Journal'

                    from IntegrationInvoices i

                    left
                    join IntegrationDebtors d on d.CustomerCodeEscrow = i.CustomerCode
	
                ) z
                where 1=1
                and z.TransactionDate between '{StartDate}' and '{EndDate}'
                {coStatus}and z.Status in ({Status})
            ";
            return ExecuteDataTable(query);
        }
        public static bool DeleteMapping()
        {
            string query = $"delete AccountMapping";
            ExecuteNonQuery(query);
            return true;
        }

        public static bool DeleteEntryType()
        {
            string query = $"delete EntryType";
            ExecuteNonQuery(query);
            return true;
        }
        public static bool DeleteEmails()
        {
            string query = $"delete Emails";
            ExecuteNonQuery(query);
            return true;
        }
    }
}
