using FFStudio;
using DG.Tweening;
using UnityEngine.UI;

public class UIButton : UIEntity
{
	public Button uiButton;
	public override Tween GoTargetPosition()
	{
		uiButton.interactable = false;
		return uiTransform.DOMove( destinationTransform.position, gameSettings.uiEntityMoveTweenDuration ).OnComplete( MakeButtonInteractable );
	}
	public override Tween GoStartPosition()
	{
		uiButton.interactable = false;
		return uiTransform.DOMove( startPosition, gameSettings.uiEntityMoveTweenDuration ).OnComplete( MakeButtonInteractable );
	}
	void MakeButtonInteractable()
	{
		uiButton.interactable = true;
	}
}
