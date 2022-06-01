using NAudio.Wave;
using System;

namespace OpenVoiceModder.Sound
{
    public class AudioSource : ISampleProvider, IDisposable
    {
        public ISampleProvider Source { get; init; }
        public IWaveProvider WaveSource { get; init; }
        public WaveFormat WaveFormat => Source.WaveFormat;
        public bool IsDisposed { get; protected set; }

        public delegate void AudioSourceRemovedEvent(AudioSource sender);
        public event AudioSourceRemovedEvent? AudioSourceRemoved;

        public bool Enabled { get; set; }

        public AudioSource(ISampleProvider source)
        {
            Source = source;
            WaveSource = this.ToWaveProvider();
        }

        void ClearBuffer(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
                buffer[i] = 0;
        }

        public virtual int Read(float[] buffer, int offset, int count)
        {
            if (IsDisposed) return 0;
            try
            {
                int sz = Source.Read(buffer, offset, count);
                if (!Enabled)
                    ClearBuffer(buffer, offset, count);
                return sz;
            } catch(Exception ex)
            {
                return 0;
            }
        }
        public virtual void Dispose()
        {
            IsDisposed = true;
            AudioSourceRemoved?.Invoke(this);
        }

        public virtual void Init(AudioPool pool)
        {
            pool.PoolDisposed += Dispose;
        }
    }
}
