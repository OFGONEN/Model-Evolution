/* Created by and for usage of FF Studios (2021). */

using UnityEngine.UI;
using FFStudio;
using DG.Tweening;

public class UIButton : UIEntity
{
#region Fields
	public Button uiButton;
#endregion

#region API	
	public override Tween GoToTargetPosition()
	{
		uiButton.interactable = false;
		
		return uiTransform
				.DOMove( destinationTransform.position, GameSettings.Instance.ui_Entity_Fade_TweenDuration )
				.OnComplete( () => uiButton.interactable = true );
	}
	
	public override Tween GoToStartPosition()
	{
		uiButton.interactable = false;
		
		return uiTransform
				.DOMove( startPosition, GameSettings.Instance.ui_Entity_Fade_TweenDuration )
				.OnComplete( () => uiButton.interactable = true );
	}
#endregion
}
