using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Helpers;

public static class ViewHelper
{
    
        // This is an extension method for the IHtmlHelper
        public static string IsActive(this IHtmlHelper html, string controller, string action = "Index")
        {
            var routeData = html.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();

            if (string.Equals(controller, currentController, System.StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action, currentAction, System.StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }
            return "";
        }
        
        // A helper for parent menus (checks only the controller)
        public static string IsMenuActive(this IHtmlHelper html, string controller)
        {
            var routeData = html.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();

            if (string.Equals(controller, currentController, System.StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }
            return "";
        }

        // A helper for parent menus with multiple controllers
        public static string IsMenuActive(this IHtmlHelper html, string[] controllers)
        {
            var routeData = html.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();

            if (controllers.Contains(currentController, System.StringComparer.OrdinalIgnoreCase))
            {
                return "menu-open";
            }
            return "";
        }
    
}