using System;
using System.Runtime.InteropServices;

namespace ClipClip
{
    public class GlobalHotkey
    {
        [DllImport("user32.dll")]
        private static extern Boolean RegisterHotKey(IntPtr hWnd, Int32 id, Int32 fsModifiers, Int32 key);

        [DllImport("user32.dll")]
        private static extern Boolean UnregisterHotKey(IntPtr hWnd, Int32 id);

        private Int32 modifier;
        private Int32 key;
        private IntPtr hWnd;
        private Int32 id;

        public GlobalHotkey(Int32 modifier, Int32 key, IntPtr handle)
        {
            this.modifier = modifier;
            this.key = key;
            hWnd = handle;
            id = GetHashCode();
        }

        public override Int32 GetHashCode()
        {
            return modifier ^ key ^ hWnd.ToInt32();
        }

        public Boolean Register()
        {
            return RegisterHotKey(hWnd, id, modifier, key);
        }

        public Boolean Unregister()
        {
            return UnregisterHotKey(hWnd, id);
        }

        public static class Modifiers
        {
            public const Int32 NOMOD = 0x0000;
            public const Int32 ALT = 0x0001;
            public const Int32 CTRL = 0x0002;
            public const Int32 SHIFT = 0x0004;
            public const Int32 WIN = 0x0008;
        }

        public const Int32 WM_HOTKEY = 0x0312;
    }
}
