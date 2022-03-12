/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "notifier_", menuName = "FF/Data/Shared/Notifier/Float Aritmetic" ) ]
public class SharedIntNotifier_Aritmetic : SharedIntNotifier
{
	public void Add( int value )
	{
		SharedValue += value;
	}

	public void Subtract( int value )
	{
		SharedValue -= value;
	}

	public void Multiply( int value )
	{
		SharedValue *= value;
	}

	public void Divide( int value )
	{
		SharedValue /= value;
	}
}