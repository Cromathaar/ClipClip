﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipClip
{
    public partial class MainForm : Form
    {
        private const Byte CLIPBOARD_RING_DEPTH = 2;

        private GlobalHotkey pasteHotkey;
        private ClipboardRing clipboardRing;

        public MainForm()
        {
            InitializeComponent();

            ContextMenu contextMenu = CreateContextMenu();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.ContextMenu = contextMenu;

            pasteHotkey = new GlobalHotkey(GlobalHotkey.Modifiers.CTRL + GlobalHotkey.Modifiers.SHIFT, (Int32)Keys.V, Handle);
            pasteHotkey.Register();

            clipboardRing = new ClipboardRing(CLIPBOARD_RING_DEPTH);
            clipboardRing.SubscribeToClipboard(Handle);
        }

        private ContextMenu CreateContextMenu()
        {
            var contextMenu = new ContextMenu();

            var exitItem = new MenuItem("Exit", new EventHandler(MenuExitItem_Click));

            contextMenu.MenuItems.Add(exitItem);

            return contextMenu;
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
            else if (message.Msg == ClipboardRing.WM_DRAWCLIPBOARD)
            {
                HandleClipboardChange(message);
                return;
            }
            else if (message.Msg == ClipboardRing.WM_CHANGECBCHAIN)
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
                clipboardRing.PasteNext();
            }
        }

        private void HandleClipboardChange(Message message)
        {
            clipboardRing.HandleNewValue(message);
        }

        private void HandleClipboardSubscriberChainChange(Message message)
        {
            clipboardRing.HandleSubscriberChainChange(message);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pasteHotkey.Unregister();
        }
    }
}

