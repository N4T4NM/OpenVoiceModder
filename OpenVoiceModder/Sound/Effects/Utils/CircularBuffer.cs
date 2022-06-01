using System;

namespace OpenVoiceModder.Sound.Effects.Utils
{
    public abstract class CircularBuffer : Effect
    {
        public override string Name => throw new NotImplementedException();

        protected float[] lBuffer = new float[0];
        protected float[] rBuffer = new float[0];

        protected int rPosition, wPosition;

        protected void InitializeBuffer(long size)
        {
            lBuffer = new float[size];
            rBuffer = new float[size];

            rPosition = 0;
            wPosition = 0;
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            throw new NotImplementedException();
        }

        public virtual void Read(out float l, out float r)
        {
            l = lBuffer[rPosition];
            r = rBuffer[rPosition++];

            if (rPosition >= lBuffer.Length)
                rPosition = 0;
        }
        public virtual void Write(float l, float r)
        {
            lBuffer[wPosition] = l;
            rBuffer[wPosition++] = r;

            if (wPosition >= lBuffer.Length)
                wPosition = 0;
        }
    }
}
