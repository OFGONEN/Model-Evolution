using UnityEngine;

namespace FFStudio
{
	public class UISafeAreaFixerTop : MonoBehaviour
	{
		RectTransform uiRectTransform;
		private void Awake()
		{
			uiRectTransform = GetComponent<RectTransform>();

			var _postion = uiRectTransform.anchoredPosition;
			_postion.y += Mathf.Sign( _postion.y ) * ( Screen.height - Screen.safeArea.height - Screen.safeArea.position.y );

			uiRectTransform.anchoredPosition = _postion;
		}
	}
}