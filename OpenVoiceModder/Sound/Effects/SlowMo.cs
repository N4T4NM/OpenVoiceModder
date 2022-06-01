using NAudio.Wave;
using OpenVoiceModder.Sound.Effects.Parameters;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class SlowMo : Effect
    {
        public override string Name => "Slow Mo";
        public SlowMo()
        {
            RegisterParameter(new TrackbarParameter("Intensity", "%", 1, 100, 1, 1, "0"))
                .AddParameterHandler<TrackbarParameter>(OnUpdated);
        }

        float[] lBuffer = new float[0];
        float[] rBuffer = new float[0];
        int wPos;
        float rPos, step;

        public override void Initialize(WaveFormat format)
        {
            base.Initialize(format);
            OnUpdated(Get<TrackbarParameter>("intensity"));
            //float scale = Get<TrackbarParameter>("intensity");
            //int newSize = (int)(scale * format.SampleRate);

            //lBuffer = new float[newSize];
            //rBuffer = new float[newSize];

            //step = (100 - scale) / 101f;
        }

        private void OnUpdated(TrackbarParameter sender)
        {
            float scale = sender;
            int newSize = (int)(scale * Format!.SampleRate);

            lBuffer = new float[newSize];
            rBuffer = new float[newSize];

            rPos = 0; wPos = 0; step = (100 - scale) / 101f;
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            float outL = lBuffer[(int)rPos];
            float outR = rBuffer[(int)rPos];

            rPos += step;
            if (rPos >= lBuffer.Length) rPos = rPos - lBuffer.Length;

            lBuffer[wPos] = l;
            rBuffer[wPos++] = r;

            if (wPos >= lBuffer.Length) wPos = 0;

            l = outL;
            r = outR;
        }

    }
}
