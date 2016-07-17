using System;
// ReSharper disable InconsistentNaming
namespace GrapherApp.UI
{
    public static class Time
    {
        private static readonly DateTime StartTime = DateTime.UtcNow;

        public static float time => (float)(DateTime.UtcNow - StartTime).TotalSeconds;
        public static int frameCount => ((int)(DateTime.UtcNow - StartTime).TotalSeconds*90);
    }
}