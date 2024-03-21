using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class EClipsMergerUtility
    {
        public static AnimationClip MergeClips(List<AnimationClip> clipsToMerge, string savePath, string mergedClipName, int priorityClipIndex)
        {
            if (clipsToMerge == null || clipsToMerge.Count == 0)
            {
                Debug.LogError("No clips to merge.");
                return null;
            }

            AnimationClip mergedClip = new AnimationClip();
            HashSet<string> addedProperties = new HashSet<string>();

            if (priorityClipIndex >= 0 && priorityClipIndex < clipsToMerge.Count)
            {
                var priorityClip = clipsToMerge[priorityClipIndex];
                clipsToMerge.RemoveAt(priorityClipIndex);
                clipsToMerge.Insert(0, priorityClip);
            }

            foreach (AnimationClip clip in clipsToMerge)
            {
                if (clip == null) continue;

                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                {
                    if (!addedProperties.Contains(binding.path + binding.propertyName))
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                        AnimationUtility.SetEditorCurve(mergedClip, binding, curve);
                        addedProperties.Add(binding.path + binding.propertyName);
                    }
                }

                AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
                foreach (var evt in events)
                {
                    mergedClip.AddEvent(evt);
                }
            }

            string fullPath = AssetDatabase.GenerateUniqueAssetPath($"{savePath}/{mergedClipName}.anim");
            AssetDatabase.CreateAsset(mergedClip, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Merged clip saved to: " + fullPath);

            return mergedClip;
        }
    }
}