/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class UISafeAreaFixerTop : MonoBehaviour
	{
#region Fields
		private RectTransform uiRectTransform;
#endregion

#region Unity API
		private void Awake()
		{
			uiRectTransform = GetComponent< RectTransform >();

			var position = uiRectTransform.anchoredPosition;
			position.y += Mathf.Sign( position.y ) * ( Screen.height - Screen.safeArea.height - Screen.safeArea.position.y );

			uiRectTransform.anchoredPosition = position;
		}
#endregion
	}
}