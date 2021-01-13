/*
  Copyright (c) 2016-2018 Francois Gendron <fg@frgn.ca>
  MIT License

  AudioDeviceCmdlets.cs
  AudioDeviceCmdlets is a suite of PowerShell Cmdlets to control audio devices on Windows
  https://github.com/frgnca/AudioDeviceCmdlets
*/

// To interact with MMDevice

using System;
using System.Collections.Generic;
using CoreAudioApi;

namespace AudioDeviceCmdlets
{
    // Class to interact with a MMDevice as an object with attributes
    public class AudioDevice
    {
        // Order in which this MMDevice appeared from MMDeviceEnumerator
        public int Index;
        // Default (for its Type) is either true or false
        public bool Default;
        // Type is either "Playback" or "Recording"
        public string Type;
        // Name of the MMDevice ex: "Speakers (Realtek High Definition Audio)"
        public string Name;
        // ID of the MMDevice ex: "{0.0.0.00000000}.{c4aadd95-74c7-4b3b-9508-b0ef36ff71ba}"
        public string ID;
        // The MMDevice itself
        public MMDevice Device;

        // To be created, a new AudioDevice needs an Index, and the MMDevice it will communicate with
        public AudioDevice(int Index, MMDevice BaseDevice, bool Default = false)
        {
            // Set this object's Index to the received integer
            this.Index = Index;

            // Set this object's Default to the received boolean
            this.Default = Default;

            // If the received MMDevice is a playback device
            if (BaseDevice.DataFlow == EDataFlow.eRender)
            {
                // Set this object's Type to "Playback"
                this.Type = "Playback";
            }
            // If not, if the received MMDevice is a recording device
            else if (BaseDevice.DataFlow == EDataFlow.eCapture)
            {
                // Set this object's Type to "Recording"
                this.Type = "Recording";
            }

            // Set this object's Name to that of the received MMDevice's FriendlyName
            this.Name = BaseDevice.FriendlyName;

            // Set this object's Device to the received MMDevice
            this.Device = BaseDevice;

            // Set this object's ID to that of the received MMDevice's ID
            this.ID = BaseDevice.ID;
        }
    }

    // Get Cmdlet
    public class GetAudioDevice 
    {
        public static (MMDeviceEnumerator DevEnum, MMDeviceCollection DeviceCollection) GetDeviceCollection()
        {
            // Create a new MMDeviceEnumerator
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            // Create a MMDeviceCollection of every devices that are enabled
            MMDeviceCollection deviceCollection =
                devEnum.EnumerateAudioEndPoints(EDataFlow.eAll, EDeviceState.DEVICE_STATE_ACTIVE);

            return (devEnum, deviceCollection);
        }

        public static IEnumerable<AudioDevice> List()
        {
            (var DevEnum, var DeviceCollection) = GetDeviceCollection();

            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is either, the same the default playback device's ID, or the same as the default recording device's ID
                if (DeviceCollection[i].ID ==
                    DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).ID ||
                    DeviceCollection[i].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).ID)
                {
                    // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself, and a default value of true
                    yield return new AudioDevice(i + 1, DeviceCollection[i], true);
                }

                // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself
                yield return new AudioDevice(i + 1, DeviceCollection[i]);
            }
        }

        public static AudioDevice GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Value cannot be null or empty.", nameof(id));

            var (DevEnum, DeviceCollection) = GetDeviceCollection();

            // For every MMDevice in DeviceCollection
            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is the same as the string received by the ID parameter
                if (string.Compare(DeviceCollection[i].ID, id, System.StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // If this MMDevice's ID is either, the same the default playback device's ID, or the same as the default recording device's ID
                    if (DeviceCollection[i].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).ID || DeviceCollection[i].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).ID)
                    {
                        // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself, and a default value of true
                        return new AudioDevice(i + 1, DeviceCollection[i], true);
                    }
                    // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself
                    return new AudioDevice(i + 1, DeviceCollection[i]);
                }
            }

            // Throw an exception about the received ID not being found
            throw new System.ArgumentException("No AudioDevice with that ID");
        }

        public static AudioDevice GetByIndex(int index)
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();

                if (index >= 1 && index <= DeviceCollection.Count)
                {
                    // If the ID of the MMDevice associated with the Index is the same as, either the ID of the default playback device, or the ID of the default recording device
                    if (DeviceCollection[index - 1].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).ID || DeviceCollection[index - 1].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).ID)
                    {
                        // Output the result of the creation of a new AudioDevice while assining it the an index, and the MMDevice itself, and a default value of true
                        return new AudioDevice(index, DeviceCollection[index - 1], true);
                    }

                    // Output the result of the creation of a new AudioDevice while assining it the an index, and the MMDevice itself, and a default value of false
                    return new AudioDevice(index, DeviceCollection[index - 1], false);
                }

                // Throw an exception about the received Index not being found
                throw new System.ArgumentException("No AudioDevice with that Index");
        }

        public static AudioDevice GetPlayback()
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();

            // For every MMDevice in DeviceCollection
            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is the same the default playback device's ID
                if (DeviceCollection[i].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).ID)
                {
                    // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself, and a default value of true
                    return new AudioDevice(i + 1, DeviceCollection[i], true);
                }
            }

            return null;
        }

        public static bool GetPlaybackMute()
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();

            return DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.Mute;
        }

        public static string GetPlaybackVolume()
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();
            return string.Format("{0}%", DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public static AudioDevice GetRecording()
        {

            var (DevEnum, DeviceCollection) = GetDeviceCollection();
            // For every MMDevice in DeviceCollection
            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is the same the default recording device's ID
                if (DeviceCollection[i].ID == DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).ID)
                {
                    // Output the result of the creation of a new AudioDevice while assining it an index, and the MMDevice itself, and a default value of true
                    return new AudioDevice(i + 1, DeviceCollection[i], true);
                }
            }

            // Stop checking for other parameters
            return null;
        }

        public static bool GetRecordingMute()
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();
            return DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.Mute;
        }

        public static string GetRecordingVolume()
        {
            var (DevEnum, DeviceCollection) = GetDeviceCollection();
            return string.Format("{0}%", DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }
    }

    // Set Cmdlet
    public class SetAudioDevice
    {
        public static AudioDevice SetByInputObject(AudioDevice inputObject)
        {
            if (inputObject == null) throw new ArgumentNullException(nameof(inputObject));

            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is the same as the ID of the MMDevice received by the InputObject parameter
                if (DeviceCollection[i].ID == inputObject.ID)
                {
                    // Create a new audio PolicyConfigClient
                    PolicyConfigClient client = new PolicyConfigClient();
                    // Using PolicyConfigClient, set the given device as the default playback communication device
                    client.SetDefaultEndpoint(DeviceCollection[i].ID, ERole.eCommunications);
                    // Using PolicyConfigClient, set the given device as the default playback device
                    client.SetDefaultEndpoint(DeviceCollection[i].ID, ERole.eMultimedia);

                    // Output the result of the creation of a new AudioDevice while assining it the an index, and the MMDevice itself, and a default value of true
                    return new AudioDevice(i + 1, DeviceCollection[i], true);
                }
            }

            // Throw an exception about the received device not being found
            throw new System.ArgumentException("No such enabled AudioDevice found");
        }

        public static AudioDevice SetById(string id)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            // For every MMDevice in DeviceCollection
            for (int i = 0; i < DeviceCollection.Count; i++)
            {
                // If this MMDevice's ID is the same as the string received by the ID parameter
                if (string.Compare(DeviceCollection[i].ID, id, System.StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // Create a new audio PolicyConfigClient
                    PolicyConfigClient client = new PolicyConfigClient();
                    // Using PolicyConfigClient, set the given device as the default communication device (for its type)
                    client.SetDefaultEndpoint(DeviceCollection[i].ID, ERole.eCommunications);
                    // Using PolicyConfigClient, set the given device as the default device (for its type)
                    client.SetDefaultEndpoint(DeviceCollection[i].ID, ERole.eMultimedia);

                    // Output the result of the creation of a new AudioDevice while assining it the index, and the MMDevice itself, and a default value of true
                    return new AudioDevice(i + 1, DeviceCollection[i], true);
                }
            }

            // Throw an exception about the received ID not being found
            throw new System.ArgumentException("No enabled AudioDevice found with that ID");

        }

        public static AudioDevice SetByIndex(int index)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            // If the Index is valid
            if (index >= 1 && index <= DeviceCollection.Count)
            {
                // Create a new audio PolicyConfigClient
                PolicyConfigClient client = new PolicyConfigClient();
                // Using PolicyConfigClient, set the given device as the default communication device (for its type)
                client.SetDefaultEndpoint(DeviceCollection[index - 1].ID, ERole.eCommunications);
                // Using PolicyConfigClient, set the given device as the default device (for its type)
                client.SetDefaultEndpoint(DeviceCollection[index - 1].ID, ERole.eMultimedia);

                // Output the result of the creation of a new AudioDevice while assining it the index, and the MMDevice itself, and a default value of true
                return new AudioDevice(index, DeviceCollection[index - 1], true);
            }
            else
            {
                // Throw an exception about the received Index not being found
                throw new System.ArgumentException("No enabled AudioDevice found with that Index");
            }
        }

        public static void SetPlaybackMute(bool muteState)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.Mute = muteState;
        }

        public static void PlaybackMuteToggle()
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.Mute = !DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.Mute;
        }

        public static void SetPlaybackVolume(float volumeAsPercentage)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar = volumeAsPercentage / 100.0f;
        }

        public static void SetRecordingMute(bool muteState)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            // Set the mute state of the default recording device to that of the boolean value received by the Cmdlet
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.Mute = muteState;
        }

            // If the RecordingMuteToggle paramter was called
        public static void RecordingMuteToggle()
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            // Toggle the mute state of the default recording device
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.Mute = !DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.Mute;
        }

            // If the RecordingVolume parameter received a value
        public static void SetRecordingVolume(int volumeAsPercentage)
        {
            var (DevEnum, DeviceCollection) = GetAudioDevice.GetDeviceCollection();

            // Set the volume level of the default recording device to that of the float value received by the RecordingVolume parameter
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar = volumeAsPercentage / 100.0f;
        }
    }

    public class WriteAudioDevice
    {
        public static int GetPlaybackProgress()
        {
            var DevEnum = new MMDeviceEnumerator();
            return System.Convert.ToInt32(DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioMeterInformation.MasterPeakValue * 100);
        }

        public static int GetStreamProgress()
        {
            var DevEnum = new MMDeviceEnumerator();
            return System.Convert.ToInt32(DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioMeterInformation.MasterPeakValue * 100);
        }

        public static int GetRecordingMeter()
        {
            var DevEnum = new MMDeviceEnumerator();
            return System.Convert.ToInt32(DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioMeterInformation.MasterPeakValue * 100);
        }

        public static int GetRecordingStream()
        {
            var DevEnum = new MMDeviceEnumerator();
            return System.Convert.ToInt32(DevEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia).AudioMeterInformation.MasterPeakValue * 100);
        }
    }
}
