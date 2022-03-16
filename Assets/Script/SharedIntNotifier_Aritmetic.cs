/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "notifier_", menuName = "FF/Data/Shared/Notifier/Float Aritmetic" ) ]
public class SharedIntNotifier_Aritmetic : SharedIntNotifier
{
	[ Button() ]
	public void Add( int value )
	{
		SharedValue += value;
	}

	[ Button() ]
	public void Subtract( int value )
	{
		SharedValue -= value;
	}

	[ Button() ]
	public void Multiply( int value )
	{
		SharedValue *= value;
	}

	[ Button() ]
	public void Divide( int value )
	{
		SharedValue /= value;
	}
}