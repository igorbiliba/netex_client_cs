using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static netex_client_cs.Components.EmailSender;

namespace netex_client_cs.Data
{
    public class BtcAddressTypeByTargetCurrenciesIdItem
    {
        public int    targetCurrencyId;
        public string btcAddressType;
    }
    public class NetexSettings
    {
        string FILE {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Settings.json"; }
        }

        public int                                      expireMinOneIp              = -1;
        public int                                      maxTryReCreate              = -1;
        public int                                      sourceCurrencyId            = -1;
        public int                                      maxHoursTestPeriodProxy     = -1;
        public string                                   targetCurrenciesIds         = "";
        public string                                   registerKeyPrefix           = "";
        public string                                   dbKeyPrefix                 = "";
        public BtcAddressTypeByTargetCurrenciesIdItem[] btcAddressTypeByTargetCurrenciesId;
        public string[]                                 allowEmails;
        public EmailSettingsItem                        email;        

        public void CreateIfNotExists() {
            if (File.Exists(FILE))
                return;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FILE, true)) {
                file.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this)); }
        }

        public void LoadSettings() {
            string jsonString = System.IO.File.ReadAllText(FILE);
            NetexSettings data = Newtonsoft.Json.JsonConvert.DeserializeObject<NetexSettings>(jsonString);

            this.expireMinOneIp                     = data.expireMinOneIp;
            this.maxTryReCreate                     = data.maxTryReCreate;
            this.sourceCurrencyId                   = data.sourceCurrencyId;
            this.targetCurrenciesIds                = data.targetCurrenciesIds;
            this.maxHoursTestPeriodProxy            = data.maxHoursTestPeriodProxy;
            this.btcAddressTypeByTargetCurrenciesId = data.btcAddressTypeByTargetCurrenciesId;
            this.allowEmails                        = data.allowEmails;
            this.email                              = data.email;
            this.registerKeyPrefix                  = data.registerKeyPrefix;
            this.dbKeyPrefix                        = data.dbKeyPrefix;
        }
    }
}
