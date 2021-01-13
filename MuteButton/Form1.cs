using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioDeviceCmdlets;

namespace MuteButton
{
    public partial class Form1 : Form
    {
        private readonly Lazy<SerialPort> _lazyArduinoPort;
        private SerialPort ArduinoPort => _lazyArduinoPort.Value;

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = MuteButton.Properties.Resources.microphone_black;
            _lazyArduinoPort = new Lazy<SerialPort>(() => new SerialPort());

            timer1.Interval = (int) TimeSpan.FromSeconds(5).TotalMilliseconds;
            timer1.Tick += CheckMuteStatusEventHandler;
            timer1.Enabled = true;
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

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckMuteStatus();
        }

        private void SetupSerialPort()
        {

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
                return;
            }

            notifyIcon1.Icon = Properties.Resources.microphone_green;
            LogMessage("Mic status: Not Muted");
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
    }
}
