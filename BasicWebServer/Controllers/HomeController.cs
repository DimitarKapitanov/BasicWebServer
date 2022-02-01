using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Controllers
{
    public class HomeController : Controller
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
   Name: <input type='text' name='Name'/>
   Age: <input type='number' name ='Age'/>
<input type='submit' value ='Save' />
</form>";

        private const string DownloadForm = @"<form action='/Content' method='POST'>
   <input type='submit' value ='Download Sites Content' /> 
</form>";

        private const string FileName = "test.pdf";

        public HomeController(Request request)
            : base(request)
        {

        }

        public Response Index() => Text($"Hello from server {DateTime.Now}");
        public Response Redirect() => Redirect("https://softuni.org");
        public Response Html() => Html(HtmlForm);


        public Response HtmlFormPost()
        {
            var formData = new StringBuilder();

            foreach (var (key, value) in this.Request.Form)
            {
                formData.AppendLine($"{key} - {value}");
            }

            return Text(formData.ToString());
        }

        public Response Session()
        {
            var currentDateKey = Server.HTTP.Session.SessionCurrentDateKey;

            var sessionExist = Request.Session.ContainsKey(currentDateKey);

            if (sessionExist)
            {
                var currentDate = Request.Session[currentDateKey];

                return Text($"Stored date: {currentDate}");
            }

            return Text("Current date stored!");
        }

        public Response Cookies()
        {
            var requestHasCookies = Request.Cookies
                 .Any(c => c.Name != Server.HTTP.Session.SessionCookieName);

            var bodyText = string.Empty;

            if (requestHasCookies)
            {
                var cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th><tr>");

                foreach (var cookie in Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");

                    cookieText.Append("<tr>");
                }

                cookieText.Append("</table>");

                bodyText = cookieText.ToString();

                return Html(bodyText);
            }
            else
            {
                bodyText = "<h1>Cookies set!</h1>";
            }

            var cookies = new CookieCollection();

            if (!requestHasCookies)
            {
                cookies.Add("My-Cookie", "My-Value");
                cookies.Add("My-Second-Cookie", "My-Second-Value");
            }

            return Html(bodyText, cookies);
        }

        public Response Content() => Html(DownloadForm);

        public Response DownloadContent() => File(FileName);



    }
}
