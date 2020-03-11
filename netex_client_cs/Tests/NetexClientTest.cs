using netex_client_cs.Components;
using netex_client_cs.Data;
using netex_client_cs.Models;
using netex_client_cs.Netex;
using netex_client_cs.Netex.ResponseType;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static netex_client_cs.Data.ProxySettings;

namespace netex_client_cs.Tests
{
    [TestFixture]
    class NetexClientTest
    {
        NetexSettings settings;
        NetexClient netex;

        [SetUp]
        public void NetexClientTestInit()
        {
            //settings = new NetexSettings();
            //DB db = new DB();
            //UsedProxyModel usedProxyModel = new UsedProxyModel() {
            //    db       = db,
            //    settings = settings
            //};

            //ProxySettingsItem freeProxy =
            //    settings.proxy.items.Length == 0 ?
            //    null :
            //    usedProxyModel.FindFreeProxy(
            //        new ProxyLog()
            //            .Load()
            //            .GetBlacklistHosts()
            //    );
            //Request request = new Request() { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = freeProxy };
            //netex = new NetexClient() { request = request, settings = settings };
        }

        [Test, Order(3)]
        public void GetRateTest()
        {
            var rates = netex.rates;
            Assert.IsTrue(rates.Length > 0, "Курсы не парсятся");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var conditionList = rates
                .Where(
                    el => el.sourceCurrencyId == settings.sourceCurrencyId
                );
            Assert.IsTrue(conditionList.Count<NetexRateItemType>() > 0, "Source валюты " + settings.sourceCurrencyId + " не существует ");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            conditionList = rates
                .Where(
                    el => el.targetCurrencyId == netex.targetCurrencyId
                );
            Assert.IsTrue(conditionList.Count<NetexRateItemType>() > 0, "Target валюты " + netex.targetCurrencyId + " не существует ");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            conditionList = rates
                .Where(
                    el => el.sourceCurrencyId == settings.sourceCurrencyId && el.targetCurrencyId == netex.targetCurrencyId
                );

            Assert.IsTrue(conditionList.Count<NetexRateItemType>() > 0, "Не нашел курс " + settings.sourceCurrencyId + " -> " + netex.targetCurrencyId);
            NetexRateItemType rate = conditionList.First<NetexRateItemType>();
            Assert.IsTrue(rate.sourceCurrencyId == settings.sourceCurrencyId, "Не удалось получить NetexRateItemResponseType при курсе " + settings.sourceCurrencyId + " -> " + netex.targetCurrencyId);
        }

        [Test, Order(2)]
        public void ChangeTargetCurrencyIdTest()
        {
            Assert.IsTrue(netex.targetCurrencyId != -1, "Не устанавливается автоматом targetCurrencyId ("+ netex.targetCurrencyId + ")");
        }

        [Test, Order(1)]
        public void CreateTest()
        {
            string[] addressList = {
                "bc1q9j2xm7mwsdj23tclgkgqzh8kezpehcfxrqkx86",
                "bc1qvuleu4x57p3zd9rqnch93q75nqan6jtpdmg6g7",
                "bc1qr03t92k33pnt3ccaxt5t3cuvga5vtyzadmup5t",
                "bc1q89zay58r2zfnszyq8vf0eqy7e9h5x4sd3af4e9",
                "bc1qa72exsgsfmh0x6nmjk0v49436xcfuun5mvj4a7"
            };

            string address = addressList[
                new Random().Next(0, addressList.Length - 1)
            ];

            string[] phoneList = {
                "+79060671230",
                "+79060671231",
                "+79060671232",
                "+79060671233",
                "+79060671234"
            };

            string phone = phoneList[
                new Random().Next(0, phoneList.Length - 1)
            ];

            ProxySettingsItem usedProxyInCreate = null;

            //NetexRequestPaymentResponseType response = Program.Create(6000, phone, address, ref usedProxyInCreate);

            //Assert.IsTrue(response.IsValid());
        }
    }
}
