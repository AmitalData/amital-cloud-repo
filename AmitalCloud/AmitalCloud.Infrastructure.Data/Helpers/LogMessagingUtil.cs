using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class LogMessagingUtil
    {
        [ThreadStatic]
        private static LogMessagingUtil _Instance;
        public static LogMessagingUtil Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LogMessagingUtil();
                }
                return _Instance;
            }

        }
        StringBuilder _StringBuilder;
        private int _Max;
        public bool LogMessaging { get; private set; }

        //public static LogMessagingUtil Instance { get; private set; }



        LogMessagingUtil()
        {
            if (Environment.UserDomainName.Equals("ntdomain", StringComparison.OrdinalIgnoreCase))
            {
                LogMessaging = true;
            }
            LogMessaging = true;//default yes yes yes !!!
            _StringBuilder = new StringBuilder();

        }
        public bool ToggleLogMessaging()
        {
            LogMessaging = !LogMessaging;
            return LogMessaging;
        }

        public LogMessagingUtil AppendLine(string value)
        {
            if (value.Length > 2048)
            {
                value = "<<<Truncate" + value.Substring(0, 2048) + "Truncate>>>";
            }
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug(value);
            if (!LogMessaging) return this;

            if (_StringBuilder.Length > _Max) return this;
            _StringBuilder.AppendLine(value);
            return this;
        }
        public LogMessagingUtil Append(object value)
        {
            Debug.Write(value);
            if (!LogMessaging) return this;
            if (_StringBuilder.Length > _Max) return this;
            _StringBuilder.Append(value);
            return this;
        }

        public void Clear(int max = 10000)
        {
            _Max = max;
            if (!LogMessaging) return;
            _StringBuilder.Clear();

        }



        public override string ToString()
        {
            return _StringBuilder.ToString();
        }

        public LogMessagingUtil LogActionTime(Action myAction,
            string ActionName = ""
            , [CallerMemberName] string myCallerMemberName = ""
            , [CallerFilePath] string myCallerFilePath = ""
            , [CallerLineNumber] int myCallerLineNumber = 0
            )
        {
            string err = "";
            var sw = Stopwatch.StartNew();
            try
            {
                myAction();
            }
            catch (Exception ee)
            {
                err = ee.ToString();
                throw;
            }

            finally
            {

                if (!string.IsNullOrWhiteSpace(ActionName))
                {
                    this.Append(ActionName);
                }
                else
                {
                    this.Append(myCallerFilePath).Append(":").Append(myCallerMemberName).Append("+").Append(myCallerLineNumber);
                }
                this.Append(":took:").AppendLine(sw.Elapsed.ToString());


            }
            return this;
        }
        public string GetLastChars(int maxLength)
        {


            return _StringBuilder.ToString().GetLast(maxLength);
        }
        public string ToString(int maxLength)
        {
            if (_StringBuilder.Length < maxLength)
            {
                return ToString();
            }
            return _StringBuilder.ToString().Substring(0, maxLength - 1);
            //return _StringBuilder.ToString().Substring(_StringBuilder.Length - maxLength);

        }
    }
    public static class StringExt
    {
        public static string GetLast(this string myString, int maxLength)
        {
            myString = myString ?? "";
            var len = myString.Length;
            if (len < maxLength)
            {
                return myString;
            }
            //return _StringBuilder.ToString().Substring(0, maxLength - 1);
            return myString.Substring(len - maxLength);

        }
    }

    public class LogMessagingUtilWR
    {
        [ThreadStatic]
        private static LogMessagingUtilWR _Instance;
        public static LogMessagingUtilWR Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LogMessagingUtilWR();
                }
                return _Instance;
            }

        }
        StringBuilder _StringBuilder;

        private int _Max;
        public bool LogMessaging { get; private set; }

        //public static LogMessagingUtil Instance { get; private set; }



        LogMessagingUtilWR()
        {
            if (Environment.UserDomainName.Equals("ntdomain", StringComparison.OrdinalIgnoreCase))
            {
                LogMessaging = true;
            }
            LogMessaging = true;//default yes yes yes !!!
            _StringBuilder = new StringBuilder();

        }

        private DateTime _LastClearAt;
        DateTime _LastWriteLineAt = DateTime.Now;
        public LogMessagingUtilWR AppendLine(string Line)
        {
            var ts = DateTime.Now.Subtract(_LastWriteLineAt);
            _LastWriteLineAt = DateTime.Now;
            if (Line.Length > 2048)
            {
                Line = "<<<Truncate" + Line.Substring(0, 2048) + "Truncate>>>";
            }
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug(Line);
            if (!LogMessaging) return this;

            if (_StringBuilder.Length > _Max) return this;
            var formatLine = ts.TotalMilliseconds + ":" + Line;
            _StringBuilder.AppendLine(formatLine);
            return this;
        }


        public void Clear(int max = 10000)
        {
            _LastClearAt = DateTime.Now;
            _LastWriteLineAt = DateTime.Now;
            _Max = max;
            if (!LogMessaging) return;
            _StringBuilder.Clear();

        }



        public string GetString(out string morethan)
        {

            var ts = DateTime.Now.Subtract(_LastClearAt);
            if (ts.TotalSeconds > 120)
            {
                morethan = "120";
                return _StringBuilder.AppendLine(">120:" + ts.TotalMilliseconds).ToString();
            }

            if (ts.TotalSeconds > 60)
            {
                morethan = "60";
                return _StringBuilder.AppendLine(">60:" + ts.TotalMilliseconds).ToString();
            }
            if (ts.TotalSeconds > 30)
            {
                morethan = "30";
                return _StringBuilder.AppendLine(">30:" + ts.TotalMilliseconds).ToString();
            }

            if (ts.TotalSeconds > 10)
            {
                morethan = "10";
                return _StringBuilder.AppendLine(">10:" + ts.TotalMilliseconds).ToString();
            }
            if (ts.TotalSeconds > 5)
            {
                morethan = "5";
                return _StringBuilder.AppendLine(">5:" + ts.TotalMilliseconds).ToString();
            }

            if (ts.TotalSeconds > 1)
            {
                morethan = "1";
                return _StringBuilder.AppendLine(">1:" + ts.TotalMilliseconds).ToString();
            }
            morethan = "lt1";
            return _StringBuilder.AppendLine("<1:" + ts.TotalMilliseconds).ToString();

        }




    }



    public class AssemblyUtil
    {


        public string GetVersion(string ProductInfo)
        {
            var parts = ProductInfo.Split(new char[] { ',' });
            return parts[0].Split(new char[] { ':' })[1];

        }
        public string BurnAt(string ProductInfo)
        {
            var parts = ProductInfo.Split(new char[] { ',' });
            return parts[0].Split(new char[] { ':' })[1];

        }
        public string GetProductInfo(Assembly assembly)
        {
            string prodInfo = "";
            try
            {


                var d111 = System.IO.File.GetLastWriteTime(assembly.Location);
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fileVersionInfo.ProductVersion;
                prodInfo = "Version:" + version + ",Burn At:" + d111.ToString();
            }
            catch (Exception)
            {


            }
            return prodInfo;
        }
    }
}
