using System;
using System.Drawing;
using System.Windows.Forms;
using AudioDeviceCmdlets;
using Muffle.Button;

namespace Muffle
{
    public partial class Form1 : Form
    {

        private bool _allowVisible = false;
        private bool _buttonWarningDisabled = false;

        private Settings _settings;
        private readonly ButtonFactory _buttonFactory;

        public Form1()
        {
            InitializeComponent();
            _buttonFactory = new ButtonFactory();

            this.Icon = Properties.Resources.microphone_black;
            notifyIcon1.Icon = Muffle.Properties.Resources.microphone_black;

            timer1.Interval = (int) TimeSpan.FromSeconds(5).TotalMilliseconds;
            timer1.Tick += CheckMuteStatusEventHandler;
            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckMuteStatus();
            Hide();

            InitializeSettings();
            InitializeArduino();
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
            if(_settings == null)
            {
                PopTooltip();
            }
        }

        private void PopTooltip()
        {
            notifyIcon1.BalloonTipTitle = "Button Not Connected";
            notifyIcon1.BalloonTipText = "Click here to connect.";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;

            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
        }

        private void InitializeArduino()
        {
            if(_settings == null || _settings?.ArduinoSettings == null)
            {
                PopTooltip();
                return;
            }

            _arduino = new Arduino(_settings.ArduinoSettings);
            _arduino.ProcessMessage += ProcessArduinoMessages;
        }

        private void ToggleMuteStatus()
        {
            SetAudioDevice.RecordingMuteToggle();
            CheckMuteStatus();
        }

        private void CheckMuteStatus()
        {
            if (GetAudioDevice.GetRecordingMute())
            {
                notifyIcon1.Icon = Properties.Resources.microphone_red;
                LogMessage("Mic status: Muted");
                _arduino.SendMessage("mutestatetrue");
                return;
            }

            notifyIcon1.Icon = Properties.Resources.microphone_green;
            LogMessage("Mic status: Not Muted");
            _arduino.SendMessage("mutestatefalse");
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
            SetAudioDevice.SetRecordingMute(true);
            CheckMuteStatus();
        }

        private void UnmuteMenuItem_Click(object sender, EventArgs e)
        {
            SetAudioDevice.SetRecordingMute(false);
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
            SetAudioDevice.SetRecordingMute(false);
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

        private void ProcessArduinoMessages(string message)
        {
            if(string.Equals(message, "togglemutestate"))
            {
                ToggleMuteStatus();
            }
            else if(string.Equals(message, "getcurrentmutestate"))
            {
                CheckMuteStatus();
            }
        }
    }
}
