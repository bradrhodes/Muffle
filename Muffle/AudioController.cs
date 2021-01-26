using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using AudioDeviceCmdlets;
using System.Linq;
using System.Threading;

namespace Muffle
{
    public class AudioController
    {
        public void MuteAllRecordingDevices()
        {
            if (GetCurrentMuteState() is MuteResult.Muted)
                return;

            Toggle();
        }

        public void UnmuteAllRecordingDevices()
        {
            if (GetCurrentMuteState() is MuteResult.Unmuted)
                return;

            Toggle();
        }

        public void Toggle()
        {
            var muteState = !GetAudioDevice.GetRecordingMute();

            var recordingDevices = GetAudioDevice.List().Where(device =>
                string.Equals(device.Type, "Recording", StringComparison.InvariantCultureIgnoreCase)).ToArray();

            SetMuteStateForAllDevices(recordingDevices, muteState);
        }

        private void SetMuteStateForAllDevices(IEnumerable<AudioDevice> devices, bool muteState)
        {
            foreach (var audioDevice in devices)
            {
                audioDevice.Device.AudioEndpointVolume.Mute = muteState;
            }
        }

        public MuteResult GetCurrentMuteState()
        {
            return GetAudioDevice.GetRecordingMute() switch
            {
                true => new MuteResult.Muted(),
                false => new MuteResult.Unmuted()
            };
        }

    }

    public abstract class MuteResult
    {
        public class Muted : MuteResult { }

        public class Unmuted : MuteResult { }
    }
}