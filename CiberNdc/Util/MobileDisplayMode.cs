using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace CiberNdc.Util
{
    public class MobileDisplayMode : DefaultDisplayMode
    {
        private readonly StringCollection _useragenStringPartialIdentifiers = new StringCollection
    {
        "Android",
        "Mobile",
        "Opera Mobi",
        "Samsung",
        "HTC",
        "Nokia",
        "Ericsson",
        "SonyEricsson",
        "iPhone",
        "Labs" //For å teste i Opera Labs
    };

        public MobileDisplayMode()
            : base("Mobile")
        {
            ContextCondition = (context => IsMobile(context.GetOverriddenUserAgent()));
        }

        private bool IsMobile(string useragentString)
        {
            return _useragenStringPartialIdentifiers.Cast<string>()
                        .Any(val => useragentString.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }
}