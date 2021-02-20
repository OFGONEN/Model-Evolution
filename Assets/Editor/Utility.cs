using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

namespace FFEditor
{
    [InitializeOnLoad]
    public static class Utility
    {
        static Utility()
        {

            EditorApplication.playModeStateChanged += PlayModeChange;
        }
        [MenuItem("FFStudios/TakeScreenShot")]
        public static void TakeScreenShot()
        {
            ScreenCapture.CaptureScreenshot("ScreenShot.png");
        }

        [MenuItem("FFStudios/Delete PlayerPrefs")]
        static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        [MenuItem("FFStudios/Save All Assets")]
        static void SaveAllAssets()
        {
            AssetDatabase.SaveAssets();
        }

        [MenuItem("FFStudios/DOTween Kill All")]
        static void DOTweenKillAll()
        {
            DOTween.KillAll();
        }

        static void PlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    // DOTween.KillAll();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                default:
                    return;
            }
        }
    }
}
