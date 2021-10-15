/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using DG.Tweening;

public class UIFlasher : MonoBehaviour
{
#region Fields
	public Image flashRenderer;

	public EventListenerDelegateResponse flashResponse;

	public float goFlashTime;
	public float returnFlashTime;
#endregion

#region Unity API
	private void OnEnable()
	{
		flashResponse.OnEnable();
	}

	private void OnDisable()
	{
		flashResponse.OnDisable();
	}

	private void Awake()
	{
		flashResponse.response = FlashResponse;
	}
#endregion

#region Implementation
	private void FlashResponse()
	{
		var sequence = DOTween.Sequence();

		sequence.Join( flashRenderer.DOFade( 1, goFlashTime ) )
				.Append( flashRenderer.DOFade( 0, returnFlashTime ) );
	}
#endregion
}
