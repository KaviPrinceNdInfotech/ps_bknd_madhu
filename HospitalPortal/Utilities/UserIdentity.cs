using HospitalPortal.Utility;
using System;
using System.Web;


namespace HospitalPortal.Utility
{
    public class UserIdentity
    {
        public static string UserRole
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["role"] != null)
                {
                    string cookieVal = HttpContext.Current.Request.Cookies["role"].Value;
                    string role = StringCipher.Decrypt(cookieVal, "rd");
                    return role;
                }

                return "";
            }
            set
            {
                string encRole = StringCipher.Encrypt(value, "rd");

                var roleCookie = new HttpCookie("role");
                roleCookie.Value = encRole;
                //roleCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.Cookies.Add(roleCookie);
            }
        }

        public static string UserName
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["userName"] != null)
                {
                    string cookieVal = HttpContext.Current.Request.Cookies["userName"].Value;
                    string val = StringCipher.Decrypt(cookieVal, "rd");
                    return val;
                }

                return "";
            }
            set
            {
                string encRole = StringCipher.Encrypt(value, "rd");

                var userNameCookie = new HttpCookie("userName");
                userNameCookie.Value = encRole;
                userNameCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.Cookies.Add(userNameCookie);

            }
        }

        public static void UnAssignUserRole()
        {
            if (HttpContext.Current.Request.Cookies["role"] != null)
            {
                var roleCookie = new HttpCookie("role");
                roleCookie.Value = "";
                roleCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(roleCookie);
            }

            if (HttpContext.Current.Request.Cookies["userName"] != null)
            {
                var roleCookie = new HttpCookie("userName");
                roleCookie.Value = "";
                roleCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(roleCookie);
            }

        }

        

    }
}