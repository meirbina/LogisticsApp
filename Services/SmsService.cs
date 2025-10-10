using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SMS.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Services
{
    public interface ISmsService
    {
        Task SendShipmentCreationSmsAsync(string phoneNumber, string waybillNumber, decimal totalCost);
        Task SendArrivalSmsAsync(string phoneNumber, string waybillNumber, string terminalName, string terminalAddress, bool isReceiver);
        Task SendCollectionSmsAsync(string senderPhoneNumber, string waybillNumber, string collectedByName);
        // --- NEW METHOD FOR THE INTERFACE ---
        Task SendEmployeeWelcomeSmsAsync(string phoneNumber, string email, string password);
    }

    public class SmsService : ISmsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TermiiSettings _termiiSettings;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IHttpClientFactory httpClientFactory, IOptions<TermiiSettings> termiiSettings, ILogger<SmsService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _termiiSettings = termiiSettings.Value;
            _logger = logger;
        }

        public async Task SendShipmentCreationSmsAsync(string phoneNumber, string waybillNumber, decimal totalCost)
        {
            if (!IsValidPhoneNumber(phoneNumber, waybillNumber)) return;
            var formattedPhone = FormatPhoneNumber(phoneNumber);
            var message = $"Your shipment with waybill number {waybillNumber} and cost {totalCost:N2} is created successfully.";
            await SendSmsAsync(formattedPhone, message, $"Waybill: {waybillNumber}");
        }

        public async Task SendArrivalSmsAsync(string phoneNumber, string waybillNumber, string terminalName, string terminalAddress, bool isReceiver)
        {
            if (!IsValidPhoneNumber(phoneNumber, waybillNumber)) return;
            var formattedPhone = FormatPhoneNumber(phoneNumber);
            string message = isReceiver
                ? $"Your shipment with waybill {waybillNumber} has arrived at our {terminalName} terminal. Please come for collection with a valid ID. Address: {terminalAddress}"
                : $"Your shipment with waybill {waybillNumber} has arrived at the destination terminal in {terminalName}.";
            await SendSmsAsync(formattedPhone, message, $"Waybill: {waybillNumber}");
        }

        public async Task SendCollectionSmsAsync(string senderPhoneNumber, string waybillNumber, string collectedByName)
        {
            if (!IsValidPhoneNumber(senderPhoneNumber, waybillNumber)) return;
            var formattedPhone = FormatPhoneNumber(senderPhoneNumber);
            var message = $"Your shipment with waybill number {waybillNumber} has been collected by {collectedByName}.";
            await SendSmsAsync(formattedPhone, message, $"Waybill: {waybillNumber}");
        }

        // --- NEW METHOD IMPLEMENTATION ---
        public async Task SendEmployeeWelcomeSmsAsync(string phoneNumber, string email, string password)
        {
            if (!IsValidPhoneNumber(phoneNumber, email)) return; // Use email for logging context
            var formattedPhone = FormatPhoneNumber(phoneNumber);
            var message = $"Welcome! Your login details are: Email={email}, Password={password}. Please change your password upon first login.";
            await SendSmsAsync(formattedPhone, message, $"Employee: {email}");
        }

        private async Task SendSmsAsync(string formattedPhoneNumber, string message, string loggingContext)
        {
             var payload = new
            {
                to = formattedPhoneNumber, from = _termiiSettings.SenderId, sms = message,
                type = "plain", channel = "generic", api_key = _termiiSettings.ApiKey
            };
            var client = _httpClientFactory.CreateClient();
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(_termiiSettings.ApiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send SMS for {Context}. Status: {StatusCode}, Response: {ResponseBody}", loggingContext, response.StatusCode, responseBody);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending SMS for {Context}", loggingContext);
            }
        }
        
        private bool IsValidPhoneNumber(string phoneNumber, string context)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("SMS not sent for {Context}: Phone number is empty.", context);
                return false;
            }
            return true;
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                return "234" + phoneNumber.Substring(1);
            }
            return phoneNumber;
        }
    }
}