using NAudio.Wave;
using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.Sound.Effects.Utils;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class DelayEffect : CircularBuffer
    {
        public override string Name => "Delay";

        public DelayEffect()
        {
            RegisterParameter(new TrackbarParameter("Delay Time", "s", 1f, 5f, 1f, 5f, "0"))
                .AddParameterHandler<TrackbarParameter>(OnDelayUpdated);
        }

        private void OnDelayUpdated(TrackbarParameter sender)
        {
            InitializeBuffer((long)(Format!.SampleRate * sender));
            rPosition = (int)sender;
        }

        public override void Initialize(WaveFormat format)
        {
            base.Initialize(format);
            OnDelayUpdated(Get<TrackbarParameter>("delay time"));
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            Write(l, r);
            Read(out l, out r);
        }

        public override void Read(out float l, out float r)
        {
            l = lBuffer[rPosition];
            r = rBuffer[rPosition++];

            if (rPosition >= lBuffer.Length)
                rPosition = (int)Get<TrackbarParameter>("delay time");
        }
    }
}
