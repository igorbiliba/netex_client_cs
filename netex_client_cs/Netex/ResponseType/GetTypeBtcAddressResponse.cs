using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Netex.ResponseType
{
    public class GetTypeBtcAddressResponse
    {
        public string btc_addresstype;
        public int target_currency_id;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
