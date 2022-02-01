using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Controllers
{
    public class UsersController : Controller
    {
        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";

        private const string Username = "user";
        private const string Password = "user123";

        public UsersController(Request request)
            : base(request)
        {
        }

        public Response Login() => Html(LoginForm);

        public Response LogInUser()
        {
            Request.Session.Clear();

            var sessionbeforLogin = Request.Session;

            var usernameMatches = Request.Form["Username"] == Username;
            var passwordMatches = Request.Form["Password"] == Password;


            if (usernameMatches && passwordMatches)
            {
                if (!Request.Session.ContainsKey(Session.SessionUserKey))
                {
                    Request.Session[Session.SessionUserKey] = "MyUserId";
                    Request.Cookies.Add(Session.SessionCookieName, Request.Session.Id);

                    return Html($"<h3>Logged successfully!" + "- <a href='/UserProfile'>UserProfile</a></h3>");
                }

                return Html("<h3> Logged successfully!</h3>");
            }

            return Redirect("/Login");
        }

        internal Response GetUserData()
        {
            if (Request.Session.ContainsKey(Session.SessionUserKey))
            {                
                return Html($"<h3>Currently logget-in user" +
                    $"is with username '{Username}'</h3>");
            }

            return Redirect("/Login");
        }

        public Response Logout()
        {
            Request.Session.Clear();

            return Html("<h3>Logged out successfuly!</h3>");
        }
    }
}
