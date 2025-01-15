using Newtonsoft.Json;
using System.Data;
using System.Net;

namespace EasyFly.Web.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _Next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new { error = "An unexpected error occurred." };
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception is DBConcurrencyException)
            {
                response = new { error = "Out of date data" };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            var jsonResponse = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
