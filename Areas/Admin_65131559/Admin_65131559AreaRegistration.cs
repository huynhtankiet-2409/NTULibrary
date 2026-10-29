using System.Web.Mvc;

namespace ProjectCK_65131559.Areas.Admin_65131559
{
    public class Admin_65131559AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_65131559";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_65131559_default",
                "Admin_65131559/{controller}/{action}/{id}",
                new { controller = "AdminHome_65131559", action = "Index", id = UrlParameter.Optional },
                new[] { "ProjectCK_65131559.Areas.Admin_65131559.Controllers" }
            );
        }
    }
}