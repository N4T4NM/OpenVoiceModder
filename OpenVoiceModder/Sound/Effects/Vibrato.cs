using NAudio.Wave;
using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.Sound.Effects.Utils;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class Vibrato : LFO
    {
        public override string Name => "Vibrato";
        public Vibrato() : base(new IParameter[0]) { }

        int pos;
        float[] lBuffer = new float[0];
        float[] rBuffer = new float[0];

        public override void Initialize(WaveFormat format)
        {
            base.Initialize(format);
            OnDelayUpdate(Get<TrackbarParameter>("delay"));
        }

        public override void Prepare(int count)
        {
            base.Prepare(count);

            int size = (int)(delay * Format!.SampleRate);
            if (lBuffer.Length != size)
            {
                lBuffer = new float[size];
                rBuffer = new float[size];
            }
        }

        protected override void Reset() => pos = 0;

        public override void ApplyEffect(ref float l, ref float r)
        {
            if (lBuffer.Length == 0) return;

            float offset = (delay / 2) * (1 + Next() * (.1f / 10f)) * Format!.SampleRate;
            if (offset > lBuffer.Length)
                offset = lBuffer.Length;

            float readOffset = pos - offset;
            readOffset = readOffset >= 0 ? (readOffset < lBuffer.Length ? readOffset : readOffset - lBuffer.Length) :
                readOffset + lBuffer.Length;

            int rPos = (int)readOffset;
            if (rPos >= lBuffer.Length)
                rPos -= lBuffer.Length;

            float frac = readOffset - rPos;
            float nextL = (rPos != lBuffer.Length - 1 ? lBuffer[rPos + 1] : lBuffer[0]);
            float nextR = (rPos != rBuffer.Length - 1 ? rBuffer[rPos + 1] : rBuffer[0]);

            float outL = lBuffer[rPos] + frac * (nextL - lBuffer[rPos]);
            float outR = rBuffer[rPos] + frac * (nextR - rBuffer[rPos]);

            lBuffer[pos] = l;
            rBuffer[pos] = r;

            l = outL;
            r = outR;

            pos = pos != lBuffer.Length - 1 ? pos + 1 : 0;
        }
    }
}
