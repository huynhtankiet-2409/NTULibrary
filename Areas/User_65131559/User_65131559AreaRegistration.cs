using System.Web.Mvc;

namespace ProjectCK_65131559.Areas.User_65131559
{
    public class User_65131559AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "User_65131559";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "User_65131559_default",
                "User_65131559/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "ProjectCK_65131559.Areas.User_65131559.Controllers" }
            );
        }
    }
}