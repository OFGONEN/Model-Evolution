/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

namespace FFStudio
{
	public class UI_Fade_Text : MonoBehaviour
	{
#region Fields
		public UnityEvent ui_OnComplete;

		private TextMeshProUGUI ui_Text;
		private RecycledTween recycledTween = new RecycledTween();

#endregion

#region Properties
		public TextMeshProUGUI UI_Text => ui_Text;
#endregion

#region Unity API
		private void Awake()
		{
			ui_Text = GetComponentInChildren< TextMeshProUGUI >();
		}
#endregion

#region API
		public void DoFade( float endValue, float duration )
		{
			recycledTween.Recycle(
				ui_Text.DOFade( endValue, duration ),
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