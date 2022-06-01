using NAudio.Wave;
using System;

namespace OpenVoiceModder.Sound.Effects.Utils
{
    public abstract class IIRFilter : Effect
    {
        public override string Name => throw new NotImplementedException();

        public override void Initialize(WaveFormat format)
        {
            base.Initialize(format);
            CalculateCoefficients();
        }

        protected abstract void CalculateCoefficients();

        protected float[] aCoefficients = new float[0];
        protected float[] bCoefficients = new float[0];

        protected float[] iBuffer = new float[0];
        protected float[] oBuffer = new float[0];

        public override void ApplyEffect(ref float l, ref float r)
        {
            Array.Copy(iBuffer, 0, iBuffer, 1, iBuffer.Length - 1);
            iBuffer[0] = l;

            float y = 0;

            for (int i = 0; i < aCoefficients.Length; i++)
                y += aCoefficients[i] * iBuffer[i];

            for (int i = 0; i < bCoefficients.Length; i++)
                y += bCoefficients[i] * oBuffer[i];

            Array.Copy(oBuffer, 0, oBuffer, 1, oBuffer.Length - 1);
            oBuffer[0] = y;

            l = y;
            r = y;
        }
    }
}
