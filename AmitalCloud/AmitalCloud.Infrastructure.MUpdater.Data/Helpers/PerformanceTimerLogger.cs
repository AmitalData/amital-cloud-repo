using System.Diagnostics;
using System.Text;

namespace AmitalCloud.Infrastructure.MUpdater.Data.Helpers

{
    public class PerformanceTimerLogger
    {
        Stopwatch stepTimer;
        Stopwatch allProccessTimer;
        StringBuilder loggBuilder;
        public PerformanceTimerLogger()
        {
            loggBuilder = new StringBuilder();
            stepTimer = new Stopwatch();
            allProccessTimer = new Stopwatch();

         
        }

        public void Start()
        {
            loggBuilder = new StringBuilder();
            stepTimer.Start();
            allProccessTimer.Start();
        }
        public void Stop()
        {
            loggBuilder = new StringBuilder();
            stepTimer.Stop();
            allProccessTimer.Stop();
        }
        public void LogMessage(string message)
        {
            var newLine = string.Format("{0},{1}", message, stepTimer.Elapsed.ToString());//
            loggBuilder.AppendLine(newLine);
            stepTimer.Restart();
           
        }

        public void WriteLogToCSVFile()
        {
          

            var newLine = string.Format("{0},{1}", ",Total", allProccessTimer.Elapsed.ToString());
            loggBuilder.AppendLine(newLine);

            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string filePath = Path.Combine(projectDirectory, @"performancelog.csv");
            File.WriteAllText(filePath, loggBuilder.ToString());

            Stop();


        }
    }
}