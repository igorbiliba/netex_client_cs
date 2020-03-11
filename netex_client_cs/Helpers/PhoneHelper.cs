using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Helpers
{
    public class PhoneHelper
    {
        public static string PhoneReplacer(string phone)
            => "+" + (phone
                            .Replace(" ", String.Empty)
                            .Replace("+", String.Empty)
                            .Replace("-", String.Empty));
    }
}
