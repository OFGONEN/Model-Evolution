/* Created by and for usage of FF Studios (2021). */

namespace FFStudio
{
    [ System.Serializable ]
	public class MultipleEventListenerDelegateResponse : EventListener
	{
		public GameEvent[] gameEvents;

		public delegate void Response();
        public Response response;
        
        public override void OnEnable()
        {
			for( int i = 0; i < gameEvents.Length; i++ )
				gameEvents[ i ].RegisterListener( this );
        }
        
        public override void OnDisable()
        {
			for( int i = 0; i < gameEvents.Length; i++ )
				gameEvents[ i ].UnregisterListener( this );
        }

        public override void OnEventRaised()
        {
            response();
        }
	}
}