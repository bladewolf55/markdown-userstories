using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownUserStories.Controllers
{
    public static class ControllerHelpers
    {
        public static string GetModelStateErrorsAsHtml(this ModelStateDictionary modelState)
        {
            StringBuilder html = new StringBuilder();
            var keys = modelState.Select(a => new {Property = a.Key, a.Value.Errors });
            foreach(var key in keys)
            {
                html.AppendLine($"{key.Property} has the following error(s):<br /><ul>");
                key.Errors.ToList().ForEach(error => html.AppendLine($"<li>{error.ErrorMessage}</li>"));
                html.AppendLine("</ul><br /><br />");
            }
            return html.ToString();
        }
    }
}
