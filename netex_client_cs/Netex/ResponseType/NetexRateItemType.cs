using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Netex.ResponseType
{
    public class NetexRateItemType
    {
        public int    sourceCurrencyId;
        public int    sourceCustomerCurrencyId;
        public int    sourceWorldCurrencyId;
        public double sourceAmount;
        public int    targetCurrencyId;
        public int    targetWorldCurrencyId;
        public int    targetCustomerCurrencyId;
        public double targetAmount;
        public double targetAvailable;
        public bool   isDisabled;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
