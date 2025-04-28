using System;
using System.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using EasyFly.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace EasyFly.Web.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }


        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
           /* context.Response.Clear();

            object model;
            if (exception is DBConcurrencyException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                model = new ErrorViewModel
                {
                    Title = "Out of date data",
                    Message = "The data you attempted to update is outdated."
                };
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                model = new ErrorViewModel
                {
                    Title = "An unexpected error occurred",
                    Message = "Please try again later."
                };
            }

            string errorHtml = await RenderViewToStringAsync(context, "Error", model);
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(errorHtml);*/
        }

       /* private static async Task<string> RenderViewToStringAsync(HttpContext context, string viewName, object model)
        {
            var serviceProvider = context.RequestServices;
            var razorViewEngine = (IRazorViewEngine)serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)serviceProvider.GetService(typeof(ITempDataProvider));

            var actionContext = new ActionContext(context, context.GetRouteData() ?? new RouteData(), new ActionDescriptor());
            var viewResult = razorViewEngine.FindView(actionContext, viewName, isMainPage: false);

            if (viewResult.View == null)
            {
                throw new InvalidOperationException($"The view '{viewName}' was not found.");
            }

            using (var sw = new StringWriter())
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(context, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }*/
    }
}
