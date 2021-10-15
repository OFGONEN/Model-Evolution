/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class CameraTransition : MonoBehaviour
    {
#region Fields
        [ Header( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelStartEventListener;

        [ Header( "Zoom Correction By Aspect Ratio" ) ]
        public CameraZoomCalculator zoomCalculator;

        [ Header( "Transition" ) ]
        public Transform outsideTransform;
        public Transform insideTransform_MinimumResolution;
        public Transform insideTransform_MaximumResolution;

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
            if( insideTransform_MinimumResolution == null )
                ( insideTransform_MinimumResolution = new GameObject().transform ).name = "Camera-Inside-Min-Resolution";
            if( insideTransform_MaximumResolution == null )
				( insideTransform_MaximumResolution = new GameObject().transform ).name = "Camera-Inside-Max-Resolution";

            if( zoomCalculator == null )
                zoomCalculator = gameObject.AddComponent< CameraZoomCalculator >();
		}
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Camera mainCamera = Camera.main;
            FFGizmos.DrawCamera( outsideTransform, Color.red,   mainCamera );
            FFGizmos.DrawCamera( insideTransform_MaximumResolution, Color.green, mainCamera );
        }
#endif
#endregion

#region Implementation
        private void OnLevelStart()
        {
            if( status != Status.Inside )
                TransitionIntoInside();
        }

        [ Button( "Transition (Tween)" ) ]
        [ ShowIf( EConditionOperator.And, "InPlayMode", "Outside" ) ]
        private void TransitionIntoInside()
        {
            var insidePosition = zoomCalculator.Calculate( insideTransform_MinimumResolution.position,
                                                           insideTransform_MaximumResolution.position );
            transform.DOMove( insidePosition, duration );
            transform.DORotate( insideTransform_MaximumResolution.rotation.eulerAngles, duration )
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

        [ Button( "Show Inside View (Min Resolution)" ) ]
        private void ShowInsideView_MinimumResolution()
        {
            transform.position = insideTransform_MinimumResolution.position;
            transform.rotation = insideTransform_MinimumResolution.rotation;
            status = Status.Inside;
        }

        [ Button( "Show Inside View ( Max Resolution)" ) ]
        private void ShowInsideView_MaximumResolution()
        {
            transform.position = insideTransform_MaximumResolution.position;
            transform.rotation = insideTransform_MaximumResolution.rotation;
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
        [ Button( "A S S I G N   Current View To Inside Transform (Min Resolution)" ) ]
        private void AssignThisViewToInsideTransform_MinimumResolution()
        {
			if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into Inside transform (MINIMUM resolution).\nAre you sure?",
				    		                 "Yes", "Cancel" ) == false )
				return;

			Camera sceneViewCam = SceneView.lastActiveSceneView.camera;

			insideTransform_MinimumResolution.position = sceneViewCam.transform.position;
            insideTransform_MinimumResolution.rotation = sceneViewCam.transform.rotation;
        }

        [ Button( "A S S I G N   Current View To Inside Transform (Max Resolution)" ) ]
        private void AssignThisViewToInsideTransform_MaximumResolution()
        {
            if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into Inside transform (MAXIMUM resolution).\nAre you sure?",
											 "Yes", "Cancel!" ) == false )
				return;

			Camera sceneViewCam = SceneView.lastActiveSceneView.camera;

            insideTransform_MaximumResolution.position = transform.position;
            insideTransform_MaximumResolution.rotation = transform.rotation;
        }

        [ Button( "A S S I G N   Current View To Outside Transform" ) ]
        private void AssignThisViewToOutsideTransform()
        {
			if( EditorUtility.DisplayDialog( /* Title: */ "Assigning Current View to a Transform",
                                             "Assigning current view into outside transform.\nAre you sure?",
											 "Yes", "Cancel!" ) == false )
				return;

			Camera sceneViewCam = SceneView.lastActiveSceneView.camera;

			outsideTransform.position = transform.position;
            outsideTransform.rotation = transform.rotation;
        }
#endif
#endregion
	}
}