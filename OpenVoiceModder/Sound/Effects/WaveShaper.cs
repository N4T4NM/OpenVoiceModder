using OpenVoiceModder.Sound.Effects.Parameters;
using System;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class WaveShaper : Effect
    {
        public override string Name => "Wave Shaper";

        public WaveShaper()
        {
            RegisterParameter(new TrackbarParameter("Distortion", "%", 1, 100, 1, 1, "0"));
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            float l1 = l * (MathF.Abs(l) + Get<TrackbarParameter>("distortion"));
            float l2 = MathF.Pow(2, l) + (Get<TrackbarParameter>("distortion") - 1) * MathF.Abs(l) + 1;

            float r1 = r * (MathF.Abs(r) + Get<TrackbarParameter>("distortion"));
            float r2 = MathF.Pow(2, r) + (Get<TrackbarParameter>("distortion") - 1) * MathF.Abs(r) + 1;

            l = l1 / l2;
            r = r1 / r2;
        }
    }
}
