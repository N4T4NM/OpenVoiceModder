using NAudio.Wave;
using OpenVoiceModder.Sound.Effects.Parameters;
using System;
using System.Collections.Generic;

namespace OpenVoiceModder.Sound.Effects
{
    public abstract class Effect
    {
        protected readonly Dictionary<string, IParameter> _parameters = new();

        public abstract string Name { get; }
        public IEnumerable<IParameter> Parameters => _parameters.Values;

        public bool Initialized { get; protected set; }
        public bool Enabled { get; set; } = true;
        public WaveFormat? Format { get; protected set; }

        public virtual void Initialize(WaveFormat format)
        {
            Format = format;
            Initialized = true;
        }
        public virtual void Prepare(int count) { }
        public abstract void ApplyEffect(ref float l, ref float r);

        protected IParameter RegisterParameter(IParameter p)
        {
            _parameters.Add(p.Name.ToLower(), p);
            return p;
        }
        protected void RegisterParameters(params IParameter[] p)
        {
            foreach (IParameter p2 in p)
                RegisterParameter(p2);
        }

        public bool HasParameter(string parameter) => _parameters.ContainsKey(parameter);
        public T Get<T>(string parameter) where T : IParameter
        {
            return (T)_parameters[parameter];
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PublicEffectAttribute : Attribute
    {

    }
}
