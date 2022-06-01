using NAudio.CoreAudioApi;
using System;

namespace OpenVoiceModder.Sound
{
    public class GlobalSound
    {
        private GlobalSound() { }
        public static GlobalSound Instance { get; } = new();

        public DeviceAudioSource? CurrentMicrophoneSource { get; protected set; }
        public MixedAudioSource? CurrentAudioSource { get; protected set; }
        public FileAudioSource? CurrentSoundboardSource { get; protected set; }

        public void Initialize(MMDevice input, MMDevice output)
        {
            CurrentAudioSource = new(new());
            CurrentMicrophoneSource = DeviceAudioSource.Create(input);

            CurrentAudioSource.RegisterAudioSource(CurrentMicrophoneSource);
            AudioPlayer.Instance.AudioPool.RegisterAudioSource(CurrentAudioSource);
            AudioPlayer.Instance.SetOutput(output);
        }

        public void SetRecordingDevice(MMDevice device)
        {
            DeviceAudioSource newAudioSource = DeviceAudioSource.Create(device);
            CurrentAudioSource!.RegisterAudioSource(newAudioSource);

            CurrentMicrophoneSource?.Dispose();
            CurrentMicrophoneSource = newAudioSource;
        }
        public void SetPlayerDevice(MMDevice device) => AudioPlayer.Instance.SetOutput(device);

        public void PlaySoundFile(Uri file, bool altMode)
        {
            if (CurrentSoundboardSource != null && !CurrentSoundboardSource.Finished)
                CurrentSoundboardSource.Dispose();

            CurrentSoundboardSource = FileAudioSource.Create(file.IsFile ? file.LocalPath : file.AbsoluteUri);

            bool apply = MainWindow.Instance!.PART_Soundboard.ApplyEffects;
            if (altMode) apply = !apply;

            if (apply)
                CurrentAudioSource!.RegisterAudioSource(CurrentSoundboardSource);
            else AudioPlayer.Instance.AudioPool.RegisterAudioSource(CurrentSoundboardSource!);
        }
    }
}
