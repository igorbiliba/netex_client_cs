using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace netex_client_cs.Netex.ResponseType
{
    public class NetexRequestPaymentResponseType
    {
        public string account;              //extra['account']
        public string comment;              //extra['comment']
        public double btc_amount;           //
        public string ip;                   //
        public string email;                //
        public string[] used_proxy_list;    //

        public NetexRequestPaymentResponseType(string requestPaymentXml)
        {
            requestPaymentXml = requestPaymentXml.Replace("\\r", "");
            requestPaymentXml = requestPaymentXml.Replace("\\n", "");
            requestPaymentXml = requestPaymentXml.Replace("\\", "");
            requestPaymentXml = requestPaymentXml.Replace("\"", "");
            requestPaymentXml = requestPaymentXml.Replace("'", "");
            requestPaymentXml = requestPaymentXml.Replace("=", "");
            requestPaymentXml = requestPaymentXml.Replace("type", "");
            requestPaymentXml = requestPaymentXml.Replace("input", "");
            requestPaymentXml = requestPaymentXml.Replace("hidden", "");
            requestPaymentXml = requestPaymentXml.Replace("value", "");
            requestPaymentXml = requestPaymentXml.Replace("/", "");
            requestPaymentXml = requestPaymentXml.Replace("name", "");
            requestPaymentXml = requestPaymentXml.Replace(">", "");


            foreach (var row in requestPaymentXml.Split('<'))
            {
                if (row.IndexOf("extra[account]") != -1)
                    this.account = row.Replace("extra[account]", "").Replace(" ", "");

                if (row.IndexOf("extra[comment]") != -1)
                    this.comment = row.Replace("extra[comment]", "").Replace(" ", "");
            }
        }
        
        public bool IsValid()
        {
            if (account.Trim().Length == 0) return false;
            if (comment.Trim().Length == 0) return false;
            if (btc_amount <= 0)            return false;

            return true;
        }

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
