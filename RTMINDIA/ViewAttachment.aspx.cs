﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;

public partial class ViewAttachment : System.Web.UI.Page
{
    clsTicketing objTicket = new clsTicketing();
    DataTable dt = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Context.Session != null)
        {
            if (Session.IsNewSession)
            {
                HttpCookie newSessionIdCookie = Request.Cookies["ASP.NET_SessionId"];
                if (newSessionIdCookie != null)
                {
                    string newSessionIdCookieValue = newSessionIdCookie.Value;
                    if (newSessionIdCookieValue != string.Empty)
                    {
                        if (!Page.IsCallback)
                        {
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            Response.RedirectLocation = Page.ResolveUrl("~/Default.aspx");
                        }
                    }
                }
            }
        }
        if (!IsPostBack)
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);

            dt = objTicket.DownloadFile(Convert.ToInt32(id));
            if (dt.Rows.Count > 0)
            {
                download(dt);
            }
        }
    }

        private void download(DataTable dt)
        {
            Byte[] bytes = (Byte[])dt.Rows[0]["I_Data"];
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = dt.Rows[0]["I_ContentType"].ToString();
            Response.AddHeader("content-disposition", "inline;filename="
            + dt.Rows[0]["I_Name"].ToString());
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
    }