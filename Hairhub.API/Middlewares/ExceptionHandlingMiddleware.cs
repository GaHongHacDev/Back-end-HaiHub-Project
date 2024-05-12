using System.Net;
using System.Text.Json;

namespace Hairhub.API.Middlewares
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string Error { get; set; }

        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {// uuid khac string thi phai cai ong add no dai hon 1 ki tu 
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse() { TimeStamp = DateTime.UtcNow, Error = exception.Message };
            switch (exception)
            {
                //add more custom exception
                //For example case AppException: do something
                case BadHttpRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogInformation(exception.Message);
                    break;
                default:
                    //unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception.ToString());
                    break;
            }
            var result = errorResponse.ToString();
            await context.Response.WriteAsync(result);
        }
    }
}
