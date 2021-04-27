using System.Collections;
using UnityEngine;

namespace FFStudio
{
    [CreateAssetMenu(fileName = "IntRoutineEvent", menuName = "FF/Event/Routine/IntRoutineEvent")]
    public class IntRoutineGameEvent : RoutineGameEvent
    {
        [HideInInspector]
        public int eventValue;
        protected override IEnumerator EventRoutine()
        {
            while (eventValue > 0)
            {
                routineTickEvent.Raise();
                yield return waitForSeconds;
                eventValue--;
            }

            EndRoutine();
        }
    }
}