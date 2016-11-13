using System;

namespace CtLab.Utilities
{
    public static class Extensions
    {
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }
    }
}

