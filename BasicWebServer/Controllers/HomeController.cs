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
using BasicWebServer.Models;

namespace BasicWebServer.Controllers
{
    public class HomeController : Controller
    {
        private const string HtmlForm = @"";

        private const string DownloadForm = @"";

        private const string FileName = "test.pdf";

        public HomeController(Request request)
            : base(request)
        {

        }

        public Response Index() => Text($"Hello from server {DateTime.Now}");
        public Response Redirect() => Redirect("https://softuni.org");
        public Response Html() => View();


        public Response HtmlFormPost()
        {
            string name = Request.Form["Name"];
            string age = Request.Form["Age"];

            var model = new FormViewModel()
            {
                Name = name,
                Age = int.Parse(age)
            };

            return View(model);
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

        public Response Content() => View();

        public Response DownloadContent() => File(FileName);



    }
}
