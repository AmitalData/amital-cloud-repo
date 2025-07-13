using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace AmitalCloud.Infrastructure.Web.Middlewares
{

	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception occurred at {Time}. ExceptionType: {ExceptionType}", DateTime.UtcNow, ex.GetType().Name);

				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var statusCode = exception switch
			{
				ArgumentNullException => StatusCodes.Status400BadRequest,
				ArgumentException => StatusCodes.Status400BadRequest,
				FormatException => StatusCodes.Status400BadRequest,
				JsonException => StatusCodes.Status400BadRequest,

				UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
				System.Security.SecurityException => StatusCodes.Status403Forbidden,

				KeyNotFoundException => StatusCodes.Status404NotFound,
				FileNotFoundException => StatusCodes.Status404NotFound,

				NotSupportedException => StatusCodes.Status405MethodNotAllowed,

				TimeoutException => StatusCodes.Status408RequestTimeout,
				TaskCanceledException => StatusCodes.Status408RequestTimeout,

				InvalidOperationException => StatusCodes.Status409Conflict,

				NotImplementedException => StatusCodes.Status501NotImplemented,

				HttpRequestException => StatusCodes.Status503ServiceUnavailable,

				_ => StatusCodes.Status500InternalServerError
			};

			var response = new
			{
				status = statusCode,
				error = GetErrorMessage(exception),
				exceptionType = exception.GetType().Name,
				stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
			};

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = _env.IsDevelopment()
			});

			await context.Response.WriteAsync(json);
		}

		private string GetErrorMessage(Exception exception) => exception switch
		{
			ArgumentNullException or ArgumentException => "Invalid input provided.",
			FormatException => "Input format is invalid.",
			JsonException => "Malformed JSON request.",
			UnauthorizedAccessException => "Unauthorized access.",
			System.Security.SecurityException => "Forbidden access.",
			KeyNotFoundException or FileNotFoundException => "Resource not found.",
			NotSupportedException => "HTTP method not allowed.",
			TimeoutException or TaskCanceledException => "Request timed out.",
			InvalidOperationException => "Conflict detected.",
			NotImplementedException => "Feature not implemented.",
			HttpRequestException => "Service unavailable.",
			_ => "An unexpected error occurred."
		};
	}
}