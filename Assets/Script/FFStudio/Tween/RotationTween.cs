/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using Shapes;

namespace FFStudio
{
    public class RotationTween : MonoBehaviour
    {
        public enum RotationMode { Local, World }
        
#region Fields (Inspector Interface)
    [ Title( "Parameters" ) ]
        [ SuffixLabel( "Degrees (°)" ) ] public float deltaAngle;
        [ SuffixLabel( "Degrees/Seconds (°/s)" ), Min( 0 ) ] public float angularSpeedInDegrees;
        public RotationMode rotationMode;
        [ ValueDropdown( "VectorValues" ), LabelText( "Rotate Around" ) ] public Vector3 rotationAxisMaskVector = Vector3.right;
        
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
        [ SerializeField ] private GameEvent[] fireTheseOnComplete;
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

			IsPlaying = false;
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
			if( rotationMode == RotationMode.Local )
			    tween = transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration, RotateMode.LocalAxisAdd );
            else
                tween = transform.DORotate( rotationAxisMaskVector * deltaAngle, Duration, RotateMode.WorldAxisAdd );
                
            tween // Don't need to set SetRelative() as RotateMode.XXXAxisAdd automatically means relative end value.
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
#if UNITY_EDITOR
        private float currentStartAngle_editor;
		private float currentEndAngle_editor;
		private Vector3 currentStartVector_editor, currentEndVector_editor;
		private Renderer renderer_editor;
        
        private void OnValidate()
        {
			renderer_editor = GetComponentInChildren< Renderer >();
        }

		private void OnDrawGizmos()
		{
			var deltaAngle_radians = Mathf.Deg2Rad * deltaAngle;
			var radius = 1.0f;

			if( renderer_editor )
			{
				Vector3 rotationPlaneExtents = Vector3.Scale( renderer_editor.bounds.extents, Vector3.one - rotationAxisMaskVector );
				radius = rotationPlaneExtents.magnitude * 1.5f;
			}

			if( Application.isPlaying == false || IsPlaying == false )
            {
				currentStartAngle_editor = Vector3.Dot( transform.rotation.eulerAngles, rotationAxisMaskVector ) * Mathf.Deg2Rad;
				currentEndAngle_editor   = currentStartAngle_editor + deltaAngle_radians;

				var startToEndRotation = Quaternion.AngleAxis( deltaAngle, rotationAxisMaskVector );

				if( rotationAxisMaskVector == Vector3.right )
					currentStartVector_editor = -transform.forward;
				else if( rotationAxisMaskVector == Vector3.up )
					currentStartVector_editor = transform.right;
				else if( rotationAxisMaskVector == Vector3.forward )
					currentStartVector_editor = transform.right;
				else
					return;

				currentEndVector_editor = startToEndRotation * currentStartVector_editor;
			}
            else
            {
				if( rotationAxisMaskVector == Vector3.right )
					Draw.Sphere( transform.position - transform.forward * radius, 0.1f, Color.green );
				else if( rotationAxisMaskVector == Vector3.up )
					Draw.Sphere( transform.position + transform.right * radius, 0.1f, Color.green );
				else if( rotationAxisMaskVector == Vector3.forward )
					Draw.Sphere( transform.position + transform.right * radius, 0.1f, Color.green );
            }

			var startPos = transform.position + currentStartVector_editor * radius;
			var endPos   = transform.position + currentEndVector_editor * radius;

			Draw.Sphere( startPos, 0.1f );
            
			Draw.ArcDashed( transform.position, rotationAxisMaskVector, radius, currentStartAngle_editor, currentEndAngle_editor );

			var arrowDirection = Vector3.Cross( rotationAxisMaskVector, currentEndVector_editor ).normalized;
			Draw.Cone( endPos, arrowDirection, 0.1f, 0.2f, Color.red );
            
            // TODO: Fix X axis problem: Arc is not "in-sync" with start and end Spheres.
		}
#endif
#endregion
    }
}
