<%@ WebHandler Language="C#" Class="KeepSessionAlive" %>

using System;
using System.Web;
using System.Web.SessionState;

public class KeepSessionAlive : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        context.Response.Cache.SetNoStore();
        context.Response.Cache.SetNoServerCaching();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}