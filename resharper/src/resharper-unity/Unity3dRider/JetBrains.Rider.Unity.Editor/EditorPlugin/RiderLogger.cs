using System;
using System.IO;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace Plugins.Editor.JetBrains
{
  public class RiderLoggerFactory : ILogFactory
  {
    public ILog GetLog(string category)
    {
      return new RiderLogger(category);
    }
  }

  public class RiderLogger : ILog
  {
    public RiderLogger(string category)
    {
      Category = category;
    }

    public bool IsEnabled(LoggingLevel level)
    {
      return level <= Menu.SelectedLoggingLevel;
    }

    public void Log(LoggingLevel level, string message, Exception exception = null)
    {
      if (!IsEnabled(level))
        return;

      // ReSharper disable once StringLastIndexOfIsCultureSpecific.1
      var dotidx = Category.LastIndexOf(".");
      var categoryText = Category.Substring(dotidx >= 0 ? dotidx + 1 : 0);
      var text = categoryText + "[" + level + "]" +
                 DateTime.Now.ToString(global::JetBrains.Util.Logging.Log.DefaultDateFormat) + " " + message;

      // using Unity logs causes frequent Unity hangs
      RiderPlugin.MainThreadDispatcher1.Queue(() =>
      {
        if (!new FileInfo(RiderPlugin.logPath).Directory.Exists)
          new FileInfo(RiderPlugin.logPath).Directory.Create();
        File.AppendAllText(RiderPlugin.logPath, Environment.NewLine + text);
      });

//      switch (level)
//      {
//        case LoggingLevel.FATAL:
//        case LoggingLevel.ERROR:
//          Debug.LogError(text);
//          if (exception != null)
//            Debug.LogException(exception);
//          break;
//        case LoggingLevel.WARN:
//          Debug.LogWarning(text);
//          if (exception != null)
//            Debug.LogException(exception);
//          break;
//        case LoggingLevel.INFO:
//        case LoggingLevel.VERBOSE:
//          Debug.Log(text);
//          if (exception != null)
//            Debug.LogException(exception);
//          break;
//        default:
//          break;
//      }
    }

    public string Category { get; private set; }
  }
}