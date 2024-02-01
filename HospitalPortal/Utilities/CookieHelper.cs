using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Utility
{
    public class CookieHelper
    {
       public static void CreateCookie(string name,int minutes,string text)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = StringCipher.Encrypt(text, "ivar123");
            cookie.Expires = DateTime.Now.AddHours(minutes);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(string name)
        {
            var cookie = HttpContext.Current.Request.Cookies[name];
            return cookie == null ? "" : StringCipher.Decrypt(cookie.Value, "ivar123");
        }

        public static bool RemoveCookie(string name)
        {
            try
            {
                var cookie = GetCookie(name);
                if (cookie != null)
                {
                    var c = new HttpCookie(name);
                    c.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(c);
                }
                return true;
            }
            catch(Exception ex)
            {
                string msg = ex.ToString();
                return false;
            }
        }
    }
}