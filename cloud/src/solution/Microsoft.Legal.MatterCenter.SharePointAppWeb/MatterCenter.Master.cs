using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Helpers;

namespace Microsoft.Legal.MatterCenter.SharePointAppWeb
{
    public partial class MatterCenter : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies[UIConstantStrings.URLReferrerCookieName] == null && Request.UrlReferrer != null)
            {
                HttpCookie referrer = new HttpCookie(UIConstantStrings.URLReferrerCookieName, Request.UrlReferrer.Host.ToString());
                Response.Cookies.Add(referrer);
            }

            if (Request.Cookies[UIConstantStrings.RequestTokenCookieName] == null && String.IsNullOrWhiteSpace(Request.Form[this.requestvalidator.Name]))
            {
                Guid newGuid = Guid.NewGuid();
                this.requestvalidator.Value = newGuid.ToString();
                HttpCookie cookie = new HttpCookie(UIConstantStrings.RequestTokenCookieName, newGuid.ToString());
                cookie.HttpOnly = false;
                Response.Cookies.Add(cookie);
            }
            else
            {
                if (Request.Cookies[UIConstantStrings.RequestTokenCookieName] != null)
                {					
                    if (Request.UrlReferrer != null && Request.UrlReferrer.Host == Request.Cookies[UIConstantStrings.URLReferrerCookieName].Value)
                    {
                        this.requestvalidator.Value = Request.Cookies[UIConstantStrings.RequestTokenCookieName].Value;
                    }
                    else if (Request.Url != null
                            && Request.UrlReferrer == null)
                    {
                        this.requestvalidator.Value = Request.Cookies[UIConstantStrings.RequestTokenCookieName].Value;
                    }
                }
            }
        }
    }
}