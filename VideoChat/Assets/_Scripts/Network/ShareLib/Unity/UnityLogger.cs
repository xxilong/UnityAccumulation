using ShareLib.Unity;

namespace ShareLib.Log
{
    class UnityLogger : BaseLogger
    {
        public override void LogMessage(LogLevel level, string msg)
        {
            ThreadPool.RunInUI(() =>
            {
                switch (level)
                {
                    case LogLevel.DEBUG:
                        UnityEngine.Debug.Log(msg);
                        break;
                    case LogLevel.INFO:
                        UnityEngine.Debug.Log(msg);
                        break;
                    case LogLevel.WARNING:
                        UnityEngine.Debug.LogWarning(msg);
                        break;
                    case LogLevel.ERROR:
                        UnityEngine.Debug.LogError(msg);
                        break;
                    default:
                        break;
                }
                
            });
        }
    }
}
