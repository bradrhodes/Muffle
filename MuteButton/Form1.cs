using System;
using System.Windows.Forms;
using AudioDeviceCmdlets;

namespace MuteButton
{
    public partial class Form1 : Form
    {

        private bool _allowVisible = false;
        private Lazy<Settings> _lazySettings;
        private Settings Settings => _lazySettings.Value;

        private readonly Arduino _arduino;

        public Form1()
        {
            InitializeComponent();

            _lazySettings = new Lazy<Settings>(() => Settings.Initialize());

            this.Icon = Properties.Resources.microphone_black;
            notifyIcon1.Icon = MuteButton.Properties.Resources.microphone_black;

            timer1.Interval = (int) TimeSpan.FromSeconds(5).TotalMilliseconds;
            timer1.Tick += CheckMuteStatusEventHandler;
            timer1.Enabled = true;

            _arduino = new Arduino(Settings.AppSettings);
            _arduino.ProcessMessage += ProcessArduinoMessages;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckMuteStatus();
            Hide();
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
