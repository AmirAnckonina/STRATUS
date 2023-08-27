using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Utils.Utils
{
    public static class SessionUtils
    {
        public static string GetSessionId(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.Request.Cookies["Stratus"];
        }
    }
}
