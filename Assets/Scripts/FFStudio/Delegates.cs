using Lean.Touch;
using UnityEngine;

namespace FFStudio
{
	public delegate void ChangeEvent();
    public delegate void TriggerEnter(Collider other);
	public delegate void UnityMessage();
	public delegate void LeanFingerDelegate( LeanFinger finger );
}
