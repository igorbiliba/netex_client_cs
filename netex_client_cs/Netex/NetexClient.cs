using Flurl.Http;
using netex_client_cs.Components;
using netex_client_cs.Data;
using netex_client_cs.Netex.ResponseType;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Netex
{
    public class NetexClient {
        public Request request;
        public NetexSettings settings;

        int _targetCurrencyId = -1;
        public int targetCurrencyId
        {
            get //Установим валюту, что есть в курсах
            {
                if (_targetCurrencyId != -1) return _targetCurrencyId;

                foreach (var rate in rates)
                {
                    if (rate.sourceCurrencyId != settings.sourceCurrencyId) continue;
                    if (("|" + settings.targetCurrenciesIds + "|")
                        .Replace(" ", "")
                        .IndexOf(
                            "|" + rate.targetCurrencyId.ToString() + "|"
                        ) == -1) continue;
                    
                    _targetCurrencyId = rate.targetCurrencyId;
                    return _targetCurrencyId;
                }

                return _targetCurrencyId;
            }
        }

        public string ExchangeDirectionGetBy(string userToken)
        {
            try
            {
                var url = "exchangeDirection/getBy?source=" + settings.sourceCurrencyId.ToString() + "&target=" + targetCurrencyId.ToString() + "&userToken=" + userToken;

                return request.HttpGet( url, 3000 );                
            }
            catch (Exception e) {}

            return null;
        }

        public string AddressValidator(string address)
        {
            try
            {
                var url = "bitflow/addressValidation?currencyId="+ targetCurrencyId.ToString() + "&address="+address;
                return request.HttpGet(url, 3000);
            }
            catch (Exception) { }

            return null;
        }

        public bool TryLoadMainPage()
        {
            try
            {
                var content = request.HttpGet("exchangeDirection/getAll", 3000);
                return content.IndexOf("[{\"") == 0;
            }
            catch (Exception) { }

            return false;
        }

        NetexRateItemType[] _rates = null;
        public NetexRateItemType[] rates
        {
            get
            {
                if (_rates == null)
                {
                    _rates =
                        JsonConvert.DeserializeObject<NetexRateItemType[]>(
                            request.HttpGet("exchangeDirection/getAll"));
                }

                return _rates;
            }
        }

        public string GenerateEmail(EmailStack emailStack, string phone) =>
            new String(
                phone
                    .Where(Char.IsDigit)
                    .ToArray()
            ) + emailStack.Next();

        public NetexCreateResponseType Create(string userToken, EmailStack emailStack, string phone, double amount, string btcAddr, double targetAmount, ref string email)
        {
            email = GenerateEmail(emailStack, phone);
            
            string template = @"{ 
                eosMemo: null,
                partnerId: null,
                rippleDestinationTag: null,
                sourceAmount: '{sourceAmount}',
                sourceAmountWithCustomerFee: {sourceAmountWithCustomerFee},
                sourceCurrencyId: {sourceCurrencyId},
                sourceWallet: '{sourceWallet}',
                targetAmount: null,
                targetCurrencyId: {targetCurrencyId},
                targetWallet: '{targetWallet}',
                userEmail: '{userEmail}',
                userPhone: '{userPhone}',
                userToken: '{userToken}'
            }";

            template = template.Replace("'"                             , "\""                                );
            template = template.Replace("{sourceAmount}"                , amount.ToString().Replace(',', '.') );
            template = template.Replace("{sourceAmountWithCustomerFee}" , amount.ToString().Replace(',', '.') );
            template = template.Replace("{sourceCurrencyId}"            , settings.sourceCurrencyId.ToString());
            template = template.Replace("{sourceWallet}"                , phone                               );
            template = template.Replace("{targetCurrencyId}"            , targetCurrencyId.ToString()         );
            template = template.Replace("{targetWallet}"                , btcAddr                             );
            template = template.Replace("{userEmail}"                   , email                               );
            template = template.Replace("{userPhone}"                   , phone                               );
            template = template.Replace("{userToken}"                   , userToken                           );

            string response = request.HttpJson("transaction/create", template);

            if (Program.WRITE_DEBUG)
            {
                Console.WriteLine(response);
                Console.ReadKey();
            }

            return JsonConvert.DeserializeObject<NetexCreateResponseType>(response);
        }

        public NetexRequestPaymentResponseType GetRequestPayment(string transactionId) {
            string parameters = "id=" + transactionId;
            string response = request.HttpPut("transaction/requestPayment?" + parameters, parameters);

            if (Program.WRITE_DEBUG)
            {
                Console.WriteLine(response);
                Console.ReadKey();
            }

            return new NetexRequestPaymentResponseType(response);
        }
    }
}
