using System.Web;
using System.Web.Mvc;

namespace ProjectCK_65131559
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
