using ZUHS_APIs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZUHS_APIs.Interfaces;
using ZUHS_APIs.Models;
using ZUHS_APIs.Intefaces;
using Escrow_Pastel_Integration;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authorization;

namespace ZUHS_APIs.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PastelController : ControllerBase  
    {
        private readonly IPastelService _pastelService;
        private readonly ILogger<PastelController> _logger;

        public PastelController(
            IPastelService pastelService,
            ILogger<PastelController> logger)
        {
            _pastelService = pastelService;
            _logger = logger;
        }
        //[HttpPost("Invoice")]
        //public async Task<IActionResult> PostInvoice([FromBody] InvoiceHeader invoiceRequest)
        //{
        //    try
        //    {
        //        // Validate request
        //        if (invoiceRequest == null)
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "Invoice request is required",
        //                Data = (object)null
        //            });
        //        }

        //        // Validate required fields
        //        if (string.IsNullOrWhiteSpace(invoiceRequest.CustomerCode) ||
                    
        //            invoiceRequest.InvoiceLines == null ||
        //            !invoiceRequest.InvoiceLines.Any())
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "CustomerCode, DocNumber, and at least one InvoiceLine are required",
        //                Data = (object)null
        //            });
        //        }

        //        // Validate required fields
        //        var validationErrors = ValidateInvoiceRequest(invoiceRequest);
        //        if (validationErrors.Any())
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "Validation failed",
        //                Errors = validationErrors
        //            });
        //        }
        //        // Validate customer exists in Pastel
        //        var customerValidationResult = await ValidateCustomerExists(invoiceRequest.CustomerCode);
        //        if (!customerValidationResult.IsValid)
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "Customer validation failed",
        //                Error = customerValidationResult.ErrorMessage
        //            });
        //        }

        //        // Set Pastel paths
        //        string GLPath = "C:\\Pastel19";
        //        string DataPath = "C:\\Pastel19\\Test";

        //        _logger.LogInformation("Attempting to post invoice to Pastel for customer: {CustomerCode}, Document: {DocNumber}",
        //            invoiceRequest.CustomerCode, invoiceRequest.DocNumber);

        //        // Convert InvoiceRequest to required parameters for PostInvoiceToPastel
        //        string result = Partner_DLL.Transactions.PostInvoiceToPastel(
        //            DataPath: DataPath,
        //            GLPath: GLPath,
        //            DocNumber: invoiceRequest.DocNumber,
        //            CustomerCode: invoiceRequest.CustomerCode,
        //            Date: invoiceRequest.Date ?? DateTime.Now.ToString("dd/MM/yyyy"),
        //            OrderNumber: invoiceRequest.OrderNumber ?? "",
        //            InvoiceMessage1: invoiceRequest.InvoiceMessage1 ?? "",
        //            DeliveryAddress1: invoiceRequest.DeliveryAddress1 ?? "",
        //            ExchangeRate:invoiceRequest.ExchangeRate ?? "",
        //            invoiceLines: invoiceRequest.InvoiceLines
        //        );

        //        if (result.StartsWith("0|"))
        //        {
        //            string invoiceNumber = result.Substring(2);
        //            _logger.LogInformation($"Invoice {invoiceNumber}  successfully posted to Pastel.",
        //                invoiceRequest.DocNumber, invoiceRequest.CustomerCode);

        //            return Ok(new
        //            {
        //                Success = true,
        //                Message = $"Invoice {invoiceNumber} successfully posted to Pastel.",
        //                Data = new { DocumentNumber = invoiceRequest.DocNumber, Result = result }
        //            });
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Failed to post invoice to Pastel for customer {CustomerCode}. Result: {Result}",
        //                invoiceRequest.CustomerCode, result);

        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = $"Failed to post invoice. Pastel returned an error.",
        //                Error = $"Pastel Result: {result}"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while posting invoice for customer: {CustomerCode}",
        //            invoiceRequest?.CustomerCode);

        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "An internal server error occurred while processing your request.",
        //            Error = ex.Message
        //        });
        //    }
        //}


       
        //[HttpPost("Customer")]
        //public IActionResult CheckCustomerExists([FromBody] Customer request)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(request?.CustomerCode))
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "Customer code is required",
        //                Data = (object)null
        //            });
        //        }

        //        _logger.LogInformation("Checking customer existence for code: {CustomerCode}", request.CustomerCode);

        //        DataTable customerData = _pastelService.GetCustomerDetails(request.CustomerCode);

        //        if (customerData == null || customerData.Rows.Count == 0)
        //        {
        //            return NotFound(new
        //            {
        //                Success = false,
        //                Message = "Customer not found",
        //                Data = (object)null
        //            });
        //        }


        //        // Get the first row from the DataTable
        //        var customerRow = customerData.Rows[0];

        //        // Extract customer details including currency
        //        var customerCode = customerRow["CustomerCode"]?.ToString();
        //        var currencyCode = customerRow["CurrencyCode"]?.ToString();

        //        // Convert DataTable to a more API-friendly format
        //        var result = new
        //        {
        //            Success = true,
        //            Message = "Customer found successfully",
        //            Data = new
        //            {
        //                CustomerCode = customerCode,
        //                CurrencyCode = currencyCode
        //            }
        //        };

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error checking customer existence for code: {CustomerCode}", request?.CustomerCode);

        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "An error occurred while processing your request",
        //            Error = ex.Message
        //        });
        //    }
        //}



        [HttpPost("Journal")]
        public async Task<IActionResult> Journal([FromBody] JournalRequest journalrequest)
        {
            try
            {

                if (journalrequest == null || string.IsNullOrWhiteSpace(journalrequest.AccountNumber) || string.IsNullOrWhiteSpace(journalrequest.Amount))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Account Number and Amount are required.",
                        Data = (object)null
                    });
                }
                var customerValidationResult = await ValidateAccountExists(journalrequest.AccountNumber);
                    if (!customerValidationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Customer validation failed",
                        Error = customerValidationResult.ErrorMessage
                    });
                }

                string GLPath = "C:\\Pastel19";
                //string DataPath = "C:\\Pastel19\\_Demo";
                string DataPath = "C:\\Pastel19\\SMED2025ZWG";
                string Period = "10";
                string GDC = "G";
                short EntryType = 0;


                _logger.LogInformation("Attempting to post payment to Pastel for customer: {CustomerCode}", journalrequest.AccountNumber);

                string pastelImportResult = Partner_DLL.Transactions.PostJournalToPastel(
                    DataPath,
                    GLPath,
                    journalrequest.Date,
                    GDC,
                    journalrequest.AccountNumber,
                    journalrequest.Reference,
                    journalrequest.Description,
                    journalrequest.Amount,
                    journalrequest.TaxType,
                    Period,
                    journalrequest.TaxAmount,
                    journalrequest.ExchangeRate,
                    journalrequest.Currency,
                     journalrequest.Contra


                );

                if (pastelImportResult == "0")
                {
                    _logger.LogInformation("Payment for customer {CustomerCode} successfully posted to Pastel.", journalrequest.AccountNumber);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Payment receipt successfully posted to Pastel.",
                        Data = new { PastelResultCode = pastelImportResult }
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to post payment to Pastel for customer {CustomerCode}. Pastel SDK returned code: {ResultCode}", journalrequest.AccountNumber, pastelImportResult);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Failed to post payment. Pastel SDK returned an error.",
                        Error = $"Pastel SDK Error Code: {pastelImportResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while posting payment for customer: {CustomerCode}", journalrequest?.AccountNumber);


                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An internal server error occurred while processing your request.",
                    Error = ex.Message
                });
            }
        }



        //[HttpPost("Payment")]
        //public async Task<IActionResult> Payment([FromBody] PaymentRequest paymentRequest)
        //{
        //    try
        //    {

        //        if (paymentRequest == null || string.IsNullOrWhiteSpace(paymentRequest.CustomerCode) || string.IsNullOrWhiteSpace(paymentRequest.Amount))
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "CustomerCode and Amount are required.",
        //                Data = (object)null
        //            });
        //        }
        //        var customerValidationResult = await ValidateCustomerExists(paymentRequest.CustomerCode);
        //        if (!customerValidationResult.IsValid)
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = "Customer validation failed",
        //                Error = customerValidationResult.ErrorMessage
        //            });
        //        }

        //        string GLPath = "C:\\Pastel19";
        //        string DataPath = "C:\\Pastel19\\_Demo";
        //        string DataPath = "C:\\Pastel19\\Test";
        //        string Period = "13";
        //        string GDC = "D";
        //        short EntryType = 0;


        //        _logger.LogInformation("Attempting to post payment to Pastel for customer: {CustomerCode}", paymentRequest.CustomerCode);

        //        string pastelImportResult = Partner_DLL.Transactions.PostPaymentsReceiptsToPastel(
        //            DataPath,
        //            GLPath,
        //            paymentRequest.Date,
        //            GDC,
        //            paymentRequest.CustomerCode,
        //            paymentRequest.AccountNumber,
        //            paymentRequest.Reference,
        //            paymentRequest.Description,
        //            paymentRequest.Amount,
        //            paymentRequest.TaxType,
        //            Period,
        //            paymentRequest.TaxAmount,
        //            paymentRequest.ExchangeRate,
        //            paymentRequest.Currency,
        //             paymentRequest.Contra


        //        );

        //        if (pastelImportResult == "0")
        //        {
        //            _logger.LogInformation("Payment for customer {CustomerCode} successfully posted to Pastel.", paymentRequest.CustomerCode);

        //            return Ok(new
        //            {
        //                Success = true,
        //                Message = "Payment receipt successfully posted to Pastel.",
        //                Data = new { PastelResultCode = pastelImportResult }
        //            });
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Failed to post payment to Pastel for customer {CustomerCode}. Pastel SDK returned code: {ResultCode}", paymentRequest.CustomerCode, pastelImportResult);

        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = $"Failed to post payment. Pastel SDK returned an error.",
        //                Error = $"Pastel SDK Error Code: {pastelImportResult}"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while posting payment for customer: {CustomerCode}", paymentRequest?.CustomerCode);


        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "An internal server error occurred while processing your request.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        private List<string> ValidateInvoiceRequest(InvoiceHeader invoiceRequest)
        {
            var errors = new List<string>();

            // Required field validations
            if (string.IsNullOrWhiteSpace(invoiceRequest.CustomerCode))
                errors.Add("CustomerCode is required");


            if (string.IsNullOrWhiteSpace(invoiceRequest.Date))
                errors.Add("Date is required");

            if (string.IsNullOrWhiteSpace(invoiceRequest.ExchangeRate))
                errors.Add("ExchangeRate is required");

            // Validate invoice lines
            if (invoiceRequest.InvoiceLines == null || !invoiceRequest.InvoiceLines.Any())
            {
                errors.Add("At least one InvoiceLine is required");
            }
            else
            {
                // Validate each invoice line
                for (int i = 0; i < invoiceRequest.InvoiceLines.Count; i++)
                {
                    var line = invoiceRequest.InvoiceLines[i];
                    var lineIndex = i + 1;

                    if (string.IsNullOrWhiteSpace(line.Code))
                        errors.Add($"InvoiceLine {lineIndex}: Code is required");

                    if (line.Quantity <= 0)
                        errors.Add($"InvoiceLine {lineIndex}: Quantity must be greater than 0");

                    if (line.UnitSellingPrice <= 0)
                        errors.Add($"InvoiceLine {lineIndex}: UnitSellingPrice must be greater than 0");

                    if (string.IsNullOrWhiteSpace(line.Description))
                        errors.Add($"InvoiceLine {lineIndex}: LineDescription is required");
                }
            }

            return errors;
        }
        private async Task<CustomerValidationResult> ValidateCustomerExists(string customerCode)
        {
            try
            {
                _logger.LogInformation("Validating customer existence for code: {CustomerCode}", customerCode);

                DataTable customerData = _pastelService.GetCustomerDetails(customerCode);

                if (customerData == null || customerData.Rows.Count == 0)
                {
                    _logger.LogWarning("Customer not found in Pastel: {CustomerCode}", customerCode);
                    return new CustomerValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Customer with code '{customerCode}' does not exist in the system."
                    };
                }

                // Optional: Additional customer validation
                var firstRow = customerData.Rows[0];

                // Check if customer is active (assuming there's an 'Active' field)
                if (customerData.Columns.Contains("Active"))
                {
                    var isActive = Convert.ToBoolean(firstRow["Active"]);
                    if (!isActive)
                    {
                        _logger.LogWarning("Customer is inactive: {CustomerCode}", customerCode);
                        return new CustomerValidationResult
                        {
                            IsValid = false,
                            ErrorMessage = $"Customer '{customerCode}' is inactive."
                        };
                    }
                }

                _logger.LogInformation("Customer validation successful for code: {CustomerCode}", customerCode);
                return new CustomerValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating customer existence for code: {CustomerCode}", customerCode);
                return new CustomerValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Error validating customer: {ex.Message}"
                };
            }
        }


        private async Task<CustomerValidationResult> ValidateAccountExists(string AccountNumber)
        {
            try
            {
          

                DataTable customerData = _pastelService.GetCustomerDetails(AccountNumber);

                if (customerData == null || customerData.Rows.Count == 0)
                {
                    _logger.LogWarning("Customer not found in Pastel: {CustomerCode}", AccountNumber);
                    return new CustomerValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Customer with code '{AccountNumber}' does not exist in the system."
                    };
                }

                // Optional: Additional customer validation
                var firstRow = customerData.Rows[0];

                // Check if customer is active (assuming there's an 'Active' field)
                if (customerData.Columns.Contains("Active"))
                {
                    var isActive = Convert.ToBoolean(firstRow["Active"]);
                    if (!isActive)
                    {
                        _logger.LogWarning("Customer is inactive: {CustomerCode}", AccountNumber);
                        return new CustomerValidationResult
                        {
                            IsValid = false,
                            ErrorMessage = $"Customer '{AccountNumber}' is inactive."
                        };
                    }
                }

                _logger.LogInformation("Customer validation successful for code: {CustomerCode}", AccountNumber);
                return new CustomerValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating customer existence for code: {CustomerCode}", AccountNumber);
                return new CustomerValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Error validating customer: {ex.Message}"
                };
            }
        }
        public class CustomerValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}