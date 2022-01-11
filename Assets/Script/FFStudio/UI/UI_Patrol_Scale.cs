/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace FFStudio
{
	public class UI_Patrol_Scale : MonoBehaviour
	{
#region Fields
		public UnityEvent ui_OnComplete;

		private Vector3 ui_StartScale;
		private RectTransform ui_rectTransform;
		private RecycledTween recycledTween = new RecycledTween();	
#endregion

#region Properties
        public RectTransform UI_RectTransform => ui_rectTransform;
#endregion

#region Unity API
        private void Awake()
        {
			ui_rectTransform = GetComponent<RectTransform>();
			ui_StartScale = ui_rectTransform.localScale;    
        }
#endregion

#region API
        public void DoScale_Target( Vector3 targetScale, float duration )
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOScale( targetScale , duration ),
			 	OnTweenComplete );
        }

        public void DoScale_Start( float duration )
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOMove( ui_StartScale, duration ),
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