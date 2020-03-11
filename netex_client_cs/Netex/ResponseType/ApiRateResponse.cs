using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Netex.ResponseType
{
    public class ApiRateResponse
    {
        public double rate;
        public double balance;

        public ApiRateResponse(NetexRateItemType model)
        {
            rate    = model.sourceAmount;
            balance = model.targetAvailable;
        }

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
