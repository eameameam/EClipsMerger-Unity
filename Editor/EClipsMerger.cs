﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EClipsMerger : EditorWindow
    {
        private readonly List<AnimationClip> _clipsToMerge = new List<AnimationClip>();
        private string _savePath = "Assets/";

        [MenuItem("Escripts/EClip Merger")]
        public static void ShowWindow()
        {
            GetWindow<EClipsMerger>("EClips Merger");
        }

        private void OnGUI()
        {
            GUILayout.Space(5);

            GUILayout.Label("Save Path", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(_savePath);
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
            {
                _savePath = EditorUtility.SaveFolderPanel("Save merged clip to folder", _savePath, "");
                if (!string.IsNullOrEmpty(_savePath))
                {
                    int assetsIndex = _savePath.IndexOf("Assets", StringComparison.Ordinal);
                    if (assetsIndex >= 0)
                    {
                        _savePath = _savePath.Substring(assetsIndex);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
        
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Animation Clips", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                _clipsToMerge.Add(null);
            }
        
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < _clipsToMerge.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _clipsToMerge[i] = (AnimationClip)EditorGUILayout.ObjectField(_clipsToMerge[i], typeof(AnimationClip), false);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _clipsToMerge.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.Width(100));
            GUI.Box(dropArea, "Drag\nTo Fill\nThe List of Clips");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        _clipsToMerge.Clear();
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            AnimationClip clip = draggedObject as AnimationClip;
                            if (clip != null)
                            {
                                _clipsToMerge.Add(clip);
                            }
                        }
                    }
                    break;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Merge Clips"))
            {
                if (_clipsToMerge.Count > 0 && !string.IsNullOrEmpty(_savePath))
                {
                    EClipMergerUtility.MergeClips(_clipsToMerge, _savePath);
                }
                else
                {
                    Debug.LogError("Please add clips to merge and specify a save path.");
                }
            }
        }
    }
}