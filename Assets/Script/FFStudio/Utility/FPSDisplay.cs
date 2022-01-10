/* Copied from another package. Modified by and for usage of FF Studios (2021). */

/*
 * 30.06.2021.
 * Copied directly from Obi V6.0.1. package.
 * Refactored and reformatted.
 * Also put in FFStudio namespace.
 */

using UnityEngine;
using UnityEngine.UI;

namespace FFStudio
{
	[ RequireComponent( typeof( Text ) ) ]
	public class FPSDisplay : MonoBehaviour
	{
		public float updateInterval = 0.5f;

		public bool showMedian = false;
		public float medianLearnrate = 0.05f;

		private float accumulated  = 0;
		private int   frames = 0;
		private float timeleft;
		private float currentFPS = 0;

		private float median  = 0;
		private float average = 0;

		public float CurrentFPS => currentFPS; 
		public float FPSMedian  => median;
		public float FPSAverage => average;

		Text uguiText;

		void Start()
		{
			uguiText = GetComponent< Text >();
			timeleft = updateInterval;
		}

		void Update()
		{
			// Timing inside the editor is not accurate. Only use in actual build.

			//#if !UNITY_EDITOR

			timeleft -= Time.deltaTime;
			accumulated += Time.timeScale / Time.deltaTime;
			++frames;

			// Interval ended - update GUI text and start new interval.
			if( timeleft <= 0.0 )
			{
				currentFPS = accumulated / frames;

				average += ( Mathf.Abs( currentFPS ) - average ) * 0.1f;
				median  += Mathf.Sign( currentFPS - median ) * Mathf.Min( average * medianLearnrate, Mathf.Abs( currentFPS - median ) );

				// Display two fractional digits (f2 format).
				float fps           = showMedian ? median : currentFPS;
				      uguiText.text = System.String.Format( "{0:F2} FPS ({1:F1} ms)", fps, 1000.0f / fps );

				timeleft    = updateInterval;
				accumulated = 0.0F;
				frames      = 0;
			}
			//#endif
		}

		public void ResetMedianAndAverage()
		{
			median = 0;
			average = 0;
		}
	}
}