using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using AudioDeviceCmdlets;


namespace Muffle
{
    class MediaController
    {
        private InputSimulator _input;
        public MediaController()
        {
            _input = new InputSimulator();
        }
        public MuteResult ToggleMute()
        {
            // _input.Keyboard.KeyPress(VirtualKeyCode.LWIN, VirtualKeyCode.SHIFT, VirtualKeyCode.VK_A);
            _input.Keyboard.ModifiedKeyStroke(new[] {VirtualKeyCode.LWIN, VirtualKeyCode.SHIFT}, VirtualKeyCode.VK_A);

            return GetAudioDevice.GetRecordingMute() switch
            {
                true => new MuteResult.Muted(),
                false => new MuteResult.Unmuted()
            };
        }

        public MuteResult GetMuteState()
        {
            return GetAudioDevice.GetRecordingMute() switch
            {
                true => new MuteResult.Muted(),
                false => new MuteResult.Unmuted()
            };
        }
    }
}
