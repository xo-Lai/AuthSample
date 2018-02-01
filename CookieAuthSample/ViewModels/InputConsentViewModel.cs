using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieAuthSample.ViewModels
{
    public class InputConsentViewModel
    {
        public string Button { get; set; }
        public IEnumerable<string> ScopeConsented { get; set; }

        public bool RememberConsent { get; set; }

        public string ReturnUrl { get; set; }
    }
}
