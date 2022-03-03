/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class ScaleTween : MonoBehaviour
	{
#region Fields (Inspector Interface)
	 [ Title( "Parameters" ) ]
    	public Vector3 targetScale;
		public float duration;

	[ Title( "Start Options" ) ]
    	public bool playOnStart;
		public bool hasDelay;
		[ ShowIf( "hasDelay" ) ] public float delayAmount;
		
	[ Title( "Tween" ) ]
    	[ DisableIf( "IsPlaying" ) ] public bool loop;
		[ ShowIf( "loop" ) ] public LoopType loopType = LoopType.Restart;
		Ease easing = Ease.Linear;
		
	[ Title( "Event Flow" ) ]
    	[ SerializeField ] private MultipleEventListenerDelegateResponse triggeringEvents;
		public GameEvent[] events_fireOnComplete;
		public UnityEvent unityEvent_fireOnComplete;
#endregion

#region Fields (Inspector Interface)
		private Vector3 startScale;
		private Tween tween;
#endregion

#region Properties
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
			
            startScale = transform.localScale;
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
		
		[ Button() ]
		public void Rewind()
		{
			if( tween == null )
				return;

			tween.Rewind();

			IsPlaying = false;
		}
#endregion

#region Implementation
		private void EventResponse()
		{
			DOVirtual.DelayedCall( delayAmount, Play );
		}

		private void CreateAndStartTween()
		{
			tween = transform.DOScale( targetScale, duration );

			tween.SetEase( easing )
                 .SetLoops( loop ? -1 : 0, loopType )
                 .OnComplete( TweenComplete );
        }
		
        private void TweenComplete()
        {
			IsPlaying = false;

			KillTween();

            for( var i = 0; i < events_fireOnComplete.Length; i++ )
				events_fireOnComplete[ i ].Raise();

			unityEvent_fireOnComplete.Invoke();
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
#endif
#endregion
	}
}