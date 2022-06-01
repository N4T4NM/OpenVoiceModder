using OpenVoiceModder.Sound.Effects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenVoiceModder.Sound.Utils
{
    public static class Reflection
    {
        public static IReadOnlyList<Type> PublicEffects { get; } = GetEffects();

        static IReadOnlyList<Type> GetEffects()
        {
            List<Type> effects = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAssignableTo(typeof(Effect)) && type != typeof(Effect) &&
                    type.GetCustomAttribute<PublicEffectAttribute>() != null)
                    effects.Add(type);
            }

            return effects;
        }
    }
}
