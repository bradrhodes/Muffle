using System;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AudioDeviceCmdlets;
using Muffle.Button;

namespace Muffle
{
    public partial class Form1 : Form
    {

        private bool _allowVisible = false;
        private bool _buttonWarningDisabled = false;

        private DeviceState _deviceState =
            new(Enumerable.Empty<AudioDevice>(), Enumerable.Empty<AudioDevice>());

        private Settings _settings;
        private readonly MuteButtonFactory _muteButtonFactory;
        private MuteButton _muteButton;
        private readonly AudioController _audioController;

        public Form1()
        {
            InitializeComponent();
            _muteButtonFactory = new MuteButtonFactory();
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
            notifyIcon1.ShowBalloonTip((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
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
            // SetAudioDevice.RecordingMuteToggle();
            _deviceState = _audioController.Toggle(_deviceState);
            // _mediaController.ToggleMute();
            CheckMuteStatus();
        }

        private void CheckMuteStatus()
        {
            if (_audioController.GetCurrentMuteState() is MuteResult.Muted)
            {
                notifyIcon1.Icon = Properties.Resources.microphone_red;
                LogMessage("Mic status: Muted");
                _muteButton.SetMuteStateTrue();
                return;
            }

            notifyIcon1.Icon = Properties.Resources.microphone_green;
            LogMessage("Mic status: Not Muted");
            _muteButton.SetMuteStateFalse();

            if (!_muteButton.IsConnected())
            {
                PopButtonNotConnectedTooltip();
            }
        }

        private void CheckMuteStatusEventHandler(object sender, EventArgs e)
        {
            CheckMuteStatus();
        }

        private void LogMessage(string message)
        {
            if(listBox1.Items.Count > 40)
                listBox1.Items.RemoveAt(0);

            listBox1.Items.Add(message);
            listBox1.TopIndex = listBox1.Items.Count - 1;
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
            // SetAudioDevice.SetRecordingMute(true);
            _deviceState = _audioController.MuteAllRecordingDevices(_deviceState);
            // if (_mediaController.GetMuteState() is MuteResult.Unmuted)
            //     _mediaController.ToggleMute();
            CheckMuteStatus();
        }

        private void UnmuteMenuItem_Click(object sender, EventArgs e)
        {
            // SetAudioDevice.SetRecordingMute(false);
            _deviceState = _audioController.UnmuteAllRecordingDevices(_deviceState);
            // if (_mediaController.GetMuteState() is MuteResult.Muted)
            //     _mediaController.ToggleMute();
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
            // SetAudioDevice.SetRecordingMute(false);
            _deviceState = _audioController.UnmuteAllRecordingDevices(_deviceState);
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
                    LogMessage($"Arduino port discovered: {connectButtonForm.PortName}");

                    // Save the port and make the connection
                    var port = connectButtonForm.PortName;
                    var baud = 115200;

                    _settings.UpdateConnectionSettings(port, baud);

                    return;
                }

                LogMessage($"Arduino port not discovered.");
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
            Debug.WriteLine($"Inbound: {message}");
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
                CheckState.Indeterminate => throw new InvalidOperationException()
            };
        }
    }
}
