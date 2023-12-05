using System.Web;
using System.Web.Mvc;

namespace Brightflow_mvc_opdracht {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
