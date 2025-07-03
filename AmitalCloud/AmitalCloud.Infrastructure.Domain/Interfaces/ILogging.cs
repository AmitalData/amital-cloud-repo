using System;


namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
	public interface ILogging
	{
		public void LogWarning(string message);
		public void LogDebug(string message);

	}
}
