using System.Collections;
using UnityEngine;

namespace FFStudio
{
    [CreateAssetMenu(fileName = "FloatRoutineEvent", menuName = "FF/Event/Routine/FloatRoutineEvent")]
    public class FloatRoutineGameEvent : RoutineGameEvent
    {
        [HideInInspector]
        public float eventValue;
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