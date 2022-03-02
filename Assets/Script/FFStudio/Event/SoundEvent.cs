/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "event_", menuName = "FF/Event/SoundEvent" ) ]
    public class SoundEvent : GameEvent
    {
        public AudioClip audioClip;
    }
}