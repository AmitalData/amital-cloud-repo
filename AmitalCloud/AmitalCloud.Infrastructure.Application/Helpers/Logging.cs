using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;


namespace AmitalCloud.Infrastructure.Application.Helpers
{
	public class Logging : ILogging
	{
		public void LogDebug(string message)
		{
			Console.WriteLine($"Debug Write To DB: {message}");
		}

		public void LogWarning(string message)
		{
			Console.WriteLine($"Warning Write To DB: {message}");
		}
	}
}
