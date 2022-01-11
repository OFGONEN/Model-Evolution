/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace FFStudio
{
	public class UI_Float : MonoBehaviour
	{
#region Fields
        public UnityEvent ui_OnComplete;

        private RectTransform ui_rectTransform;
        private RecycledTween recycledTween = new RecycledTween();

		public Tween Tween => recycledTween.Tween;
#endregion

#region Properties
        public RectTransform UI_RectTransform => ui_rectTransform;
#endregion

#region Unity API
        private void Awake()
        {
            ui_rectTransform = GetComponent< RectTransform >();
        }
#endregion

#region API
        public Tween DoFloat( float relativeValue, float duration ) 
        {
			recycledTween.Recycle( 
				ui_rectTransform.DOMove( ui_rectTransform.position + Vector3.up * relativeValue, duration ),
			 	OnTweenComplete );

			return recycledTween.Tween;
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