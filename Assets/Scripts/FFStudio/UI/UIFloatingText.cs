/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;

public class UIFloatingText : UIText
{
#region Fields
	public UIFloatingTextStack floatingTextStack;
	
	[ HideInInspector ] public Color textColor;
#endregion

#region Unity API
	private void Awake()
	{
		textColor = textRenderer.color;
	}
	
	private void OnDisable()
	{
		floatingTextStack.Stack.Push( this );
	}
#endregion

#region API
	public override Tween GoToTargetPosition()
	{
		textRenderer.DOFade( 0, GameSettings.Instance.ui_Entity_FloatingMove_TweenDuration )
					.SetEase( Ease.InExpo );
		return uiTransform.DOMove( destinationTransform.position, GameSettings.Instance.ui_Entity_FloatingMove_TweenDuration );
	}
#endregion
}
