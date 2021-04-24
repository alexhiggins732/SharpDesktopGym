using System;

namespace GymSharp
{
    public static class CleanupExtensions
    {
        public static void TryDispose(this object obj)
        {
            if (obj != null && obj is IDisposable src)
            {
                try
                {
                    src.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
