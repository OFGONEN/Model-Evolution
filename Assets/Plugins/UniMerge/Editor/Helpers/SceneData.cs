//Matt Schoen
//1-20-2019
//
// This software is the copyrighted material of its author, Matt Schoen, and his company Defective Studios.
// It is available for sale on the Unity Asset store and is subject to their restrictions and limitations, as well as
// the following: You shall not reproduce or re-distribute this software without the express written (e-mail is fine)
// permission of the author. If permission is granted, the code (this file and related files) must bear this license
// in its entirety. Anyone who purchases the script is welcome to modify and re-use the code at their personal risk
// and under the condition that it not be included in any distribution builds. The software is provided as-is without
// warranty and the author bears no responsibility for damages or losses caused by the software.
// This Agreement becomes effective from the day you have installed, copied, accessed, downloaded and/or otherwise used
// the software.

//UniMerge 1.11
//SceneData class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_5 || UNITY_5_3_OR_NEWER || (UNITY_4 && !(UNITY_4_2 || UNITY_4_1 || UNITY_4_0))
using UnityEngine.Rendering;
#endif
#if UNITY_5_5_OR_NEWER
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;
#endif
using UnityObject = UnityEngine.Object;
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global

namespace UniMerge.Editor.Helpers {
	public class SceneData : ScriptableObject {
		const string MyTempSettingsPath = "Assets/UM_SceneSettings_temp_mine.asset";
		const string TheirTempSettingsPath = "Assets/UM_SceneSettings_temp_theirs.asset";

		static readonly PropertyInfo[] OcclusionCullingProperties = typeof(StaticOcclusionCulling).GetProperties();
		static readonly PropertyInfo[] LightmapSettingsProperties = typeof(UnityEngine.LightmapSettings).GetProperties();
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4)
		static readonly PropertyInfo[] LightmapDataProperties = typeof(UnityEngine.LightmapData).GetProperties();
#endif

#if UNITY_5 || UNITY_5_3_OR_NEWER
		static readonly MethodInfo GetRenderSettings =
			GetStaticMethod(typeof(UnityEngine.RenderSettings), "GetRenderSettings");
#else
		static readonly PropertyInfo[] RenderSettingsProperties = typeof(UnityEngine.RenderSettings).GetProperties();
#endif

		static readonly MethodInfo GetLightmapEditorSettings =
			GetStaticMethod(typeof(UnityEditor.LightmapEditorSettings), "GetLightmapSettings");

		[Serializable]
		public class OcclusionCullingSettings {
			public float smallestOccluder;
			public float smallestHole;
			public float backfaceThreshold;
		}

		[Serializable]
		public class RenderSettings {
#if UNITY_5 || UNITY_5_3_OR_NEWER
			public bool m_Fog;
			public Color m_FogColor;
			public FogMode m_FogMode;
			public float m_FogDensity;
			public float m_LinearFogStart;
			public float m_LinearFogEnd;
			public Color m_AmbientSkyColor;

			public Color m_AmbientEquatorColor;
			public Color m_AmbientGroundColor;
			public float m_AmbientIntensity;
			public int m_AmbientMode;
#if UNITY_5_6_OR_NEWER
			public Color m_SubtractiveShadowColor;
#endif
			public Material m_SkyboxMaterial;
			public float m_HaloStrength;
			public float m_FlareStrength;
			public float m_FlareFadeSpeed;
			public Texture2D m_HaloTexture;
			public Texture2D m_SpotCookie;
			public DefaultReflectionMode m_DefaultReflectionMode;
			public int m_DefaultReflectionResolution;
			public int m_ReflectionBounces;
			public float m_ReflectionIntensity;
			public Cubemap m_CustomReflection;
			public Light m_Sun;
			public string sunPath;
			public Color m_IndirectSpecularColor;
#else
			public bool fog;
			public Color fogColor;
#if !(UNITY_3_0 || UNITY_3_1)
			public FogMode fogMode;
#endif
			public float fogDensity;
			public float fogStartDistance;
			public float fogEndDistance;
			public Color ambientLight;
			public Material skybox;
			public float haloStrength;
			public float flareStrength;
			public float flareFadeSpeed;
			public Texture2D haloTexture;
#endif
		}

		[Serializable]
		public class LightmapEditorSettings {
			public float m_Resolution;
			public float m_BakeResolution;
			public int m_TextureWidth;
			public int m_TextureHeight;
#if UNITY_5_4_OR_NEWER
			public bool m_AO;
#endif
			public float m_AOMaxDistance;
			public float m_CompAOExponent;
#if UNITY_5_4_OR_NEWER
			public float m_CompAOExponentDirect;
#endif
			public int m_Padding;
#if UNITY_5 || UNITY_5_3_OR_NEWER
			public LightmapParameters m_LightmapParameters;
#endif
			public int m_LightmapsBakeMode;
			public bool m_TextureCompression;
#if UNITY_5_4 || UNITY_5_5
			public bool m_DirectLightInLightProbes;
#endif
			public bool m_FinalGather;
#if UNITY_5_4_OR_NEWER
			public bool m_FinalGatherFiltering;
#endif
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_6_OR_NEWER
			public int m_FinalGatherRayCount;
#endif
			public int m_ReflectionCompression;
#if UNITY_5_6_OR_NEWER
			public int m_MixedBakeMode;
			public int m_BakeBackend;
			public int m_PVRSampling;
			public int m_PVRDirectSampleCount;
			public int m_PVRSampleCount;
			public int m_PVRBounces;
			public int m_PVRFiltering;
			public int m_PVRFilteringMode;
			public bool m_PVRCulling;
			public int m_PVRFilteringGaussRadiusDirect;
			public int m_PVRFilteringGaussRadiusIndirect;
			public int m_PVRFilteringGaussRadiusAO;
			public float m_PVRFilteringAtrousColorSigma;
			public float m_PVRFilteringAtrousNormalSigma;
			public float m_PVRFilteringAtrousPositionSigma;
#endif
		}

		[Serializable]
		public sealed class NavMeshBuildSettings {
#if UNITY_5_4_OR_NEWER
			public int agentTypeID;
#endif
			public float agentRadius;
			public float agentHeight;
			public float agentSlope;
			public float agentClimb;
#if UNITY_2017_1_OR_NEWER
			public float ledgeDropHeight;
			public float maxJumpAcrossDistance;
#endif
			public float minRegionArea;
			public bool manualCellSize;
			public float cellSize;
#if UNITY_5_6_OR_NEWER
			public bool manualTileSize;
			public int tileSize;
#endif
#if UNITY_5_3 || UNITY_2017_1_OR_NEWER
			public int accuratePlacement;
#endif
		}

#if UNITY_5 || UNITY_5_3_OR_NEWER
		[Serializable]
		public class GISettings {
			public float m_BounceScale;
			public float m_IndirectOutputScale;
			public float m_AlbedoBoost;
			public float m_TemporalCoherenceThreshold;
			public int m_EnvironmentLightingMode;
			public bool m_EnableBakedLightmaps;
			public bool m_EnableRealtimeLightmaps;
		}
#endif

		[Serializable]
		public class LightmapSettings {
			public LightmapsMode lightmapsMode;
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			public LightingDataAsset lightingData;
#endif
			public LightmapData[] lightmaps;
		}

		[Serializable]
		public sealed class LightmapData {
			public Texture2D lightmapColor;
			public Texture2D lightmapDir;
			public Texture2D shadowMask;
		}

		public OcclusionCullingSettings occlusionCullingSettings = new OcclusionCullingSettings();
		public RenderSettings renderSettings = new RenderSettings();
#if UNITY_5 || UNITY_5_3_OR_NEWER
		public GISettings giSettings = new GISettings();
#endif
		public LightmapEditorSettings lightmapEditorSettings = new LightmapEditorSettings();
		public LightmapSettings lightmapSettings = new LightmapSettings();
		public NavMeshBuildSettings navMeshBuildSettings = new NavMeshBuildSettings();

		static MethodInfo GetStaticMethod(Type t, string methodName) {
			return t.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		public static void Cleanup() {
			var paths = new[] { MyTempSettingsPath, TheirTempSettingsPath };

			foreach (var path in paths)
				if (!string.IsNullOrEmpty(path))
					AssetDatabase.DeleteAsset(path);
		}

		public static void Capture(bool isMine) {
			var sceneData = CreateInstance<SceneData>();
			sceneData.CaptureSettings();
			AssetDatabase.CreateAsset(sceneData, isMine ? MyTempSettingsPath : TheirTempSettingsPath);
		}

		void CaptureSettings() {
			UnityObject obj;
#if UNITY_5 || UNITY_5_3_OR_NEWER
			SerializedObject sceneRenderSettings = null;
			if (GetRenderSettings != null) {
				obj = GetRenderSettings.Invoke(null, null) as UnityObject;
				if (obj)
					sceneRenderSettings = new SerializedObject(obj);
			}
#endif

			SerializedObject sceneLightmapEditorSettings = null;
			if (GetLightmapEditorSettings != null) {
				obj = GetLightmapEditorSettings.Invoke(null, null) as UnityObject;
				if (obj)
					sceneLightmapEditorSettings = new SerializedObject(obj);
			}

#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
			SerializedObject sceneNavMeshBuildSettings = null;
			obj = NavMeshBuilder.navMeshSettingsObject;
			if (obj)
				sceneNavMeshBuildSettings = new SerializedObject(obj);
#endif

			var lightmaps = UnityEngine.LightmapSettings.lightmaps;
			var lightmapCount = lightmaps.Length;
			lightmapSettings.lightmaps = new LightmapData[lightmapCount];
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			lightmapSettings.lightingData = Lightmapping.lightingDataAsset;
#endif

			var serializedObject = new SerializedObject(this);
			var iterator = serializedObject.GetIterator();
			var hasNext = true;
			// ReSharper disable once AssignmentInConditionalExpression
			while (hasNext && (hasNext = iterator.NextVisible(true))) {
				var propertyPath = iterator.propertyPath;
				if (propertyPath.Contains("occlusionCullingSettings"))
					CaptureSettingToProperty(OcclusionCullingProperties, iterator);

#if UNITY_5 || UNITY_5_3_OR_NEWER
				if (propertyPath.Contains("renderSettings") && sceneRenderSettings != null)
					CopyFromObjectToProperty(sceneRenderSettings, iterator);
#else
				if (propertyPath.Contains("renderSettings") && RenderSettingsProperties != null)
					CaptureSettingToProperty(RenderSettingsProperties, iterator);
#endif

				if (propertyPath.Contains("lightmapSettings") && LightmapSettingsProperties != null)
					CaptureSettingToProperty(LightmapSettingsProperties, iterator);

				if (propertyPath.Contains("giSettings") && sceneLightmapEditorSettings != null)
					CopyFromObjectToProperty(sceneLightmapEditorSettings, iterator, "m_GISettings.");

				if (propertyPath.Contains("lightmapEditorSettings") && sceneLightmapEditorSettings != null)
					CopyFromObjectToProperty(sceneLightmapEditorSettings, iterator, "m_LightmapEditorSettings.");

#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
				if (propertyPath.Contains("navMeshBuildSettings") && sceneNavMeshBuildSettings != null)
					CopyFromObjectToProperty(sceneNavMeshBuildSettings, iterator, "m_BuildSettings.");

				if (propertyPath == "lightmapSettings.lightmaps") {
					if (iterator.arraySize == 0)
						continue;

					iterator.NextVisible(true); // Move past ArraySize
					iterator.NextVisible(true); // Move past first element
					var depth = iterator.depth;
					for (var i = 0; i < lightmapCount; i++) {
						var lightmapData = lightmaps[i];
						// ReSharper disable once AssignmentInConditionalExpression
						while (hasNext = iterator.NextVisible(true)) {
							if (iterator.depth <= depth)
								break;

							// Setting propertyPath will allow for other array types in the future (like NavMeshBuildSettings)
							//propertyPath = iterator.propertyPath;
							CaptureSettingToProperty(LightmapDataProperties, iterator, lightmapData);
						}
					}
				}
#endif

				if (!hasNext)
					break;
			}

			serializedObject.ApplyModifiedProperties();

#if UNITY_5 || UNITY_5_3_OR_NEWER
			var sun = renderSettings.m_Sun;
			if (sun)
				renderSettings.sunPath = sun.transform.GetPath();
#endif
		}

		static void CopyFromObjectToProperty(SerializedObject obj, SerializedProperty iterator, string prefix = "") {
			var property = obj.FindProperty(prefix + iterator.name);
			if (property != null)
				Util.SetProperty(property, iterator);
		}

		static void CaptureSettingToProperty(PropertyInfo[] properties, SerializedProperty iterator, object obj = null) {
			foreach (var property in properties) {
				if (iterator.name != property.Name)
					continue;

				switch (iterator.propertyType) {
					case SerializedPropertyType.Float:
						iterator.floatValue = (float) property.GetValue(obj, null);
						break;
					case SerializedPropertyType.Integer:
						iterator.intValue = (int) property.GetValue(obj, null);
						break;
					case SerializedPropertyType.Boolean:
						iterator.boolValue = (bool) property.GetValue(obj, null);
						break;
					case SerializedPropertyType.Color:
						iterator.colorValue = (Color) property.GetValue(obj, null);
						break;
					case SerializedPropertyType.Enum:
						iterator.enumValueIndex = Array.IndexOf(Enum.GetValues(property.PropertyType), property.GetValue(obj, null));
						break;
					case SerializedPropertyType.ObjectReference:
						iterator.objectReferenceValue = property.GetValue(obj, null) as UnityObject;
						break;
					case SerializedPropertyType.Generic: break;
					default:
						Debug.Log("can't handle " + iterator.propertyType);
						break;
				}
			}
		}

		public void ApplySettings() {
			// TODO: Fix this method for Unity 3/4, reflection props will be null
#if (UNITY_5 || UNITY_5_3_OR_NEWER)
			var sceneRenderSettings = new SerializedObject(GetRenderSettings.Invoke(null, null) as UnityObject);
#endif
#if !(UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0)
			var sceneNavMeshBuildSettings = new SerializedObject(NavMeshBuilder.navMeshSettingsObject);
#if !UNITY_3_5
			var sceneLightmapEditorSettings = new SerializedObject(GetLightmapEditorSettings.Invoke(null, null) as UnityObject);
#endif
#endif

			var lightmaps = UnityEngine.LightmapSettings.lightmaps;
			var lightmapCount = lightmaps.Length;
			lightmapSettings.lightmaps = new LightmapData[lightmapCount];

			var serializedObject = new SerializedObject(this);
			var iterator = serializedObject.GetIterator();
			var hasNext = true;
			// ReSharper disable once AssignmentInConditionalExpression
			while (hasNext && (hasNext = iterator.NextVisible(true))) {
				var propertyPath = iterator.propertyPath;
				if (propertyPath.Contains("occlusionCullingSettings"))
					ApplySettingFromProperty(OcclusionCullingProperties, iterator);

#if UNITY_5 || UNITY_5_3_OR_NEWER
				if (propertyPath.Contains("renderSettings"))
					ApplySettingFromPropertyToObject(sceneRenderSettings, iterator);
#else
				if (propertyPath.Contains("renderSettings"))
					CaptureSettingToProperty(RenderSettingsProperties, iterator);
#endif

				if (propertyPath.Contains("lightmapSettings"))
					CaptureSettingToProperty(LightmapSettingsProperties, iterator);

#if !(UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0)
				if (propertyPath.Contains("navMeshBuildSettings"))
					ApplySettingFromPropertyToObject(sceneNavMeshBuildSettings, iterator, "m_BuildSettings.");

				if (propertyPath == "lightmapSettings.lightmaps") {
					if (iterator.arraySize == 0)
						continue;

					iterator.NextVisible(true); // Move past ArraySize
					iterator.NextVisible(true); // Move past first element
					var depth = iterator.depth;
					for (var i = 0; i < lightmapCount; i++) {
						var lightmapData = lightmaps[i];
						// ReSharper disable once AssignmentInConditionalExpression
						while (hasNext = iterator.NextVisible(true)) {
							if (iterator.depth <= depth)
								break;
							// Unused variable will allow for other array types in the future (like NavMeshBuildSettings)
							propertyPath = iterator.propertyPath;
							ApplySettingFromProperty(LightmapDataProperties, iterator, lightmapData);
						}
					}
				}
#if !UNITY_3_5
				if (propertyPath.Contains("lightmapEditorSettings"))
					ApplySettingFromPropertyToObject(sceneLightmapEditorSettings, iterator, "m_LightmapEditorSettings.");

				if (propertyPath.Contains("giSettings"))
					ApplySettingFromPropertyToObject(sceneLightmapEditorSettings, iterator, "m_GISettings.");
#endif
#endif

				if (!hasNext)
					break;
			}

#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
			sceneRenderSettings.ApplyModifiedProperties();
			sceneLightmapEditorSettings.ApplyModifiedProperties();
#endif

#if !(UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0)
			sceneNavMeshBuildSettings.ApplyModifiedProperties();

#if UNITY_5_5_OR_NEWER
			var sun = GameObject.Find(serializedObject.FindProperty("renderSettings.sunPath").stringValue);
			if (sun)
				UnityEngine.RenderSettings.sun = sun.GetComponent<Light>();
#endif

			InternalEditorUtility.RepaintAllViews();
#endif
		}

		static void ApplySettingFromPropertyToObject(SerializedObject obj, SerializedProperty iterator, string prefix = "") {
			var property = obj.FindProperty(prefix + iterator.name);

			if (property != null)
				Util.SetProperty(iterator, property);
		}

		static void ApplySettingFromProperty(PropertyInfo[] properties, SerializedProperty iterator, object obj = null) {
			foreach (var property in properties) {
				if (iterator.name != property.Name)
					continue;

				var propertyType = iterator.propertyType;
				switch (propertyType) {
					case SerializedPropertyType.Float:
						property.SetValue(obj, iterator.floatValue, null);
						break;
					case SerializedPropertyType.Integer:
						property.SetValue(obj, iterator.intValue, null);
						break;
					case SerializedPropertyType.Boolean:
						property.SetValue(obj, iterator.boolValue, null);
						break;
					case SerializedPropertyType.Color:
						property.SetValue(obj, iterator.colorValue, null);
						break;
					case SerializedPropertyType.Enum:
						var values = Enum.GetValues(property.PropertyType);
						var index = iterator.enumValueIndex;
						if (index >= 0 && values.Length > index)
							property.SetValue(obj, values.GetValue(iterator.enumValueIndex), null);
						break;
					case SerializedPropertyType.ObjectReference:
						property.SetValue(obj, iterator.objectReferenceValue, null);
						break;
					case SerializedPropertyType.Generic: break;
					default:
						Debug.Log("can't handle " + propertyType);
						break;
				}
			}
		}

		public static SceneData RecaptureSettings(bool isMine) {
#if !UNITY_4_2 && !UNITY_4_3
			var path = isMine ? MyTempSettingsPath : TheirTempSettingsPath;
			var sceneData = (SceneData) AssetDatabase.LoadAssetAtPath(path, typeof(SceneData));

			// Instantiate to allow setting scene objects and editing after deletion
			// ReSharper disable once RedundantCast
			sceneData = (SceneData) Instantiate(sceneData); // Cast required for Unity < 5

			return sceneData;
#else
			return CreateInstance<SceneData>(); // TODO: Figure out SerializedObject bug for Unity 4.2 & 4.3
#endif
		}
	}
}
