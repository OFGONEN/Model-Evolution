/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEditor;
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
		public GameEvent[] fireTheseOnComplete;
		public bool hasDelay_beforeEvents;
		[ ShowIf( "hasDelay_beforeEvents" ) ] public float delayAmount_beforeEvents;
#endregion

#region Fields (Private)
		private Vector3 startPosition;
		private Vector3 targetPosition;
		private Tween tween;
		private float Duration => Mathf.Abs( deltaPosition.magnitude / velocity );
#endregion

#region Properties (Public)
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }
		
		public Tween Tween => tween;
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
		
		[ Button() ]
		public void PlayBackwards()
		{
			if( tween == null )
				CreateAndStartTween( true /* reversed. */ );
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
			if( hasDelay_beforeEvents )
				DOVirtual.DelayedCall( delayAmount, Play );
			else
				Play();
		}

		private void CreateAndStartTween( bool isReversed = false )
		{
			if( movementMode == MovementMode.Local )
				tween = transform.DOLocalMove( isReversed ? -deltaPosition : deltaPosition, Duration );
			else
				tween = transform.DOMove( isReversed ? -deltaPosition : deltaPosition, Duration );

			tween
				.SetRelative()
				.SetLoops( loop ? -1 : 0, loopType )
				.SetEase( easing )
				.OnComplete( TweenComplete );

#if UNITY_EDITOR
			tween.SetId( name + "_ff_rotation_tween" );
#endif
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