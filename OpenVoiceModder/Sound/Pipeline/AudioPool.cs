using NAudio.Wave;
using System;
using System.Diagnostics;

namespace OpenVoiceModder.Sound
{
    public class AudioPool : IWaveProvider, IDisposable
    {
        public WaveFormat WaveFormat => _mixer.WaveFormat;
        readonly MixingWaveProvider32 _mixer = new();

        public delegate void PoolDisposedEvent();
        public event PoolDisposedEvent? PoolDisposed;

        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return _mixer.Read(buffer, offset, count);
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return 0;
            }
        }

        public void RegisterAudioSource(AudioSource src, bool autoEnable=true)
        {
            src.AudioSourceRemoved += OnSourceRemoved;
            src.Init(this);

            _mixer.AddInputStream(src.WaveSource);
            src.Enabled = autoEnable;         
        }
        private void OnSourceRemoved(AudioSource sender)
        {
            if (sender.Enabled)
                _mixer.RemoveInputStream(sender.WaveSource);
            sender.AudioSourceRemoved -= OnSourceRemoved;
        }

        public void Dispose() => PoolDisposed?.Invoke();
    }
}
