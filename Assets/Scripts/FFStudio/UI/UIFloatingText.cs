using UnityEngine;
using FFStudio;
using DG.Tweening;

public class UIFloatingText : UIText
{
	public UIFloatingTextStack floatingTextStack;


	[HideInInspector] public Color textColor;
	private void Awake()
	{
		textColor = textRenderer.color;
	}
	private void OnDisable()
	{
		floatingTextStack.Push( this );
	}
	public override Tween GoTargetPosition()
	{
		textRenderer.DOFade( 0, GameSettings.Instance.uiFloatingEntityTweenDuration ).SetEase( Ease.InExpo );
		return uiTransform.DOMove( destinationTransform.position, GameSettings.Instance.uiFloatingEntityTweenDuration );
	}
}
