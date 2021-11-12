/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class GuillotinTween : MonoBehaviour
    {
#region Fields
        public enum RotationMode { Local, World }

        [ Label( "Delta Angle (°)" )]
        public float deltaAngle;
        [ Label( "Angular Speed (°/s)" ), Min( 0 ) ]
        public float angularSpeedInDegrees;
        
        public bool playOnStart;

        [ ShowIf( "playOnStart" ) ]
		public bool hasDelay;

        [ ShowIf( "hasDelay" ) ]
		public float delayAmount;

        public RotationMode rotationMode;

        [ Dropdown( "GetVectorValues" ), Label( "Rotate Around" ) ]
        public Vector3 rotationAxisMaskVector;
        
        [ Dropdown( "GetVectorValues" ), Label( "Rotate Blade Around" ) ]
        public Vector3 rotationAxisMaskVector_Blade;

        public Ease easing = Ease.Linear;

        public GameEvent[] fireTheseOnComplete;
        
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }
        
/* Private Fields */

        private Sequence sequence;
        private float Duration => Mathf.Abs( deltaAngle / angularSpeedInDegrees );

        private DropdownList< Vector3 > GetVectorValues()
        {
            return new DropdownList< Vector3 >()
            {
                { "X",   Vector3.right      },
                { "Y",   Vector3.up         },
                { "Z",   Vector3.forward    }
            };
        }
#endregion

#region Unity API
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

            for( var i = 0; i < fireTheseOnComplete.Length; i++ )
				fireTheseOnComplete[ i ].Raise();

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
