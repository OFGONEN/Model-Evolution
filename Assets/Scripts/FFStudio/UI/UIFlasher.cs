using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using DG.Tweening;

public class UIFlasher : MonoBehaviour
{
	public Image flashRenderer;

	public EventListenerDelegateResponse flashResponse;

	public float goFlashTime;
	public float returnFlashTime;

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
		flashResponse.response = Flash;
	}

	void Flash()
	{
		var _sequence = DOTween.Sequence();

		_sequence.Join( flashRenderer.DOFade( 1, goFlashTime ) );
		_sequence.Append( flashRenderer.DOFade( 0, returnFlashTime ) );

	}
}
