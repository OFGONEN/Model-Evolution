/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class RotationTween : MonoBehaviour
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

		[ DisableIf( "IsPlaying" ) ]
        public bool loop;

        [ ShowIf( "loop" ) ]
        public LoopType loopType = LoopType.Restart;

        public RotationMode rotationMode;

        [ Dropdown( "GetVectorValues" ), Label( "Rotate Around" ) ]
        public Vector3 rotationAxisMaskVector;

        public Ease easing = Ease.Linear;

        public GameEvent[] fireTheseOnComplete;
        
        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }
        
/* Private Fields */

        private Tween tween;
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
