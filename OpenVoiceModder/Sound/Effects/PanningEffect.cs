using OpenVoiceModder.Sound.Effects.Parameters;

namespace OpenVoiceModder.Sound.Effects
{
    [PublicEffect]
    public class PanningEffect : Effect
    {
        public override string Name => "Panning";
        public PanningEffect()
        {
            RegisterParameter(new TrackbarParameter("Left", "%", 0, 100, 1, 50, "0"));
            RegisterParameter(new TrackbarParameter("Right", "%", 0, 100, 1, 50, "0"));
        }

        public override void ApplyEffect(ref float l, ref float r)
        {
            float leftPanning = Get<TrackbarParameter>("left");
            float rightPanning = Get<TrackbarParameter>("right");

            l *= leftPanning / 50;
            r *= rightPanning / 50;
        }
    }
}
