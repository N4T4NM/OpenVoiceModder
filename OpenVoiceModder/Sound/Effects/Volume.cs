using OpenVoiceModder.Sound.Effects.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class Volume : Effect
    {
        public override string Name => "Volume";
        public Volume()
        {
            RegisterParameter(new TrackbarParameter("Boost", "dB", 1, 20, 1f, 1f));
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            float boost = Get<TrackbarParameter>("boost");
            l *= boost;
            r *= boost;
        }
    }
}
