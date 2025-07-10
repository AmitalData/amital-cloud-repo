using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Web.Middlewares
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<LoggingMiddleware> _logger;

		public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			context.Request.EnableBuffering();

			var requestBody = await ReadStreamAsync(context.Request.Body);
			context.Request.Body.Position = 0;

			_logger.LogInformation("HTTP Request Information:\nMethod: {method}\nURL {url} \nHeaders: {headers} \nBody: {body}",
				context.Request.Method,
				context.Request.Path + context.Request.QueryString,
				context.Request.Headers,
				requestBody);


			var originalBodyStream = context.Response.Body;

			using var responseBody = new MemoryStream();
			context.Response.Body = responseBody;

			await _next(context);

			var responseBodyText = await ReadStreamAsync(context.Response.Body);

			_logger.LogInformation("HTTP Response Information:\nStatusCode: {statusCode} \nHeaders: {headers} \nBody: {body}",
				context.Response.StatusCode,
				context.Response.Headers,
				responseBodyText);

			await responseBody.CopyToAsync(originalBodyStream);
		}

		private async Task<string> ReadStreamAsync(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
			var text = await reader.ReadToEndAsync();

			stream.Seek(0, SeekOrigin.Begin);

			return text;
		}
	}

}
