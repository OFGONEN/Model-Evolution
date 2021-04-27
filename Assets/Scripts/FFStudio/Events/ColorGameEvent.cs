/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "ColorGameEvent", menuName = "FF/Event/ColorGameEvent" )]
	public class ColorGameEvent : GameEvent
	{
		public Color eventValue;
	}
}