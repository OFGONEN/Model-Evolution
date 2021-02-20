namespace FFStudio
{
    public abstract class EventListener
    {
        public GameEvent gameEvent;

        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void OnEventRaised();
    }
}