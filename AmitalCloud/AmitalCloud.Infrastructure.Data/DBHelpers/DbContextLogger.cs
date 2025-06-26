using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    internal class DbContextLogger : IDbContextLogger
    {
        private StringBuilder _StringBuilder;
        private Action _DisposeMe;
        private readonly Action<string> _logAction;

        internal DbContextLogger(DatabaseFacade database)
        {
            _StringBuilder = new StringBuilder();
            _logAction = LogMe;

            database.GetDbConnection().StateChange += LogMeToDatabase;

            this._DisposeMe = () =>
            {
                try
                {
                    database.GetDbConnection().StateChange -= LogMeToDatabase;
                }
                catch
                {
                }
            };
        }

        private void LogMe(string mess)
        {
            if (_StringBuilder == null)
                _StringBuilder = new StringBuilder();
            if (mess == Environment.NewLine)
            {
                return;
            }
            _StringBuilder.AppendLine(mess);
        }

        private void LogMeToDatabase(object sender, System.Data.StateChangeEventArgs e)
        {
            _StringBuilder.AppendLine($"State changed: {e.CurrentState}");
        }

        public override string ToString()
        {
            return _StringBuilder.ToString();
        }
        public string ToString(int lastCharacter)
        {
            var s = this.ToString();
            var sLast = s.Substring(Math.Max(0, s.Length - lastCharacter));
            return sLast;
        }
        public void Dispose()
        {
            _StringBuilder = null;
            _DisposeMe?.Invoke();
            _DisposeMe();
            _DisposeMe = null;
        }




        public void AddExplainLog(string ExplainLog)
        {
            LogMe(ExplainLog);
        }
    }
}
