/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "event_", menuName = "FF/Event/DressData GameEvent" ) ]
public class DressData_GameEvent : GameEvent
{
    public DressData eventValue;

	public void Raise( DressData data )
	{
		eventValue = data;
		Raise();
	}
}