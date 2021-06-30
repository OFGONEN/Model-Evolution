/* Created by and for usage of FF Studios (2021). */

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