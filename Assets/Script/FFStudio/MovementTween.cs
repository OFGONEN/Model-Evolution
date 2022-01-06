/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using DG.Tweening;

namespace FFStudio
{
	public class MovementTween : MonoBehaviour
	{
		public enum MovementMode { Local, World }
		
#region Fields (Inspector Interface)

	[ Header( "Parameters" ) ]
	
		public Vector3 deltaPosition;
		public float velocity;

		public MovementMode movementMode;

	[ Header( "Start" ) ]
	
		public bool playOnStart;
		public bool hasDelay;
        [ ShowIf( "hasDelay" ) ] public float delayAmount;
		
	[ Header( "Tween" ) ]

		[ DisableIf( "IsPlaying" ) ] public bool loop;
        [ ShowIf( "loop" ) ] public LoopType loopType = LoopType.Restart;
		
        public Ease easing = Ease.Linear;
		
	[ Header( "Event Flow" ) ]
	
        [ SerializeField ] private MultipleEventListenerDelegateResponse triggeringEvents;
	
        public GameEvent[] fireTheseOnComplete;
#endregion

#region Fields (Private)
		private Vector3 startPosition;
		private Vector3 targetPosition;
		private Tween tween;
		private float Duration 
		{
			get 
			{
				return Mathf.Abs( deltaPosition.magnitude / velocity );
			}
		}
#endregion

#region Properties (Public)
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }
#endregion

#region Unity API
		private void OnEnable()
		{
			triggeringEvents.OnEnable();
		}
		
		private void OnDisable()
		{
			triggeringEvents.OnDisable();
		}

		private void Awake()
		{
			triggeringEvents.response = EventResponse;

			if( movementMode == MovementMode.Local )
				startPosition = transform.localPosition;
			else
				startPosition = transform.position;
		}
		
        private void Start()
        {
            if( !enabled )
                return;

            if( playOnStart )
            {
                if( hasDelay )
					DOVirtual.DelayedCall( delayAmount, Play );
                else
					Play();
			}
        }
        
        private void OnDestroy()
        {
            KillTween();
        }

#endregion

#region API
		[ Button() ]
		public void Play()
		{
			if( tween == null )
				CreateAndStartTween();
			else
				tween.Play();

			IsPlaying = true;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Pause()
		{
			if( tween == null )
				return;

			tween.Pause();

			IsPlaying = false;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Stop()
		{
			if( tween == null )
				return;

			tween.Rewind();

			IsPlaying = false;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Restart()
		{
			if( tween == null )
				Play();
			else
			{
				tween.Restart();

				IsPlaying = true;
			}
		}
#endregion

#region Implementation
		private void EventResponse()
		{
			DOVirtual.DelayedCall( delayAmount, Play );
		}

		private void CreateAndStartTween()
		{
			if( movementMode == MovementMode.Local )
				tween = transform.DOLocalMove( deltaPosition, Duration );
			else
				tween = transform.DOMove( deltaPosition, Duration );

			tween.SetRelative()
				 .SetEase( easing )
                 .SetLoops( loop ? -1 : 0, loopType )
                 .OnComplete( TweenComplete );
        }
        private void TweenComplete()
        {
			IsPlaying = false;

			KillTween();

            for( var i = 0; i < fireTheseOnComplete.Length; i++ )
				fireTheseOnComplete[ i ].Raise();
		}

		private void KillTween()
		{
			IsPlaying = false;

			tween.Kill();
			tween = null;
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Vector3 startPos = startPosition;

			Handles.color = Color.green;

			if( Application.isPlaying )
			{
				if( movementMode == MovementMode.Local && transform.parent != null )
					startPos = transform.parent.TransformPoint( startPosition );
			}
			else
				startPos = transform.position;

			Handles.DrawSolidDisc( startPos, Vector3.up, 0.25f );
			Handles.DrawSolidDisc( startPos + deltaPosition, Vector3.up, 0.25f );
			Handles.DrawDottedLine( startPos, startPos + deltaPosition, 10 );

			Handles.color = Color.blue;
			Handles.DrawSolidDisc( transform.position, Vector3.up, 0.125f );
		}
#endif
#endregion
	}
}