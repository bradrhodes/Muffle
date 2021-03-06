﻿using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AudioDeviceCmdlets;
using Microsoft.Extensions.Logging;
using Muffle.Button;

namespace Muffle
{
    public partial class Form1 : Form
    {
        private readonly ILogger _logger;

        private bool _allowVisible = false;
        private bool _buttonWarningDisabled = false;
        private bool _previousButtonConnectedState = true;

        private Settings _settings;
        private readonly MuteButtonFactory _muteButtonFactory;
        private MuteButton _muteButton;
        private readonly AudioController _audioController;

        private readonly BindingSource _bindingSource;
        private readonly TrayIcon _trayIcon;

        public Form1(ILogger<Form1> logger, IBindLogData logDataBinder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _bindingSource = logDataBinder.Source;

            _trayIcon = new TrayIcon(this.SetIcon);

            InitializeComponent();
            _muteButtonFactory = new MuteButtonFactory(_logger);
            _audioController = new AudioController();

            this.Icon = Properties.Resources.microphone_black;
            notifyIcon1.Icon = Muffle.Properties.Resources.microphone_black;

            InitializeSettings();
            InitializeButton();
            CheckMuteStatus();

            timer1.Interval = (int) TimeSpan.FromSeconds(5).TotalMilliseconds;
            timer1.Tick += CheckMuteStatusEventHandler;
            timer1.Enabled = true;

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Hide();

            listBox1.DataSource = _bindingSource;
        }

        protected override void SetVisibleCore(bool value)
        {
            if(!_allowVisible)
            {
                value = false;
                if(!this.IsHandleCreated) CreateHandle();
            }

            base.SetVisibleCore(value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckMuteStatus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToggleMuteStatus();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToggleMuteStatus();
        }

        private void InitializeSettings()
        {
            _settings = Settings.Initialize();
        }

        private void PopButtonNotConnectedTooltip()
        {
            if (_buttonWarningDisabled)
                return;

            notifyIcon1.BalloonTipTitle = "Button Not Connected";
            notifyIcon1.BalloonTipText = "Right click and select Connect Button to connect.";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
            // notifyIcon1.BalloonTipClicked += ConnectButtonMenuItem_Click;

            notifyIcon1.Visible = true;
            // notifyIcon1.ShowBalloonTip((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            notifyIcon1.ShowBalloonTip(int.MaxValue);
            _buttonWarningDisabled = true;
        }

        private void InitializeButton()
        {
            _muteButton = _muteButtonFactory.Create(_settings.ArduinoSettings);
            _muteButton.ProcessMessage += ProcessMuteButtonMessages;

            if (_muteButton is NullMuteButton)
            {
                PopButtonNotConnectedTooltip();
            }
        }

        private void ToggleMuteStatus()
        {
            _audioController.Toggle();
            CheckMuteStatus();
        }

        private void CheckMuteStatus()
        {
            var buttonConnected = CheckButtonConnectedState();

            _logger.LogInformation("Checking mute status");
            if (_audioController.GetCurrentMuteState() is MuteResult.Muted)
            {
                // notifyIcon1.Icon = Properties.Resources.microphone_red;
                if(buttonConnected)
                    _trayIcon.SetMuted();
                else
                    _trayIcon.SetMutedWarning();
                _logger.LogInformation("Mic status: Muted");
                _muteButton.SetMuteStateTrue();
                return;
            }

            // notifyIcon1.Icon = Properties.Resources.microphone_green;
            if(buttonConnected)
                _trayIcon.SetUnmuted();
            else
                _trayIcon.SetUnmutedWarning();

            _logger.LogInformation("Mic status: Not Muted");
            _muteButton.SetMuteStateFalse();

        }


        private bool CheckButtonConnectedState()
        {
            var buttonConnected = _muteButton.IsConnected();

            if (!buttonConnected && _previousButtonConnectedState)
            {
                _buttonWarningDisabled = false;
                IgnoreWarningsMenuItem.CheckState = CheckState.Unchecked;
            }

            _previousButtonConnectedState = buttonConnected;

            if(!buttonConnected)
                PopButtonNotConnectedTooltip();

            return buttonConnected;
        }

        private void CheckMuteStatusEventHandler(object sender, EventArgs e)
        {
            CheckMuteStatus();
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void MuteMenuItem_Click(object sender, EventArgs e)
        {
            _audioController.MuteAllRecordingDevices();
            CheckMuteStatus();
        }

        private void UnmuteMenuItem_Click(object sender, EventArgs e)
        {
            _audioController.UnmuteAllRecordingDevices();
            CheckMuteStatus();
        }

        private void DebugMenuItem_Click(object sender, EventArgs e)
        {
            _allowVisible = true;
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            _audioController.UnmuteAllRecordingDevices();
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SetupSerialPort();
        }

        private void ConnectButtonMenuItem_Click(object sender, EventArgs e)
        {
            ConnectButton();
        }

        private void ConnectButton()
        {
            using(var connectButtonForm = new ConnectButtonForm())
            {
                if(connectButtonForm.ShowDialog() == DialogResult.OK)
                {
                    _logger.LogInformation($"Arduino port discovered: {connectButtonForm.PortName}");

                    // Save the port and make the connection
                    var port = connectButtonForm.PortName;
                    var baud = connectButtonForm.BaudRate;

                    _settings.UpdateConnectionSettings(port, baud);

                    _logger.LogInformation($"Button settings updated: {_settings.ArduinoSettings.PortName}, {_settings.ArduinoSettings.BaudRate}");

                    _logger.LogInformation($"Reinitializing button.");
                    InitializeButton();
                    _logger.LogInformation("Button reinitialized.");

                    return;
                }

                _logger.LogInformation($"Arduino port not discovered.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConnectButton();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                Hide();
                notifyIcon1.Visible = true;
                return;
            }
        }

        private void ProcessMuteButtonMessages(string message)
        {
            _logger.LogInformation($"Received from button: {message}");

            if(string.Equals(message, "togglemutestate"))
            {
                ToggleMuteStatus();
            }
            else if(string.Equals(message, "getcurrentmutestate"))
            {
                CheckMuteStatus();
            }
        }

        private void IgnoreWarningsMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            _buttonWarningDisabled = menuItem.CheckState switch
            {
                CheckState.Checked => true,
                CheckState.Unchecked => false,
                CheckState.Indeterminate => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException()
            };
        }

        private void GetMuteButtonStatus_button_Click(object sender, EventArgs e)
        {

        }

        private void SetIcon(Icon icon)
        {
            notifyIcon1.Icon = icon;
        }
    }
}
