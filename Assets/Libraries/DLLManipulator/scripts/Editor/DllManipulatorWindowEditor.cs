﻿using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UnityNativeTool.Internal
{
    public class DllManipulatorWindowEditor : EditorWindow
    {
        [MenuItem("Window/Dll manipulator")]
        static void Init()
        {
            var window = GetWindow<DllManipulatorWindowEditor>();
            window.Show();
        }

        void OnGUI()
        {
            var dllManipulator = FindObjectOfType<DllManipulatorScript>();
            if (dllManipulator == null)
                dllManipulator = Resources.FindObjectsOfTypeAll<DllManipulatorScript>()
                    .FirstOrDefault(d => !EditorUtility.IsPersistent(d) && d.gameObject.scene.IsValid());

            if (dllManipulator != null)
            {
                var editor = Editor.CreateEditor(dllManipulator);
                editor.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"There is no {nameof(DllManipulatorScript)} script in the scene.");
            }
        }
    }
}