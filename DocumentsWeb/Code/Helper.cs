using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using DocumentsWeb.Code;

namespace DocumentWeb.Extentions
{
    public static class Helper
    {
        public static MvcHtmlString ImageActionLink(
            this HtmlHelper helper,
            string imageUrl,
            string altText,
            string actionName,
            string controllerName,
            object routeValues,
            object linkHtmlAttributes,
            object imgHtmlAttributes)
        {
            var linkAttributes = AnonymousObjectToKeyValue(linkHtmlAttributes);
            var imgAttributes = AnonymousObjectToKeyValue(imgHtmlAttributes);
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.MergeAttributes(imgAttributes, true);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routeValues));
            linkBuilder.MergeAttributes(linkAttributes, true);
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }
        private static Dictionary<string, object> AnonymousObjectToKeyValue(object anonymousObject)
        {
            var dictionary = new Dictionary<string, object>();
            if (anonymousObject != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
                {
                    dictionary.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(anonymousObject));
                }
            }
            return dictionary;
        }


        public static bool IsRouteMatch(this Uri uri, string controllerName, string actionName)
        {
            RouteInfo routeInfo = new RouteInfo(uri, HttpContext.Current.Request.ApplicationPath);
            return (routeInfo.RouteData.Values["controller"].ToString() == controllerName && routeInfo.RouteData.Values["action"].ToString() == actionName);
        }

        public static string GetRouteParameterValue(this Uri uri, string paramaterName)
        {
            RouteInfo routeInfo = new RouteInfo(uri, HttpContext.Current.Request.ApplicationPath);
            return routeInfo.RouteData.Values[paramaterName] != null ? routeInfo.RouteData.Values[paramaterName].ToString() : null;
        }


        /// <summary>
        /// Плучение значения параметра, передаваемого в PerfofmCallback
        /// </summary>
        /// <param name="request">Объект Request</param>
        /// <param name="argName">Имя параметра</param>
        /// <returns></returns>
        public static string GetCallbackArgument(HttpRequestBase request, string argName)
        {
            try
            {
                string args = request.Params["DXCallbackArgument"];
                int i = args.IndexOf(argName);
                int start = args.IndexOf('=', i) + 1;
                int end = args.IndexOf(';', start) - 1;
                string res = args.Substring(start, end - start + 1);
                return res;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}