using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using DocumentsWeb.Controllers;

namespace DocumentsWeb
{
    public class MultiAuthorizeAttribute : AuthorizeAttribute
    {
        public new string[] Roles
        {
            get { return base.Roles.Split(','); }
            set { base.Roles = string.Join(",", value); }
        }
        public new string[] Users
        {
            get { return base.Users.Split(','); }
            set { base.Users = string.Join(",", value); }
        }
    }

    //public class OrAuthorizationAttribute : AuthorizeAttribute
    //{

    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {

    //        return httpContext.Request.IsLocal || base.AuthorizeCore(httpContext);
    //    }
    //} 

    //public class CustomAuthAttribute : AuthorizeAttribute
    //{
    //    //private string[] allowedUsers;

    //    public CustomAuthAttribute(params string[] users)
    //    {
    //        //allowedUsers = users;
    //    }

    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {

    //        //return httpContext.Request.IsAuthenticated &&
    //        //       allowedUsers.Contains(httpContext.User.Identity.Name,
    //        //                             StringComparer.InvariantCultureIgnoreCase);
    //    }
    //}
/*
 * 
 public GenesisController : Controller 
{ 
    [CheckLoggedIn(gr = genesisRepository, memberGuid = md.memberGUID)] 
    public ActionResult Home(MemberData md) 
    { 
        return View(md); 
    } 
} 

    //public class CheckLoggedIn : ActionFilterAttribute
    //{
    //    public IGenesisRepository gr { get; set; }
    //    public Guid memberGuid { get; set; }

    //    public override void OnActionExecuting(ActionExecutingContext filterContext) 
    //{ 
    //    /* how to get the controller*/ 
    //    var controllerUsingThisAttribute = ((GenesisController)filterContext.Controller); 
 
    //    /* now you can use the public properties from the controller */ 
    //    gr = controllerUsingThisAttribute .genesisRepository; 
    //    memberGuid = (controllerUsingThisAttribute .memberGuid; 
 
    //    Member thisMember = gr.GetActiveMember(memberGuid); 
    //    Member bottomMember = gr.GetMemberOnBottom(); 
 
    //    if (thisMember.Role.Tier <= bottomMember.Role.Tier) 
    //    { 
    //        filterContext 
    //            .HttpContext 
    //            .Response 
    //            .RedirectToRoute(new { controller = "Member", action = "Login" }); 
    //    } 
 
    //    base.OnActionExecuting(filterContext); 
    //}
    //} 

    //public class CheckMyCompanyId : ActionFilterAttribute
    //{
    //    public BaseController gr { get; set; }
    //    public Guid memberGuid { get; set; }

    //    public override void OnActionExecuting(ActionExecutingContext filterContext) 
    //{ 
    //    /* how to get the controller*/ 
    //    var controllerUsingThisAttribute = ((BaseController)filterContext.Controller); 

    //    /* now you can use the public properties from the controller */ 
    //    gr = controllerUsingThisAttribute.genesisRepository; 
    //    memberGuid = (controllerUsingThisAttribute.memberGuid; 

    //    Member thisMember = gr.GetActiveMember(memberGuid); 
    //    Member bottomMember = gr.GetMemberOnBottom(); 

    //    if (thisMember.Role.Tier <= bottomMember.Role.Tier) 
    //    { 
    //        filterContext 
    //            .HttpContext 
    //            .Response 
    //            .RedirectToRoute(new { controller = "Member", action = "Login" }); 
    //    } 

    //    base.OnActionExecuting(filterContext); 
    //}
    //} 

}