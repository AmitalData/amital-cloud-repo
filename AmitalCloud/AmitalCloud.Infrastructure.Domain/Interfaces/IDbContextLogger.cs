using System;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IDbContextLogger : IDisposable
    {
        string ToString();
        void AddExplainLog(string ExplainLog);
        string ToString(int LastCharacter);
    }
}
