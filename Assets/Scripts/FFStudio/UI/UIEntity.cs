using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
	public class UIEntity : MonoBehaviour
	{
		#region Fields
		public RectTransform uiTransform;
		public RectTransform destinationTransform;
		[HideInInspector] public Vector3 startPosition;
		[HideInInspector] public Vector3 startScale;

		#endregion

		#region UnityAPI
		public virtual void Start()
		{
			startPosition = uiTransform.position;
			startScale = uiTransform.localScale;
		}
		#endregion

		#region API

		public virtual Tween GoTargetPosition()
		{
			return uiTransform.DOMove( destinationTransform.position, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
		}

		public virtual Tween GoStartPosition()
		{
			return uiTransform.DOMove( startPosition, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
		}

		public virtual Tween GoPopOut()
		{
			return uiTransform.DOScale( startScale, GameSettings.Instance.ui_Entity_Scale_TweenDuration );
		}

		public virtual Tween GoPopIn()
		{
			return uiTransform.DOScale( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration );
		}
		#endregion
	}
}