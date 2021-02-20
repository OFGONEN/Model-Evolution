namespace FFStudio
{
    [System.Serializable]
    public class EventListenerDelegateResponse : EventListener
    {
        public delegate void Response();
        public Response response;
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
            response();
        }
    }
}