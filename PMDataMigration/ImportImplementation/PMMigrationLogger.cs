
using System;
using log4net;
using System.Threading;
using System.Drawing;

namespace PMImportImplementation
{
	public class PMMigrationLogger
	{
		public delegate void MessageLogging( string message,Color color, FontStyle fontstyle);
		public static event MessageLogging MessageLogged;
		
		public delegate void ErrorTypeMessageLogging( string message,Color color, FontStyle fontstyle );
		public static event ErrorTypeMessageLogging MessageLoggedWithError;
		
		public enum LogLevel
		{
			Debug,
			Info,
			Warn,
			Error,
			Fatal
		}
		
		static ILog m_Logger = null;
		
		public static void Configure()
		{
			log4net.Config.XmlConfigurator.Configure();
			m_Logger = log4net.LogManager.GetLogger("PMMigrationLog");
		}
		
		public static void Log( string message)
		{
			Log(message,LogLevel.Debug, Color.Black,FontStyle.Regular);
		}
		
		public static void Log( string message, Color color, FontStyle fontstyle)
		{
			Log(message,LogLevel.Debug, color, fontstyle);
		}
		
		public static void Log( string message, LogLevel level, Color color, FontStyle fontstyle)
		{
			switch (level)
            {
                case LogLevel.Debug:
                    m_Logger.Debug(message);
                    FormatAndRaiseLoggedMessage(message,color,fontstyle);
                    break;
                case LogLevel.Info:
                    m_Logger.Info(message);
                    FormatAndRaiseLoggedMessage(message,color,fontstyle);
                    break;
                case LogLevel.Warn:
                    m_Logger.Warn(message);
                    FormatAndRaiseLoggedMessage(message,color,fontstyle);
                    break;
                case LogLevel.Error:
                    m_Logger.Error(message);
                    FormatAndRaiseLoggedErrorMessage(message,color,fontstyle);
                    break;
                case LogLevel.Fatal:
                    m_Logger.Fatal(message);
                    FormatAndRaiseLoggedErrorMessage(message,color,fontstyle);
                    break;
            }
		}
		
		private static void FormatAndRaiseLoggedMessage(string message, Color color, FontStyle fontstyle)
        {
            string formattedMessage = string.Format("[{0}] >> {1} {2}", DateTime.Now, message,Environment.NewLine);
            //string formattedMessage = string.Format("{0} {1}", message, Environment.NewLine);

            if (MessageLogged != null)
            {
            	// Raising Event
                MessageLogged(formattedMessage, color,fontstyle);
            }
        }
		
		private static void FormatAndRaiseLoggedErrorMessage(string message,Color color, FontStyle fontstyle)
        {
            string formattedMessage = string.Format("[{0}] >> {1} {2}", DateTime.Now, message, Environment.NewLine);   
            //string formattedMessage = string.Format("{0} {1}", message, Environment.NewLine);

            if (MessageLoggedWithError != null)
            {
            	//Raising event
                MessageLoggedWithError(formattedMessage,color,fontstyle);
            }
        }
		
	}
}
