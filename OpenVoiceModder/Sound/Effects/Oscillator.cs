using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.Sound.Effects.Utils;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class Oscillator : LFO
    {
        public override string Name => "Oscillator";
        public Oscillator() : base(new IParameter[0])
        {

        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            l *= Next();
            r *= Next();
        }
    }
}
