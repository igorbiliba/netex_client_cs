using netex_client_cs.Components;
using netex_client_cs.Data;
using netex_client_cs.Helpers;
using netex_client_cs.Models;
using netex_client_cs.Netex;
using netex_client_cs.Netex.ResponseType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using static netex_client_cs.Data.ProxySettings;

namespace netex_client_cs
{
    class Program {
        const int ACTION_ID = 0;

        static string CheckCapcha(ProxyStack proxyStack, NetexSettings settings)
        {
            DB db = new DB();
            List<string> usedProxyList = new List<string>();
            ProxySettingsItem freeProxy = proxyStack.Next(ref usedProxyList);
            Request request = new Request() { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = freeProxy };
            string response = request.HttpGet("exchangeDirection/getAll");

            Console.WriteLine("Used proxy: " + freeProxy.ip);
            Console.WriteLine("");

            //конспирация конспирологам
            return response
                .Replace("Netex24", "bart")
                .Replace("Netex",   "bart")
                .Replace("netex24", "bart")
                .Replace("netex",   "bart")
                .Replace("NETEX24", "bart")
                .Replace("NETEX",   "bart");
        }

        public static bool TEST {
            get
            {
                return File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\istest");
            }
        }

        public const bool WRITE_DEBUG = false;
        static void Main(string[] args) {
            //args = new string[] {
            //    "--create",
            //    "5000",
            //    "+79062532468",
            //    "3DVobv7Pf5TVWdy7fykuSdWscy3kTS4MvV"
            //};
            //args = new string[] { "--gettypebtcaddress" };

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            NetexSettings settings = new NetexSettings();
            settings.CreateIfNotExists();
            settings.LoadSettings();

            string USER_TOKEN = Guid.NewGuid().ToString();

            DB db = new DB();
            StorageModelDB  storageDB   = new StorageModelDB() { db = db }.MigrateUp();
            StorageModelREG storageREG  = new StorageModelREG();

            EmailStorageModelDB emailStorageModel = new EmailStorageModelDB() { storageDB = storageDB };
            EmailStack emailStack                 = new EmailStack() { allowEmails = settings.allowEmails, emailStorageModel = emailStorageModel };
            EmailSender emailSender               = new EmailSender() { settings = settings.email };
            
            //proxy list create
            const string FILE_CREATE_PROXY = "ProxyListForCreate.json";
            ProxyStorageModelMixed createProxyStorageModel = new ProxyStorageModelMixed(settings.registerKeyPrefix, settings.dbKeyPrefix, FILE_CREATE_PROXY) { storageDB = storageDB, storageREG = storageREG };
            ProxySettings createProxySettings         = new ProxySettings(FILE_CREATE_PROXY) { createProxyStorageModel = createProxyStorageModel }.LoadSettings();            
            ProxyStack createProxyStack               = new ProxyStack() { settings = settings, emailSender = emailSender, proxySettings = createProxySettings, proxyStorageModel = createProxyStorageModel };
            
            //proxy list rate
            const string FILE_RATE_PROXY = "ProxyListForRate.json";
            ProxySettings rateProxySettings              = new ProxySettings(FILE_RATE_PROXY).LoadSettings();
            ProxyStorageModelMixed rateProxyStorageModel = new ProxyStorageModelMixed(settings.registerKeyPrefix, settings.dbKeyPrefix, FILE_RATE_PROXY) { storageDB = storageDB, storageREG = storageREG };
            ProxyStack rateProxyStack                    = new ProxyStack() { settings = settings, emailSender = emailSender, proxySettings = rateProxySettings, proxyStorageModel = rateProxyStorageModel };
            
            //proxy list type addr
            const string FILE_TYPEADDR_PROXY = "ProxyListForGetTypeAddr.json";
            ProxySettings typeaddrProxySettings              = new ProxySettings(FILE_TYPEADDR_PROXY).LoadSettings();
            ProxyStorageModelMixed typeaddrProxyStorageModel = new ProxyStorageModelMixed(settings.registerKeyPrefix, settings.dbKeyPrefix, FILE_TYPEADDR_PROXY) { storageDB = storageDB, storageREG = storageREG };            
            ProxyStack typeaddrProxyStack                    = new ProxyStack() { settings = settings, emailSender = emailSender, proxySettings = typeaddrProxySettings, proxyStorageModel = typeaddrProxyStorageModel };
            
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine(CheckCapcha(createProxyStack, settings));

                    Console.WriteLine("\n==========================================\n");
                    Console.WriteLine("Check proxy " + FILE_TYPEADDR_PROXY + ":");
                    CheckAllProxy(typeaddrProxyStack, typeaddrProxySettings, settings);

                    Console.WriteLine("\n==========================================\n");
                    Console.WriteLine("Check proxy "+ FILE_CREATE_PROXY + ":");
                    CheckAllProxy(createProxyStack, createProxySettings, settings);

                    Console.ReadKey();
                    return;
                }

                //первым делом установи доступную валюту
                switch (args[ACTION_ID])
                {
                    case "--rate":
                        Console.WriteLine(Rate(rateProxyStack.First(), settings));
                        break;
                    case "--create":
                        double amount = -1;
                        try
                        { amount = double.Parse(args[1].Replace(',', '.')); }
                        catch (Exception)
                        { amount = double.Parse(args[1].Replace('.', ',')); }

                        string phone = PhoneHelper.PhoneReplacer(args[2]);
                        string btcAddr = args[3];

                        List<string> usedProxyList = new List<string>();
                        int cntTry = settings.maxTryReCreate;
                        do
                        {
                            try
                            {
                                NetexRequestPaymentResponseType response = Create(USER_TOKEN, emailStack, createProxyStack, settings, amount, phone, btcAddr, ref usedProxyList);
                                response.used_proxy_list = usedProxyList.ToArray();
                                Console.WriteLine(response.toJson());

                                if (WRITE_DEBUG) Console.ReadKey();
                                
                                return;
                            }
                            catch (Exception) { }
                        } while (--cntTry > 0);                        
                        break;
                    case "--gettypebtcaddress":
                        if (typeaddrProxyStack.proxySettings.items.Count() == 0)
                        {
                            Console.WriteLine(
                                new GetTypeBtcAddressResponse()
                                {
                                    btc_addresstype = "",
                                    target_currency_id = 106
                                }.toJson()
                            );

                            if (WRITE_DEBUG) Console.ReadKey();
                            return;
                        }

                        Console.WriteLine(GetTypeBtcAddress(typeaddrProxyStack.First(), settings));
                        if (WRITE_DEBUG) Console.ReadKey();
                        break;
                    case "--getallowcurrenciesids":
                        Console.WriteLine(GetAllowCurrenciesIds(typeaddrProxyStack.First(), settings));

                        if (WRITE_DEBUG) Console.ReadKey();
                        break;
                    case "--checkallproxy":
                        CheckAllProxy(createProxyStack, createProxySettings, settings);

                        if (WRITE_DEBUG) Console.ReadKey();
                        break;
                    default:
                        if (args[ACTION_ID].Substring(args[ACTION_ID].Length - 4).Trim().ToLower() == ".txt")
                            AddNexProxy(args[ACTION_ID], createProxySettings);
                        break;
                }
            }
            catch (Exception ex) {
                if (TEST)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void AddNexProxy(string pathFile, ProxySettings proxySettings)
        {
            Console.WriteLine("start load proxy from file: " + pathFile);

            var lines = File.ReadAllLines(pathFile);
            string username = lines[0];
            string password = lines[1];

            List<ProxySettingsItem> newProxyItems = new List<ProxySettingsItem>();

            bool beginRead = false;
            foreach (var line in lines)
            {
                if(!beginRead)
                {
                    beginRead = line.IndexOf("=") != -1;                    
                    continue;
                }

                var scope = line.Split(':');
                if (scope.Length != 2) continue;

                newProxyItems.Add(new ProxySettingsItem()
                {
                    username = username,
                    password = password,
                    ip       =           scope[0].Trim(),                    
                    port     = int.Parse(scope[1].Trim())
                });
            }

            List<ProxySettingsItem> proxySettingsItems = proxySettings
                    .items
                    .ToList();

            int addedCnt = 0;
            foreach (var proxyItem in newProxyItems)
            {
                if (proxySettingsItems.Exists(el => el.ip.Trim().ToLower() == proxyItem.ip.Trim().ToLower()))
                    continue;

                proxySettingsItems.Add(proxyItem);
                addedCnt++;
            }

            proxySettings.items = proxySettingsItems.ToArray();
            proxySettings.SaveItemsToFile();

            Console.WriteLine("Has been added " + addedCnt + " new items");

            Console.ReadKey();
        }

        static void CheckAllProxy(ProxyStack proxyStack, ProxySettings proxySettings, NetexSettings settings)
        {
            List<ProxySettingsItem> bannedItems = new List<ProxySettingsItem>();

            for(int i = 0; i < proxySettings.items.Length; i++)
            {
                List<string> usedProxyList = new List<string>();

                ProxySettingsItem freeProxy = proxyStack.Next(ref usedProxyList, false, false);
                Request request = new Request() { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = freeProxy };
                NetexClient netex = new NetexClient() { request = request, settings = settings };
                
                if(!netex.TryLoadMainPage())
                {
                    bannedItems.Add(freeProxy);
                    Console.WriteLine("banned: " + freeProxy.ip);
                }
                else
                {
                    Console.WriteLine("success: " + freeProxy.ip);
                }
            }

            foreach (var item in bannedItems)
            {
                proxySettings.RemoveFromFile(item);
            }

            Console.WriteLine("===============================");
            Console.WriteLine("Has been removed: " + bannedItems.Count + " banned proxy");
        }

        static string GetAllowCurrenciesIds(ProxySettingsItem proxy, NetexSettings settings)
        {
            List<string> list = new List<string>();
            
            Request           request   = new Request() { BASE_URL = "https://api.netex24.net/", proxy = proxy, settings = settings };
            NetexClient       netex     = new NetexClient() { request = request, settings = settings };

            foreach (var rate in netex.rates)
            {
                if (rate.sourceCurrencyId != settings.sourceCurrencyId) continue;
                if (("|" + settings.targetCurrenciesIds + "|")
                    .Replace(" ", "")
                    .IndexOf(
                        "|" + rate.targetCurrencyId.ToString() + "|"
                    ) == -1) continue;

                list.Add(rate.targetCurrencyId.ToString());
            }

            return String.Join(", ", list);
        }

        static string GetTypeBtcAddress(ProxySettingsItem proxy, NetexSettings settings)
        {
            Request             request     = new Request()     { BASE_URL = "https://api.netex24.net/", proxy = proxy, settings = settings };
            NetexClient         netex       = new NetexClient() { request = request, settings = settings };
            int currencyId = netex.targetCurrencyId;

            try
            {
                BtcAddressTypeByTargetCurrenciesIdItem target = settings
                    .btcAddressTypeByTargetCurrenciesId
                    .Where(
                        el => el.targetCurrencyId == currencyId
                    )
                    .First();

                return new GetTypeBtcAddressResponse()
                {
                    btc_addresstype    = target.btcAddressType,
                    target_currency_id = target.targetCurrencyId
                }.toJson();
            }
            catch (Exception) { }

            return null;
        }

        static string Rate(ProxySettingsItem proxy, NetexSettings settings)
        {
            Request                     request        = new Request()     { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = proxy };
            NetexClient                 netex          = new NetexClient() { request  = request, settings = settings };
            NetexRateItemType           rate           = netex
                .rates
                .Where(
                    el => el.sourceCurrencyId == settings.sourceCurrencyId && el.targetCurrencyId == netex.targetCurrencyId
                )
                .First<NetexRateItemType>();

            return new ApiRateResponse(rate).toJson();
        }
        
        public static NetexRequestPaymentResponseType Create(string userToken, EmailStack emailStack, ProxyStack proxyStack, NetexSettings settings, double amount, string phone, string btcAddr, ref List<string> usedProxyList)
        {
            ProxySettingsItem           usedProxy      = proxyStack.Next(ref usedProxyList);
            Request                     request        = new Request()     { BASE_URL = "https://api.netex24.net/", proxy = usedProxy, settings = settings };
            NetexClient                 netex          = new NetexClient() { request  = request, settings = settings };
            NetexRateItemType           rate           = netex
                .rates
                .Where(
                    el => el.sourceCurrencyId == settings.sourceCurrencyId && el.targetCurrencyId == netex.targetCurrencyId
                )
                .First<NetexRateItemType>();

            double btcAmount = amount / rate.sourceAmount;

            string email = "";
            var directionResponse = netex.ExchangeDirectionGetBy(userToken);
            
            netex.AddressValidator(btcAddr);
            
            NetexCreateResponseType         createResponse = netex.Create(userToken, emailStack, phone, amount, btcAddr, btcAmount, ref email);            
            NetexRequestPaymentResponseType requestPayment = netex.GetRequestPayment(createResponse.transactionId);

            requestPayment.btc_amount = btcAmount;
            requestPayment.ip         = usedProxy == null ? "no proxy" : usedProxy.ip;
            requestPayment.email      = email;

            return requestPayment;
        }
    }
}
