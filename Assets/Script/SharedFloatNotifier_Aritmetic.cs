/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "notifier_", menuName = "FF/Data/Shared/Notifier/Float Aritmetic" ) ]
public class SharedFloatNotifier_Aritmetic : SharedFloatNotifier
{
	public void Add( float value )
	{
		SharedValue += value;
	}

	public void Subtract( float value )
	{
		SharedValue -= value;
	}

	public void Multiply( float value )
	{
		SharedValue *= value;
	}

	public void Divide( float value )
	{
		SharedValue /= value;
	}
}