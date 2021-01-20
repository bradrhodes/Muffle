using System;
using AudioDeviceCmdlets;
using System.Linq;

namespace Muffle
{
    public class AudioController
    {
        public MuteResult MuteAllRecordingDevices()
        {
            var (devEnum, devices) = GetAudioDevice.GetDeviceCollection();

            var recordingDevices = GetAudioDevice.List().Where(device =>
                string.Equals(device.Type, "Recording", StringComparison.InvariantCultureIgnoreCase));

            foreach (var recordingDevice in recordingDevices)
            {
                var newObject = SetAudioDevice.SetByInputObject(recordingDevice);
                newObject.Device.AudioEndpointVolume.Mute = true;
            }

            return new MuteResult.Muted();
        }

        public MuteResult UnmuteAllRecordingDevices()
        {
            var (devEnum, devices) = GetAudioDevice.GetDeviceCollection();

            var recordingDevices = GetAudioDevice.List().Where(device =>
                string.Equals(device.Type, "Recording", StringComparison.InvariantCultureIgnoreCase));

            foreach (var recordingDevice in recordingDevices)
            {
                recordingDevice.Device.AudioEndpointVolume.Mute = false;
            }

            return new MuteResult.Muted();
        }

        public MuteResult Toggle()
        {
            var muteState = !GetAudioDevice.GetRecordingMute();
            var (devEnum, devices) = GetAudioDevice.GetDeviceCollection();

            var recordingDevices = GetAudioDevice.List().Where(device =>
                string.Equals(device.Type, "Recording", StringComparison.InvariantCultureIgnoreCase));

            foreach (var recordingDevice in recordingDevices)
            {
                var setRecordingDevice = SetAudioDevice.SetByInputObject(recordingDevice);
                setRecordingDevice.Device.AudioEndpointVolume.Mute = muteState;
            }

            return muteState switch
            {
                true => new MuteResult.Muted(),
                false => new MuteResult.Unmuted()
            };
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