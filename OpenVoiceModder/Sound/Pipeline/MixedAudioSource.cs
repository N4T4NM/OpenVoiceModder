using NAudio.Wave;
using OpenVoiceModder.Sound.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenVoiceModder.Sound
{
    public class MixedAudioSource : AudioSource
    {
        public MixingWaveProvider32 Mixer { get; init; }
        public bool ApplyEffects { get; set; } = true;
        public List<Effect> Effects { get; } = new();

        public MixedAudioSource(MixingWaveProvider32 mixer) : base(mixer.ToSampleProvider())
        {
            Mixer = mixer;
        }
        public void RegisterAudioSource(AudioSource src, bool autoEnable = true)
        {
            src.AudioSourceRemoved += OnSourceRemoved;
            src.Init(AudioPlayer.Instance.AudioPool);

            Mixer.AddInputStream(src.WaveSource);
            src.Enabled = autoEnable;
        }

        private void OnSourceRemoved(AudioSource sender)
        {
            if (sender.Enabled)
                Mixer.RemoveInputStream(sender.WaveSource);
            sender.AudioSourceRemoved -= OnSourceRemoved;
        }
        public override int Read(float[] buffer, int offset, int count)
        {
            int sz = base.Read(buffer, offset, count);
            if (!ApplyEffects) return sz;

            try
            {
                foreach (Effect effect in Effects)
                {
                    if (!effect.Initialized)
                        effect.Initialize(WaveFormat);

                    if (effect.Enabled)
                        effect.Prepare(count);
                }
            } catch(Exception ex) { }

            for (int i = 0; i < count; i++)
            {
                float l = buffer[offset];
                float r = l;
                if (WaveFormat.Channels == 2)
                {
                    r = buffer[offset + 1];
                    i++;
                }

                try
                {
                    foreach (Effect effect in Effects)
                        if (effect.Enabled)
                            effect.ApplyEffect(ref l, ref r);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                buffer[offset++] = l;
                if (WaveFormat.Channels == 2)
                    buffer[offset++] = r;
            }

            return sz;
        }
        public override void Dispose() => base.Dispose();
    }
}
