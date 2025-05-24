using PSK2025.ApiService.Services.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PSK2025.ApiService.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public TwilioSmsService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:SID"] ?? throw new InvalidOperationException("Twilio:AccountSid is missing");
            _authToken = configuration["Twilio:Token"] ?? throw new InvalidOperationException("Twilio:AuthToken is missing");
            _fromPhoneNumber = configuration["Twilio:PhoneNumber"] ?? throw new InvalidOperationException("Twilio:FromPhoneNumber is missing");

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toPhoneNumber),
                    from: new PhoneNumber(_fromPhoneNumber),
                    body: message
                );

                return result.Status != MessageResource.StatusEnum.Failed &&
                       result.Status != MessageResource.StatusEnum.Undelivered;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send SMS via Twilio", ex);
            }
        }
    }
}