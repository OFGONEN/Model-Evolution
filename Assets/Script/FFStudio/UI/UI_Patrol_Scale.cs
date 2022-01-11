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

		private SafeEvent onComplete = new SafeEvent();

		public Tween Tween => recycledTween.Tween;
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
        public Tween DoScale_Target( Vector3 targetScale, float duration )
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOScale( targetScale , duration ),
			 	OnTweenComplete );

			return recycledTween.Tween;
		}

        public Tween DoScale_Start( float duration )
        {
			FFLogger.Log( "StartScale: " + ui_StartScale );
			recycledTween.Recycle( 
				ui_rectTransform.DOScale( ui_StartScale, duration ),
			 	OnTweenComplete );

			return recycledTween.Tween;
        }

		public void Subscribe_OnComplete( UnityMessage callback )
		{
			onComplete.Subscribe( callback );
		}
#endregion

#region Implementation
        private void OnTweenComplete()
        {
			onComplete.Invoke();
			ui_OnComplete.Invoke();

			onComplete.ClearInvokeList();
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}