using System;
using System.Runtime.InteropServices;

namespace Agava.YandexGames
{
    public static class ServerTime
    {
        /// <summary>
        /// If this is called in the Editor, this will return the system time.
        /// </summary>
        public static long Milliseconds
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return GetServerTime();
#else
                return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
#endif
            }
        }

        [DllImport("__Internal")]
        private static extern long GetServerTime();
    }
}