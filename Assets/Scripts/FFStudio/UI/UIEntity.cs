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
			return uiTransform.DOMove( destinationTransform.position, GameSettings.instance.uiEntityMoveTweenDuration );
		}

		public virtual Tween GoStartPosition()
		{
			return uiTransform.DOMove( startPosition, GameSettings.instance.uiEntityMoveTweenDuration );
		}

		public virtual Tween GoPopOut()
		{
			return uiTransform.DOScale( startScale, GameSettings.instance.uiEntityScaleTweenDuration );
		}

		public virtual Tween GoPopIn()
		{
			return uiTransform.DOScale( Vector3.zero, GameSettings.instance.uiEntityScaleTweenDuration );
		}
		#endregion
	}
}