using FFStudio;
using DG.Tweening;
using UnityEngine.UI;

public class UIButton : UIEntity
{
	public Button uiButton;
	public override Tween GoTargetPosition()
	{
		uiButton.interactable = false;
		return uiTransform.DOMove( destinationTransform.position, GameSettings.Instance.uiEntityMoveTweenDuration ).OnComplete( MakeButtonInteractable );
	}
	public override Tween GoStartPosition()
	{
		uiButton.interactable = false;
		return uiTransform.DOMove( startPosition, GameSettings.Instance.uiEntityMoveTweenDuration ).OnComplete( MakeButtonInteractable );
	}
	void MakeButtonInteractable()
	{
		uiButton.interactable = true;
	}
}
