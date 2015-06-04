using System.Web;
using System.Web.Mvc;

namespace Chronozoom.AdlibImporter.Backend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
