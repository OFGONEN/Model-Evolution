/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;

namespace FFStudio
{
    public class GuillotinTween : MonoBehaviour
    {
#region Fields
        public enum RotationMode { Local, World }
    [ Title( "Parameters" ) ]
		[ SuffixLabel( "Degrees (°)", true ) ] public float deltaAngle;
        [ SuffixLabel( "Degrees/Seconds (°/s)", true ), Min( 0 ) ] public float angularSpeedInDegrees;
        public RotationMode rotationMode;
        [ ValueDropdown( "VectorValues" ), LabelText( "Rotate Pendulum Around" ) ] public Vector3 rotationAxisMaskVector_Blade = Vector3.right;

    [ Title( "Start Options" ) ]
        public bool playOnStart;
        public bool hasDelay;
        [ ShowIf( "hasDelay" ) ] public float delayAmount;

        [ ValueDropdown( "VectorValues" ), LabelText( "Rotate Around" ) ] public Vector3 rotationAxisMaskVector = Vector3.right;

    [ Title( "Tween" ) ]
        public Ease easing = Ease.Linear;
        
    [ Title( "Event Flow" ) ]
        [ SerializeField ] private MultipleEventListenerDelegateResponse triggeringEvents;
        public GameEvent[] events_fireOnComplete;
		public UnityEvent unityEvent_fireOnComplete;
#endregion
        
#region Fields (Private)
        private Sequence sequence;
        private float Duration => Mathf.Abs( deltaAngle / angularSpeedInDegrees );

        private static IEnumerable VectorValues = new ValueDropdownList< Vector3 >()
        {
            { "X",   Vector3.right      },
            { "Y",   Vector3.up         },
            { "Z",   Vector3.forward    }
        };
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
			KillSequence();
		}
#endregion

#region API
        [ Button() ]
        public void Play()
        {
            if( sequence == null )
                CreateAndStartSequence();
            else
                sequence.Play();

            IsPlaying = true;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Pause()
        {
            if( sequence == null )
                return;

            sequence.Pause();

            IsPlaying = false;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Stop()
        {
            if( sequence == null )
                return;
                
            sequence.Rewind();

            IsPlaying = false;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Restart()
        {
            if( sequence == null )
                Play();
            else
            {
                sequence.Restart();

                IsPlaying = true;
            }
        }
#endregion

#region Implementation
        private void EventResponse()
        {
			DOVirtual.DelayedCall( delayAmount, Play );
		}

        private void CreateAndStartSequence()
        {
			/* Since we use SetRelative + RotateMode.FastBeyond360 combo, we need to specify a delta instead of end value. */

			sequence = DOTween.Sequence()
							.Append( transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration ).SetEase( easing ) )
							.Append( transform.DOLocalRotate( rotationAxisMaskVector_Blade * 180.0f, 0.25f ).SetEase( easing ) )
							.Append( transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration ).SetEase( easing ) )
							.Append( transform.DOLocalRotate( rotationAxisMaskVector_Blade * 180.0f, 0.25f ).SetEase( easing ) );

			sequence
				.SetRelative()
                .SetLoops( -1, LoopType.Restart )
                .OnComplete( OnSequenceComplete );
        }

        private void OnSequenceComplete()
        {
			IsPlaying = false;

			KillSequence();

            for( var i = 0; i < events_fireOnComplete.Length; i++ )
				events_fireOnComplete[ i ].Raise();

			unityEvent_fireOnComplete.Invoke();
		}

        private void KillSequence()
        {
            IsPlaying = false;
            
            sequence.Kill();
            sequence = null;
        }
#endregion
    }
}
