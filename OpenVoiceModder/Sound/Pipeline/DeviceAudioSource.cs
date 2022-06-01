using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace OpenVoiceModder.Sound
{
    public class DeviceAudioSource : AudioSource
    {
        public WasapiCapture Capture { get; init; }
        public MMDevice Device { get; init; }
        readonly BufferedWaveProvider _waveProvider;

        private DeviceAudioSource(MMDevice device, WasapiCapture capture, BufferedWaveProvider provider) : base(provider.ToSampleProvider())
        {
            Capture = capture;
            Device = device;

            _waveProvider = provider;
            _waveProvider.DiscardOnBufferOverflow = true;

            Capture.DataAvailable += OnDataReceived;
            Capture.StartRecording();
        }

        private void OnDataReceived(object? sender, WaveInEventArgs e)
        {
            if (!Enabled)
                _waveProvider.ClearBuffer();
            else _waveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        public override void Dispose()
        {
            base.Dispose();

            _waveProvider.ClearBuffer();
            Capture.StopRecording();

            Capture.Dispose();
            Device.Dispose();
        }

        public static DeviceAudioSource Create(MMDevice inputDevice)
        {
            WasapiCapture capture = new(inputDevice, true, 5);
            return new(inputDevice, capture, new(capture.WaveFormat));
        }

        public static DeviceAudioSource CreateLoopback(MMDevice loopbackDevice)
        {
            WasapiLoopbackCapture capture = new(loopbackDevice);
            return new(loopbackDevice, capture, new(capture.WaveFormat));
        }
    }
}
