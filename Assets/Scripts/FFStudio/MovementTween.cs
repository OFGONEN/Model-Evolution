/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using DG.Tweening;
using UnityEditor;

namespace FFStudio
{
	public class MovementTween : MonoBehaviour
	{
#region Fields

		public enum MovementMode { Local, World }

		public Vector3 deltaPosition;
		public float velocity;

		public bool playOnStart;

        [ ShowIf( "playOnStart" ) ]
		public bool hasDelay;

        [ ShowIf( "hasDelay" ) ]
		public float delayAmount;

		[ DisableIf( "IsPlaying" ) ]
        public bool loop;

        [ ShowIf( "loop" ) ]
        public LoopType loopType = LoopType.Restart;

		public MovementMode movementMode;
        public Ease easing = Ease.Linear;
        public GameEvent[] fireTheseOnComplete;
        
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }

		/* Private Fields */

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

#region Unity API

		private void OnEnable()
		{
			Play();
		}
		
		private void OnDisable()
		{
			Pause();
		}

		private void Awake()
		{
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
		[Button()]
		public void Play()
		{
			if( tween == null )
				CreateAndStartTween();
			else
				tween.Play();

			IsPlaying = true;
		}

		[Button(), EnableIf( "IsPlaying" )]
		public void Pause()
		{
			if( tween == null )
				return;

			tween.Pause();

			IsPlaying = false;
		}

		[Button(), EnableIf( "IsPlaying" )]
		public void Stop()
		{
			if( tween == null )
				return;

			tween.Rewind();

			IsPlaying = false;
		}

		[Button(), EnableIf( "IsPlaying" )]
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
		private void OnDrawGizmos()
		{
			Vector3 startPos;

			Handles.color = Color.green;

			if(Application.isPlaying)
				startPos = startPosition;
			else
				startPos = transform.position;

			Handles.DrawSolidDisc( startPos, Vector3.forward, 0.25f );
			Handles.DrawSolidDisc( startPos + deltaPosition, Vector3.forward, 0.25f );
			Handles.DrawDottedLine( startPos, startPos + deltaPosition, 10 );

			Handles.color = Color.blue;
			Handles.DrawSolidDisc( transform.position, Vector3.forward, 0.125f );
		}
#endregion
	}
}