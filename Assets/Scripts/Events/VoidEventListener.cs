using UnityEngine;
using UnityEngine.Events;

namespace MausTemple
{
    public class VoidEventListener : MonoBehaviour
    {
        [SerializeField] VoidEventChannelSO _channel = default;
        public UnityEvent OnEventRaised;

        void OnEnable()
        {
            if (_channel != null)
                _channel.OnEventRaised += Respond;
        }

        void OnDisable()
        {
            if (_channel != null)
                _channel.OnEventRaised -= Respond;
        }

        void Respond()
        {
            OnEventRaised?.Invoke();
        }
    }
}
