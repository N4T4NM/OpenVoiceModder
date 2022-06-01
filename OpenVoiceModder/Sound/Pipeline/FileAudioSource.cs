using NAudio.Wave;
using System;
using System.Diagnostics;

namespace OpenVoiceModder.Sound
{
    public class FileAudioSource : AudioSource
    {
        readonly MediaFoundationReader _reader;
        readonly MediaFoundationResampler _resampler;

        public string FilePath { get; init; }
        public long Length => _reader.Length;
        public long Position => _reader.Position;
        public bool Finished { get; protected set; }

        private FileAudioSource(string path, MediaFoundationReader reader, MediaFoundationResampler resampler) : base(resampler.ToSampleProvider())
        {
            _reader = reader;
            _resampler = resampler;

            FilePath = path;
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            if (Position >= Length)
                Dispose();

            int sz = base.Read(buffer, offset, count);
            return sz;
        }
        public override void Dispose()
        {
            Finished = true;
            base.Dispose();

            _resampler.Dispose();
            _reader.Dispose();
        }

        public static FileAudioSource Create(string path)
        {
            MediaFoundationReader reader = new(path);
            MediaFoundationResampler resampler = new(reader, AudioPlayer.Instance.AudioPool.WaveFormat);

            return new(path, reader, resampler);
        }
    }
}
