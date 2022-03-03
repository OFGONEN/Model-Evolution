/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
using Shapes;

namespace FFStudio
{
	public class MovementTween : MonoBehaviour
	{
		public enum MovementMode { Local, World }
		
#region Fields (Inspector Interface)

	[ Title( "Parameters" ) ]
		public Vector3 deltaPosition;
		public float velocity;
		public MovementMode movementMode;

	[ Title( "Start Options" ) ]
		public bool playOnStart;
		public bool hasDelay;
        [ ShowIf( "hasDelay" ) ] public float delayAmount;
		
	[ Title( "Tween" ) ]
		[ DisableIf( "IsPlaying" ) ] public bool loop;
        [ ShowIf( "loop" ) ] public LoopType loopType = LoopType.Restart;
		public Ease easing = Ease.Linear;
		
	[ Title( "Event Flow" ) ]
		[ SerializeField ] private MultipleEventListenerDelegateResponse triggeringEvents;
		public GameEvent[] events_firedOnComplete;
		public UnityEvent unityEvents_firedOnComplete;
		public bool hasDelay_beforeEvents;
		[ ShowIf( "hasDelay_beforeEvents" ) ] public float delayAmount_beforeEvents;
#endregion

#region Fields (Private)
		private RecycledTween recycledTween = new RecycledTween();
		private float Duration => Mathf.Abs( deltaPosition.magnitude / velocity );
		
		private Vector3 startPosition;
		private Vector3 targetPosition;
#endregion

#region Properties (Public)
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }
		
		public Tween Tween => recycledTween.Tween;
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
			if( recycledTween.Tween == null )
				CreateAndStartTween();
			else
				recycledTween.Tween.Play();

			IsPlaying = true;
		}
		
		[ Button() ]
		public void PlayBackwards()
		{
			if( recycledTween.Tween == null )
				CreateAndStartTween( true /* reversed. */ );
			else
				recycledTween.Tween.Play();

			IsPlaying = true;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Pause()
		{
			if( recycledTween.Tween == null )
				return;

			recycledTween.Tween.Pause();

			IsPlaying = false;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Stop()
		{
			if( recycledTween.Tween == null )
				return;

			recycledTween.Tween.Rewind();

			IsPlaying = false;
		}

		[ Button(), EnableIf( "IsPlaying" ) ]
		public void Restart()
		{
			if( recycledTween.Tween == null )
				Play();
			else
			{
				recycledTween.Tween.Restart();

				IsPlaying = true;
			}
		}
#endregion

#region Implementation
		private void EventResponse()
		{
			if( hasDelay_beforeEvents )
				DOVirtual.DelayedCall( delayAmount, Play );
			else
				Play();
		}

		private void CreateAndStartTween( bool isReversed = false )
		{
			if( movementMode == MovementMode.Local )
				recycledTween.Recycle( transform.DOLocalMove( isReversed ? -deltaPosition : deltaPosition, Duration ), OnTweenComplete );
			else
				recycledTween.Recycle( transform.DOMove( isReversed ? -deltaPosition : deltaPosition, Duration ), OnTweenComplete );

			recycledTween.Tween
				.SetRelative()
				.SetLoops( loop ? -1 : 0, loopType )
				.SetEase( easing );

#if UNITY_EDITOR
			recycledTween.Tween.SetId( name + "_ff_movement_tween" );
#endif
		}
		
        private void OnTweenComplete()
        {
			IsPlaying = false;

            for( var i = 0; i < events_firedOnComplete.Length; i++ )
				events_firedOnComplete[ i ].Raise();

			unityEvents_firedOnComplete.Invoke();
		}

		private void KillTween()
		{
			IsPlaying = false;

			recycledTween.Kill();
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Vector3 startPos = startPosition;

			if( Application.isPlaying )
			{
				if( movementMode == MovementMode.Local && transform.parent != null )
					startPos = transform.parent.TransformPoint( startPosition );
			}
			else
				startPos = transform.position;

			Color color = new Color( 1.0f, 0.75f, 0.0f );
			Draw.LineDashed( startPos, startPos + deltaPosition, new DashStyle( 1 ), 0.125f, LineEndCap.None, color );
			var direction = deltaPosition.normalized;
			var deltaMagnitude = deltaPosition.magnitude;
			var coneLength = 0.2f;
			var conePos = Vector3.Lerp( startPos, startPos + deltaPosition, 1.0f - coneLength / deltaMagnitude );
			Draw.Cone( conePos, deltaPosition.normalized, 0.2f, 0.2f, color );
		}
#endif
#endregion
	}
}