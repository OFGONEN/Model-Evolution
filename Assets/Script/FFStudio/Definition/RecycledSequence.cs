using DG.Tweening;

namespace FFStudio
{
	public class RecycledSequence
	{
		private UnityMessage onComplete;
		private Sequence sequence;

		public Sequence Recycle( UnityMessage onComplete )
		{
			sequence = sequence.KillProper();

			this.onComplete = onComplete;

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );

			return sequence;
		}

		public Sequence Recycle()
		{
			sequence = sequence.KillProper();

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );
			return sequence;
		}

		public void Kill()
		{
			sequence = sequence.KillProper();
		}

		private void OnComplete_Safe()
		{
			sequence = null;
			onComplete?.Invoke();
		}
	}
}