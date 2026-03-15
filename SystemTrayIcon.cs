using System;
using System.Drawing;
using System.Windows.Forms;

namespace WpfAppTest
{
    public class SystemTrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _contextMenuStrip;
        private bool _disposed;

        public SystemTrayIcon(Icon icon, string toolTipText, Action? onRestore = null, Action? onExit = null)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = toolTipText,
                Visible = false
            };

            _contextMenuStrip = new ContextMenuStrip();
            
            var restoreMenuItem = new ToolStripMenuItem("Restore");
            restoreMenuItem.Click += (s, e) => onRestore?.Invoke();
            _contextMenuStrip.Items.Add(restoreMenuItem);
            
            _contextMenuStrip.Items.Add(new ToolStripSeparator());
            
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += (s, e) => onExit?.Invoke();
            _contextMenuStrip.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = _contextMenuStrip;
            _notifyIcon.Click += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            MouseEventArgs? mouseArgs = e as MouseEventArgs;
            if (mouseArgs?.Button == MouseButtons.Left)
            {
                OnIconClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
        }

        public void Hide()
        {
            _notifyIcon.Visible = false;
        }

        public void UpdateToolTip(string text)
        {
            _notifyIcon.Text = text;
        }

        public event EventHandler? OnIconClicked;

        public void Dispose()
        {
            if (!_disposed)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _contextMenuStrip.Dispose();
                _disposed = true;
            }
        }
    }
}
