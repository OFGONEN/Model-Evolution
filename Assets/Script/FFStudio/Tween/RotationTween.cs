/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
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
        [ SerializeField ] private GameEvent[] events_fireOnComplete;
        [ SerializeField ] private UnityEvent unityEvents_FireOnComplete;
#endregion

#region Fields (Private)
		private RecycledTween recycledTween = new RecycledTween();
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
#endregion

#region Implementation
        private void EventResponse()
		{
			DOVirtual.DelayedCall( delayAmount, Play );
		}

        private void CreateAndStartTween()
        {
			if( rotationMode == RotationMode.Local )
				recycledTween.Recycle( transform.DOLocalRotate( rotationAxisMaskVector * deltaAngle, Duration, RotateMode.LocalAxisAdd ), OnTweenComplete );
			else
				recycledTween.Recycle( transform.DORotate( rotationAxisMaskVector * deltaAngle, Duration, RotateMode.WorldAxisAdd ), OnTweenComplete );

			recycledTween.Tween // Don't need to set SetRelative() as RotateMode.XXXAxisAdd automatically means relative end value.
				 .SetEase( easing )
				 .SetLoops( loop ? -1 : 0, loopType );

#if UNITY_EDITOR
			recycledTween.Tween.SetId( name + "_ff_rotation_tween" );
#endif
		}

        private void OnTweenComplete()
        {
			IsPlaying = false;

            for( var i = 0; i < events_fireOnComplete.Length; i++ )
				events_fireOnComplete[ i ].Raise();

			unityEvents_FireOnComplete.Invoke();
		}

        private void KillTween()
        {
            IsPlaying = false;

			recycledTween.Kill();
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
