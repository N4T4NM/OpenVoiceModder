using OpenVoiceModder.Sound.Effects.Parameters;
using System;

namespace OpenVoiceModder.Sound.Effects.Utils
{
    public class LFO : Effect
    {
        public override string Name => "Low Frequency Oscilator";

        public LFO(IParameter[] userParameters)
        {
            foreach (IParameter parameter in userParameters)
                RegisterParameter(parameter);

            RegisterParameter(new TrackbarParameter("Frequency", "Hz", 0, 25, 0.1f, 7.60f));
            RegisterParameter(new TrackbarParameter("Delay", "s", 0, 5, 1, 1))
                .AddParameterHandler<TrackbarParameter>(OnDelayUpdate);
        }

        protected void OnDelayUpdate(TrackbarParameter sender)
        {
            float newDelay = MathF.Round(sender);
            if (delay - newDelay >= 1f)
                Reset();

            delay = newDelay;
        }

        protected virtual void Reset() { }

        protected float phase;
        protected float delay;

        public override void ApplyEffect(ref float l, ref float r)
        {
            throw new NotImplementedException();
        }

        protected float Next()
        {
            float dp = 2 * MathF.PI * Get<TrackbarParameter>("frequency") / Format!.SampleRate;
            float value = MathF.Sin(phase);

            value = (value + 1 * .5f);

            phase += dp;
            while (phase > 2 * MathF.PI)
                phase -= 2 * MathF.PI;

            return value;
        }
    }
}
