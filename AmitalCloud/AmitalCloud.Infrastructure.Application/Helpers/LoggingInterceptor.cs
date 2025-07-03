using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;


namespace AmitalCloud.Infrastructure.Application.Helpers
{
	public class LoggingInterceptor : ILogging
	{
		private readonly ILogging _logging;

		public LoggingInterceptor(ILogging logging)
		{
			_logging = logging;
		}

		public void LogWarning(string message)
		{
			Console.WriteLine("Before executing LogWarning");
			_logging.LogWarning(message);
			Console.WriteLine("After executing LogWarning");
		}
		public void LogDebug(string message)
		{
			Console.WriteLine("Before executing LogDebug");
			_logging.LogDebug(message);
			Console.WriteLine("After executing LogDebug");
		}
	}
}
