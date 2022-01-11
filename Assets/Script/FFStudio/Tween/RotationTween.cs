/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;

namespace FFStudio
{
    public class RotationTween : MonoBehaviour
    {
        public enum RotationMode { Local, World }
        
#region Fields (Inspector Interface)
        [ TitleGroup( "Parameters" ), SuffixLabel( "Degrees (°)" ) ]                    public float deltaAngle;
        [ TitleGroup( "Parameters" ), SuffixLabel( "Degrees/Second (°/s)" ), Min( 0 ) ] public float angularSpeedInDegrees;
        [ TitleGroup( "Parameters" ) ]                                                  public RotationMode rotationMode;
        [ TitleGroup( "Parameters" ), ValueDropdown( "VectorValues" ), LabelText( "Rotate Around" ) ]
            public Vector3 rotationAxisMaskVector = Vector3.right;
        
        [ TitleGroup( "Start Options" ) ]                       public bool playOnStart;
		[ TitleGroup( "Start Options" ) ]                       public bool hasDelay;
        [ TitleGroup( "Start Options" ), ShowIf( "hasDelay" ) ] public float delayAmount;
        
        [ TitleGroup( "Tween" ), DisableIf( "IsPlaying" ) ] public bool loop;
        [ TitleGroup( "Tween" ), ShowIf( "loop" ) ]         public LoopType loopType = LoopType.Restart;
        [ TitleGroup( "Tween" ) ]                           public Ease easing = Ease.Linear;
        
        [ TitleGroup( "Event Flow" ), SerializeField ] private MultipleEventListenerDelegateResponse triggeringEvents;
        [ TitleGroup( "Event Flow" ) ] public GameEvent[] fireTheseOnComplete;
#endregion

#region Fields (Private)
		private Tween tween;
        private float Duration => Mathf.Abs( deltaAngle / angularSpeedInDegrees );

        private IEnumerable VectorValues = new ValueDropdownList< Vector3 >()
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
			/* Since we use SetRelative + RotateMode.FastBeyond360 combo, we need to specify a delta instead of end value. */

			if( rotationMode == RotationMode.Local )
			    tween = transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration );
            else
                tween = transform.DORotate( rotationAxisMaskVector * deltaAngle, Duration );
                
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
    }
}
