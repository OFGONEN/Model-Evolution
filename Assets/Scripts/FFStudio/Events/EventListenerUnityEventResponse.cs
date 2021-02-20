using UnityEngine.Events;

namespace FFStudio
{
    [System.Serializable]
    public class EventListenerUnityEventResponse : EventListener
    {
        public UnityEvent response;
        public override void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }
        public override void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public override void OnEventRaised()
        {
            response.Invoke();
        }
    }
}