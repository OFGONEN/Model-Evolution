using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class CameraController : MonoBehaviour
    {
#region Fields
        [ Header( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelStartEventListener;

        [ Header( "Zoom Correction By Aspect Ratio" ) ]
        public CameraZoomCalculator zoomCalculator;

        [ Header( "Transition" ) ]
        public Transform outsideTransform;
        public Transform inCabinTransform_MinimumResolution;
        public Transform inCabinTransform_MaximumResolution;

        [ InfoBox( "You might want to move this duration field into GameSettings.cs" ) ]
        public float duration = 1.0f;

        public enum Status
        {
            Outside, Inside, Free
        }
        [ ReadOnly ] public Status status;

		private bool InPlayMode => Application.isPlaying;
		private bool Inside  => status == Status.Inside;
		private bool Outside => status == Status.Outside;
#endregion

#region Unity API
		private void Awake()
        {
            levelStartEventListener.response = OnLevelStart;
        }

        private void OnEnable()
        {
            levelStartEventListener.OnEnable();
        }

        private void OnDisable()
        {
            levelStartEventListener.OnDisable();
        }

        private void Start()
        {
            ShowOutsideView();
        }

        private void Reset()
        {
            if( outsideTransform == null )
                ( outsideTransform = new GameObject().transform ).name = "Camera-Outside-Position";
            if( inCabinTransform_MinimumResolution == null )
                ( inCabinTransform_MinimumResolution = new GameObject().transform ).name =  
                                                            "Camera-In-Cabin-Position-Min-Resolution";
            if( inCabinTransform_MaximumResolution == null )
				( inCabinTransform_MaximumResolution = new GameObject().transform ).name =
															"Camera-In-Cabin-Position-Max-Resolution";

            if( zoomCalculator == null )
                zoomCalculator = gameObject.AddComponent< CameraZoomCalculator >();
		}
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Camera mainCamera = Camera.main;
            FFGizmos.DrawCamera( outsideTransform, Color.red,   mainCamera );
            FFGizmos.DrawCamera( inCabinTransform_MaximumResolution, Color.green, mainCamera );
        }
#endif
#endregion

#region Implementation
        private void OnLevelStart()
        {
            if( status != Status.Inside )
                TransitionIntoCabin();
        }

        [ Button( "Transition (Tween)" ) ]
        [ ShowIf( EConditionOperator.And, "InPlayMode", "Outside" ) ]
        private void TransitionIntoCabin()
        {
            var inCabinPosition = zoomCalculator.Calculate( inCabinTransform_MinimumResolution.position,
                                                            inCabinTransform_MaximumResolution.position );
            transform.DOMove( inCabinPosition, duration );
            transform.DORotate( inCabinTransform_MaximumResolution.rotation.eulerAngles, duration )
                     .OnComplete( () => status = Status.Inside );
        }

        [ Button( "Reverse Transition (Tween)" ) ]
        [ ShowIf( EConditionOperator.And, "InPlayMode", "Inside" ) ]
        private void TransitionBackIntoOutside()
        {
            transform.DOMove( outsideTransform.position, duration );
            transform.DORotate( outsideTransform.rotation.eulerAngles, duration )
                     .OnComplete( () => status = Status.Outside );
        }
#if UNITY_EDITOR
        [ Button() ]
        private void ResetCameraTransform()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            status = Status.Free;
        }

        [ Button( "Show Cabin View (Min Resolution)" ) ]
        private void ShowCabinView_MinimumResolution()
        {
            transform.position = inCabinTransform_MinimumResolution.position;
            transform.rotation = inCabinTransform_MinimumResolution.rotation;
            status = Status.Inside;
        }

        [ Button( "Show Cabin View ( Max Resolution)" ) ]
        private void ShowCabinView_MaximumResolution()
        {
            transform.position = inCabinTransform_MaximumResolution.position;
            transform.rotation = inCabinTransform_MaximumResolution.rotation;
            status = Status.Inside;
        }

        [ Button() ]
#endif
        private void ShowOutsideView()
        {
            transform.position = outsideTransform.position;
            transform.rotation = outsideTransform.rotation;
            status = Status.Outside;
        }
#if UNITY_EDITOR
        [ Button( "A S S I G N   Current View To In-Cabin Transform (Min Resolution)" ) ]
        private void AssignThisViewToInCabinTransform_MinimumResolution()
        {
			if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into in-cabin transform (MINIMUM resolution).\nAre you sure?",
				    		                 "Yes", "Cancel" ) == false )
				return;

			inCabinTransform_MinimumResolution.position = transform.position;
            inCabinTransform_MinimumResolution.rotation = transform.rotation;
        }

        [ Button( "A S S I G N   Current View To In-Cabin Transform (Max Resolution)" ) ]
        private void AssignThisViewToInCabinTransform_MaximumResolution()
        {
            if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into in-cabin transform (MAXIMUM resolution).\nAre you sure?",
											 "Yes", "Cancel!" ) == false )
				return;

            inCabinTransform_MaximumResolution.position = transform.position;
            inCabinTransform_MaximumResolution.rotation = transform.rotation;
        }

        [ Button( "A S S I G N   Current View To Outside Transform" ) ]
        private void AssignThisViewToOutsideTransform()
        {
			if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into outside transform.\nAre you sure?",
											 "Yes", "Cancel!" ) == false )
				return;

            outsideTransform.position = transform.position;
            outsideTransform.rotation = transform.rotation;
        }
#endif
#endregion
	}
}