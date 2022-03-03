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
		private RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
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
			if( recycledTween.Tween == null )
				CreateAndStartTween();
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
		
		[ Button() ]
		public void Rewind()
		{
			if( recycledTween.Tween == null )
				return;

			recycledTween.Tween.Rewind();

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
			recycledTween.Recycle( transform.DOScale( targetScale, duration ), OnTweenComplete );

			recycledTween.Tween.SetEase( easing )
				 .SetLoops( loop ? -1 : 0, loopType );

#if UNITY_EDITOR
			recycledTween.Tween.SetId( name + "_ff_scale_tween" );
#endif
		}
		
        private void OnTweenComplete()
        {
			IsPlaying = false;

            for( var i = 0; i < events_fireOnComplete.Length; i++ )
				events_fireOnComplete[ i ].Raise();

			unityEvent_fireOnComplete.Invoke();
		}

		private void KillTween()
		{
			IsPlaying = false;

			recycledTween.Kill();
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
#endif
#endregion
	}
}