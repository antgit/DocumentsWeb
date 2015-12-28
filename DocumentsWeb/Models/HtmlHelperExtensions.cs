using System.Linq.Expressions;
using System.Linq;

namespace System.Web.Mvc.Html
{
    /// <summary> 
    /// Extends the HtmlHelper 
    /// </summary> 
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ActionLink(this HtmlHelper html, string linkText, string actionName, string[] role)
        {
            return role.Any(r => html.ViewContext.RequestContext.HttpContext.User.IsInRole(r))
               ? html.ActionLink(linkText, actionName, new object())
               : MvcHtmlString.Empty;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper html, string linkText, string actionName, string controller, string[] role)
        {
            return role.Any(r => html.ViewContext.RequestContext.HttpContext.User.IsInRole(r))
               ? html.ActionLink(linkText, actionName, controller)
               : MvcHtmlString.Empty;

        }

        public static MvcHtmlString ActionLink(this HtmlHelper html, string linkText, string actionName, string[] role, object routeValues)
        {
            return role.Any(r => html.ViewContext.RequestContext.HttpContext.User.IsInRole(r))
               ? html.ActionLink(linkText, actionName, routeValues)
               : MvcHtmlString.Empty;
        }
    }
}