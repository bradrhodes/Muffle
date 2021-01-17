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

namespace Muffle
{
    public partial class ConnectButtonForm : Form
    {
        private Timer _checkForAruino;
        private IEnumerable<string> _initialComPorts = Enumerable.Empty<string>();

        public string PortName { get; private set; } = string.Empty;

        public ConnectButtonForm()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.microphone_black;

            _checkForAruino = new Timer()
            {
                Interval = 50,
            };

            _checkForAruino.Tick += CheckForArduino;
        }

        private void CheckForArduino(object sender, EventArgs e)
        {
            var currentComPorts = SerialPort.GetPortNames();

            var diff = currentComPorts.Except(_initialComPorts).SingleOrDefault();

            if(diff != null)
            {
                _checkForAruino.Enabled = false;
                PortName = diff;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void buttonPair_Click(object sender, EventArgs e)
        {

            _initialComPorts = SerialPort.GetPortNames();
            label1.Text = "Please plug in the button now.";

            _checkForAruino.Enabled = true;
        }

        private void ConnectButtonForm_Load(object sender, EventArgs e)
        {

        }
    }
}
