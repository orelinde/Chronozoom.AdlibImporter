using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace Chronozoom.AdlibImporter.Backend.BatchProcessor
{
    public class WebClientInfinite : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            webRequest.Timeout = Timeout.Infinite;
            return webRequest;
        }
    }
}