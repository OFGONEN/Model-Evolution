using DG.Tweening;

namespace FFStudio
{
	public class RecycledSequence
	{
		private UnityMessage onComplete;
		private Sequence sequence;

		public void Recycle( UnityMessage onComplete )
		{
			sequence = sequence.KillProper();

			this.onComplete = onComplete;

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );
		}

		public void Recycle()
		{
			sequence = sequence.KillProper();

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );
		}

		private void OnComplete_Safe()
		{
			onComplete?.Invoke();

			sequence = null;
		}
	}
}