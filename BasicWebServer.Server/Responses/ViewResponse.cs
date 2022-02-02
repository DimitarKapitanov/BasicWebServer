﻿using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class ViewResponse : ContentResponse
    {
        private const char PathSeparator = '/';

        public ViewResponse(string viewName, string controlerName) 
            : base("", ContentType.Html)
        {
            if (!viewName.Contains(PathSeparator))
            {
                viewName = controlerName + PathSeparator + viewName;
            }

            var viewPath = Path.GetFullPath(
                $"./Views/{viewName.TrimStart(PathSeparator)}.cshtml");

            var viewContent = File.ReadAllText(viewPath);

            Body = viewContent;
        }
    }
}