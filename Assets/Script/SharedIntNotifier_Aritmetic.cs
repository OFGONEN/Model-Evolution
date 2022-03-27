/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

[ CreateAssetMenu( fileName = "notifier_", menuName = "FF/Data/Shared/Notifier/Float Aritmetic" ) ]
public class SharedIntNotifier_Aritmetic : SharedIntNotifier
{
	public float cooldown;

	private bool canInteract = true;

	[ Button() ]
	public void Add( int value )
	{
		if( canInteract )
		{
			SharedValue += value;
			CoolDown();
		}
	}

	[ Button() ]
	public void Subtract( int value )
	{
		if( canInteract )
		{
			SharedValue -= value;
			CoolDown();
		}
	}

	[ Button() ]
	public void Multiply( int value )
	{
		if( canInteract )
		{
			SharedValue *= value;
			CoolDown();
		}
	}

	[ Button() ]
	public void Divide( int value )
	{
		if( canInteract )
		{
			SharedValue /= value;
			CoolDown();
		}
	}

	private void CoolDown()
	{
		canInteract = false;
		DOVirtual.DelayedCall( cooldown, OnCoolDownComplete );
	}

	private void OnCoolDownComplete()
	{
		canInteract = true;
	}
}