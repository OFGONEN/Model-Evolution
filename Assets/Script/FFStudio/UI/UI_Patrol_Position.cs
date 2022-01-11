/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace FFStudio
{
	public class UI_Patrol_Position : MonoBehaviour
	{
#region Fields
		public RectTransform ui_Target;
		public UnityEvent ui_OnComplete;

		private Vector3 ui_StartPosition;
		private RectTransform ui_rectTransform;
        private RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
        public RectTransform UI_RectTransform => ui_rectTransform;
#endregion

#region Unity API
        private void Awake()
        {
            ui_rectTransform = GetComponent< RectTransform >();
			ui_StartPosition = ui_rectTransform.position;
		}
#endregion

#region API
        public void DoGo_Target( float duration )
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOMove( ui_Target.position, duration ),
			 	OnTweenComplete );
        }

        public void DoGo_Start( float duration )
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOMove( ui_StartPosition, duration ),
			 	OnTweenComplete );
        }
#endregion

#region Implementation
        private void OnTweenComplete()
        {
			ui_OnComplete.Invoke();
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}