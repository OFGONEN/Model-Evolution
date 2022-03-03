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
        private RecycledSequence recycledSequence = new RecycledSequence();
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
        
        public Sequence Sequence => recycledSequence.Sequence;
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
            if( recycledSequence.Sequence == null )
                CreateAndStartSequence();
            else
                recycledSequence.Sequence.Play();

            IsPlaying = true;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Pause()
        {
            if( recycledSequence.Sequence == null )
                return;

            recycledSequence.Sequence.Pause();

            IsPlaying = false;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Stop()
        {
            if( recycledSequence.Sequence == null )
                return;
                
            recycledSequence.Sequence.Rewind();

            IsPlaying = false;
        }
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Restart()
        {
            if( recycledSequence.Sequence == null )
                Play();
            else
            {
                recycledSequence.Sequence.Restart();

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

			recycledSequence.Recycle( OnSequenceComplete )
                .Append( transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration ).SetEase( easing ) )
                .Append( transform.DOLocalRotate( rotationAxisMaskVector_Blade * 180.0f, 0.25f ).SetEase( easing ) )
                .Append( transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration ).SetEase( easing ) )
                .Append( transform.DOLocalRotate( rotationAxisMaskVector_Blade * 180.0f, 0.25f ).SetEase( easing ) );

			recycledSequence.Sequence
				.SetRelative()
				.SetLoops( -1, LoopType.Restart );
		}

        private void OnSequenceComplete()
        {
			IsPlaying = false;

            for( var i = 0; i < events_fireOnComplete.Length; i++ )
				events_fireOnComplete[ i ].Raise();

			unityEvent_fireOnComplete.Invoke();
		}

        private void KillSequence()
        {
            IsPlaying = false;
            
            recycledSequence.Kill();
        }
#endregion
    }
}
