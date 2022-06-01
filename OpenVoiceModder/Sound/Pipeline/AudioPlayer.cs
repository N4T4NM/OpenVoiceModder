using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace OpenVoiceModder.Sound
{
    public class AudioPlayer
    {
        public static AudioPlayer Instance { get; } = new();
        private AudioPlayer() { }

        public AudioPool AudioPool { get; } = new();

        public MMDevice? OutputDevice { get; private set; }
        public WasapiOut? Output { get; private set; }

        void DisposeCurrentOutput()
        {
            Output?.Stop();
            Output?.Dispose();
            Output = null;

            OutputDevice?.Dispose();
        }

        public void SetOutput(MMDevice outputDevice)
        {
            DisposeCurrentOutput();

            OutputDevice = outputDevice;

            Output = new(OutputDevice, AudioClientShareMode.Shared, true, 5);
            Output.Init(AudioPool);

            Output.Play();
        }

        public void Destroy()
        {
            DisposeCurrentOutput();
            AudioPool.Dispose();
        }
    }
}
