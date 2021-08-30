/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
	public class UIEntity : MonoBehaviour
	{
#region Fields
		public RectTransform uiTransform;
		public RectTransform destinationTransform;
		[ HideInInspector ] public Vector3 startPosition;
		[ HideInInspector ] public Vector3 startScale;
#endregion

#region UnityAPI
		public virtual void Start()
		{
			startPosition = uiTransform.position;
			startScale    = uiTransform.localScale;
		}
#endregion

#region API
		public virtual Tween GoToTargetPosition()
		{
			return uiTransform.DOMove( destinationTransform.position, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
		}

		public virtual Tween GoToStartPosition()
		{
			return uiTransform.DOMove( startPosition, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
		}

		public virtual Tween Appear()
		{
			return uiTransform.DOScale( startScale, GameSettings.Instance.ui_Entity_Scale_TweenDuration );
		}

		public virtual Tween Disappear()
		{
			return uiTransform.DOScale( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration );
		}
#endregion
	}
}