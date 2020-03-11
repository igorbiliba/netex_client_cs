using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Netex.ResponseType
{
    public class ErrorResponse
    {
        public string error;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
