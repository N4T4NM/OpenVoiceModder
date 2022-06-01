using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.Sound.Effects.Utils;
using System;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class BandPass : IIRFilter
    {
        public override string Name => "BandPass Filter";
        public BandPass()
        {
            RegisterParameter(new TrackbarParameter("Frequency", "Hz", 100, 4110, 1, 100, "0"))
                .AddParameterHandler<TrackbarParameter>(OnFrequencyChanged);

            RegisterParameter(new TrackbarParameter("Bandwidth", "Hz", 100, 2000, 1, 1000, "0"))
                .AddParameterHandler<TrackbarParameter>(OnFrequencyChanged);
        }

        private void OnFrequencyChanged(TrackbarParameter parameter) => CalculateCoefficients();

        protected override void CalculateCoefficients()
        {
            float R = 1 - 3 * (Get<TrackbarParameter>("bandwidth") / Format!.SampleRate);
            float fracFreq = Get<TrackbarParameter>("frequency") / Format!.SampleRate;

            float T = 2 * MathF.Cos(2 * MathF.PI * fracFreq);
            float K = (1 - R * T + R * R) / (2 - T);

            aCoefficients = new float[] { 1 - K, (K - R) * T, R * R - K };
            bCoefficients = new float[] { R * T, -R * R };

            iBuffer = new float[aCoefficients.Length];
            oBuffer = new float[bCoefficients.Length];
        }
    }
}
