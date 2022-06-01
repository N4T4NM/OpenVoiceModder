using NAudio.Wave;
using OpenVoiceModder.Sound.Effects.Parameters;
using System;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class EchoEffect : Effect
    {
        public override string Name => "Echo Effect";

        public EchoEffect()
        {
            RegisterParameter(new TrackbarParameter("Decay", "dB", 0.1f, 1f, 0.1f, 0.5f));
            RegisterParameter(new TrackbarParameter("Tempo", "", 50, 200, 1, 103, "00"));
        }

        public override void Initialize(WaveFormat format)
        {
            base.Initialize(format);
            lBuffer = new float[Format!.SampleRate * 5];
            rBuffer = new float[lBuffer.Length];
        }

        int pos;
        float[] lBuffer = new float[0];
        float[] rBuffer = new float[0];

        public override void ApplyEffect(ref float l, ref float r)
        {
            float spb = 1 / (Get<TrackbarParameter>("tempo") / 60);
            float sdl = (float)Math.Floor(0.5f * spb * Format!.SampleRate);

            lBuffer[pos] = l;
            rBuffer[pos] = r;

            if (pos - sdl < 0)
            {
                l = lBuffer[pos];
                r = rBuffer[pos];
            }
            else
            {
                l = lBuffer[pos] + (1 - Get<TrackbarParameter>("decay")) * lBuffer[(int)(pos - sdl)];
                r = rBuffer[pos] + (1 - Get<TrackbarParameter>("decay")) * rBuffer[(int)(pos - sdl)];

                lBuffer[pos] = l;
                rBuffer[pos] = r;
            }

            pos++;
            if (pos >= lBuffer.Length) pos = (int)sdl;
        }
    }
}
