using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Windows.Forms;

namespace Muffle
{
    public class TrayIcon
    {
        private MicIcon _iconState;
            
        public event IconChanger IconChanger;
        private Timer _animationTimer;

        public TrayIcon(IconChanger iconChanger)
        {
            _iconState = new MicIcon.Startup();

            IconChanger += iconChanger;

            _animationTimer = new Timer();
            _animationTimer.Interval = 2000;
            _animationTimer.Tick += SetCurrentIcon;
            _animationTimer.Enabled = true;
        }

        ~TrayIcon()
        {
            _animationTimer.Enabled = false;
        }

        private void SetCurrentIcon(object sender, EventArgs e)
        {
            SetIcon();
        }


        private void SetIcon()
        {
            IconChanger?.Invoke(_iconState.NextIcon);
        }

        public void SetStartup()
        {
            _iconState = new MicIcon.Startup();
            SetIcon();
        }

        public void SetUnmuted()
        {
            _iconState = new MicIcon.Unmuted();
            SetIcon();
        }

        public void SetMuted()
        {
            _iconState = new MicIcon.Muted();
            SetIcon();
        }

        public void SetUnmutedWarning()
        {
            _iconState = new MicIcon.UnmutedWarning();
            SetIcon();
        }

        public void SetMutedWarning()
        {
            _iconState = new MicIcon.MutedWarning();
            SetIcon();
        }
    }
}