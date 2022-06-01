using System.Windows.Controls;

namespace OpenVoiceModder.Sound.Effects.Parameters
{
    public interface IParameter
    {
        public delegate void ParameterUpdatedEvent(IParameter parameter);
        public event ParameterUpdatedEvent? ParameterUpdated;

        public delegate void ParameterUpdatedHandler<T>(T parameter) where T : IParameter;

        public string Name { get; }
        public void AddParameterController(Panel target);

        public string BuildString();
        public void ParseString(string str);

        public void AddParameterHandler<T>(ParameterUpdatedHandler<T> handler) where T : IParameter
        {
            this.ParameterUpdated += (p) => handler.Invoke((T)p);
        }
    }
}
