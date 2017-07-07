using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipClip
{
    public partial class MainForm : Form
    {
        private GlobalHotkey pasteHotkey;
        private QueuedClipboardManager queuedClipboardManager;

        public MainForm()
        {
            InitializeComponent();

            ContextMenu contextMenu = CreateContextMenu();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.ContextMenu = contextMenu;

            pasteHotkey = new GlobalHotkey(GlobalHotkey.Modifiers.CTRL + GlobalHotkey.Modifiers.SHIFT, (Int32)Keys.V, Handle);
            pasteHotkey.Register();

            queuedClipboardManager = new QueuedClipboardManager(2);
            queuedClipboardManager.SubscribeToClipboard(Handle);
        }

        private ContextMenu CreateContextMenu()
        {
            var contextMenu = new ContextMenu();

            var settingItem = new MenuItem("Settings", new EventHandler(MenuSettingsItem_Click));
            var exitItem = new MenuItem("Exit", new EventHandler(MenuExitItem_Click));

            contextMenu.MenuItems.Add(settingItem);
            contextMenu.MenuItems.Add(exitItem);

            return contextMenu;
        }

        private void MenuSettingsItem_Click(Object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void MenuExitItem_Click(Object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == GlobalHotkey.WM_HOTKEY)
            {
                HandleHotkey(message);
            }
            else if (message.Msg == QueuedClipboardManager.WM_DRAWCLIPBOARD)
            {
                HandleClipboardChange(message);
                return;
            }
            else if (message.Msg == QueuedClipboardManager.WM_CHANGECBCHAIN)
            {
                HandleClipboardSubscriberChainChange(message);
                return;
            }
            
            base.WndProc(ref message);
        }

        private void HandleHotkey(Message message)
        {
            if (message.WParam.ToInt32() == pasteHotkey.GetHashCode())
            {
                queuedClipboardManager.PasteNext();
            }
        }

        private void HandleClipboardChange(Message message)
        {
            queuedClipboardManager.HandleNewValue(message);
        }

        private void HandleClipboardSubscriberChainChange(Message message)
        {
            queuedClipboardManager.HandleSubscriberChainChange(message);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pasteHotkey.Unregister();
        }
    }
}

