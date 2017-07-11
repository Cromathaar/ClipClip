using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClipClip
{
    public class ClipboardRing
    {
        public const Int32 WM_DRAWCLIPBOARD = 0x0308;
        public const Int32 WM_CHANGECBCHAIN = 0x030D;

        private IntPtr nextClipboardSubscriber;
        private CircularList<String> circularList;

        #region WinApi Imported Functions
        [DllImport("user32.dll")]
        protected static extern Int32 SetClipboardViewer(Int32 hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SendMessage(IntPtr hwnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll")]
        static extern UInt32 GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 lpdwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, Boolean fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private extern static Int32 GetCaretPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern Boolean ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        #endregion

        public ClipboardRing(Byte ringDepth)
        {
            circularList = new CircularList<String>(ringDepth);
        }

        public void SubscribeToClipboard(IntPtr handle)
        {
            Subscribe(handle);
            if (TheOnlySubscriber(handle))
            {
                ForgetNextSubscriber();
            }
        }

        private void Subscribe(IntPtr handle)
        {
            nextClipboardSubscriber = (IntPtr)SetClipboardViewer(handle.ToInt32());
        }

        private Boolean TheOnlySubscriber(IntPtr handle)
        {
            return nextClipboardSubscriber == handle;
        }

        private void ForgetNextSubscriber()
        {
            nextClipboardSubscriber = IntPtr.Zero;
        }

        public void HandleNewValue(Message message)
        {
            if (Clipboard.ContainsText())
            {
                String clip = Clipboard.GetText();

                if (ClipNotADuplicate(clip))
                {
                    circularList.Add(Clipboard.GetText());
                }
            }

            PassMessageToNextSubscriber(message);
        }

        private Boolean ClipNotADuplicate(String clip)
        {
            return circularList.Count == 0 || clip != circularList.GetCurrent();
        }

        private void PassMessageToNextSubscriber(Message message)
        {
            SendMessage(nextClipboardSubscriber, message.Msg, message.WParam, message.LParam);
        }

        public void HandleSubscriberChainChange(Message message)
        {
            if (NextSubscriberUnsubscribed(message))
            {
                ReassignNextSubscriber(message);
            }
            else
            {
                PassMessageToNextSubscriber(message);
            }
        }

        private Boolean NextSubscriberUnsubscribed(Message message)
        {
            return message.WParam == nextClipboardSubscriber;
        }

        private void ReassignNextSubscriber(Message message)
        {
            nextClipboardSubscriber = message.LParam;
        }

        public void PasteNext()
        {
            if (circularList.Count == 0)
            {
                return;
            }

            Clipboard.SetText(circularList.GetNext());

            IntPtr foregroundWindowHandle = GetForegroundWindow();

            UInt32 currentThreadId = GetCurrentThreadId();
            UInt32 processId;
            UInt32 foregroundWindowThreadId = GetWindowThreadProcessId(foregroundWindowHandle, out processId);

            if (foregroundWindowThreadId != currentThreadId)
            {
                AttachThreadInput((IntPtr)currentThreadId, (IntPtr)foregroundWindowThreadId, true);
            }

            IntPtr focusedControlHandle = GetFocus();

            SendMessage(focusedControlHandle, 0x0302, IntPtr.Zero, IntPtr.Zero);

            if (foregroundWindowThreadId != currentThreadId)
            {
                AttachThreadInput((IntPtr)currentThreadId, (IntPtr)foregroundWindowThreadId, false);
            }
        }
    }
}