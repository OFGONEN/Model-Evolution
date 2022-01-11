/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace FFStudio
{
	public class UI_Fade_Image : MonoBehaviour
	{
#region Fields
        public UnityEvent ui_OnComplete;

        private Image ui_Image;
        private RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
        private void Awake()
        {
            ui_Image = GetComponentInChildren< Image >();
        }
#endregion

#region API
        public void DoFade( float endValue, float duration )
        {
			recycledTween.Recycle( 
                ui_Image.DOFade( endValue, duration ),
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