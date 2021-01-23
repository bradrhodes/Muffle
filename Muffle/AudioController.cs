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
        public DeviceState MuteAllRecordingDevices(DeviceState deviceState)
        {
            return GetCurrentMuteState() switch
            {
                MuteResult.Muted => deviceState,
                MuteResult.Unmuted => Toggle(deviceState),
                _  => throw new UnknownPatternTypeException("Unknown MuteResult type")
            };
        }

        public DeviceState UnmuteAllRecordingDevices(DeviceState deviceState)
        {
            return GetCurrentMuteState() switch
            {
                MuteResult.Muted => Toggle(deviceState),
                MuteResult.Unmuted => deviceState,
                _  => throw new UnknownPatternTypeException("Unknown MuteResult type")
            };
        }

        public DeviceState Toggle(DeviceState deviceState)
        {
            var checkForUnmutableDevices = false;
            var muteState = !GetAudioDevice.GetRecordingMute();
            var unmutableDevices = Enumerable.Empty<AudioDevice>();

            var recordingDevices = GetAudioDevice.List().Where(device =>
                string.Equals(device.Type, "Recording", StringComparison.InvariantCultureIgnoreCase)).ToArray();

            if (recordingDevices.Except(deviceState.CurrentDevices).Any())
                checkForUnmutableDevices = true;

            SetMuteStateForAllDevices(recordingDevices, muteState);

            if (checkForUnmutableDevices)
            {
                unmutableDevices = CheckForUnmutableDevices(recordingDevices);
                // I'm not sure if this is needed if I'm not setting the device as active before muting
            }

            // foreach (var recordingDeviceIndex in recordingDeviceIndexes)
            // {
            //     SetAudioDevice.SetByIndex(recordingDeviceIndex);
            //     try
            //     {
            //         SetAudioDevice.RecordingMuteToggle();
            //     }
            //     catch (Exception ex)
            //     {
            //         Debug.WriteLine($"Exception muting device: {ex.Message}");
            //     }
            // }

            // foreach (var recordingDevice in recordingDevices)
            // {
            //     var setRecordingDevice = SetAudioDevice.SetByInputObject(recordingDevice);
            //     setRecordingDevice.Device.AudioEndpointVolume.Mute = muteState;
            // }

            return new DeviceState(recordingDevices, unmutableDevices);

        }

        private IEnumerable<AudioDevice> CheckForUnmutableDevices(IEnumerable<AudioDevice> devices)
        {
            Thread.Sleep(501);
            return devices.Where(audioDevice => !audioDevice.Device.AudioEndpointVolume.Mute);
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

    public class DeviceState
    {
        public IEnumerable<AudioDevice> UnmutableDevices { get; }
        public IEnumerable<AudioDevice> CurrentDevices { get; }

        public DeviceState(IEnumerable<AudioDevice> currentDevices, IEnumerable<AudioDevice> unmutableDevices)
        {
            CurrentDevices = currentDevices;
            UnmutableDevices = unmutableDevices;
        }
    }

    public class UnknownPatternTypeException : Exception
    {
        public UnknownPatternTypeException(string message) : base(message)
        {
            
        }
    }
}