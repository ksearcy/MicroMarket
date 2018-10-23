using System.Web;
using System.Web.Mvc;
using deOROWeb.Security;

namespace deOROWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            //filters.Add(new deOROSessionTimeout());
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}