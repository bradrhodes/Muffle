using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioDeviceCmdlets;

namespace MuteButton
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var muteStatus = GetAudioDevice.GetRecordingMute();
            textBox1.AppendText($@"Mute status: {muteStatus}{Environment.NewLine}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += $@"Toggling mute status...{Environment.NewLine}";
            SetAudioDevice.RecordingMuteToggle();
            textBox1.AppendText($"Toggled.{Environment.NewLine}");
        }
    }
}
