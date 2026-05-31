using UnityEditor;
using UnityEngine;

namespace Cog.Editor
{
    [CustomEditor(typeof(CogNPC))]
    public class CogNPCInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var npc = (CogNPC)target;

            // Identity section
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            if (string.IsNullOrEmpty(npc.npcId))
                EditorGUILayout.HelpBox("NPC ID will be auto-generated at runtime.", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("npcId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("profile"));

            EditorGUILayout.Space();

            // Behavior section
            EditorGUILayout.LabelField("Behavior", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoObserve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thinkCooldown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("observationRadius"));

            EditorGUILayout.Space();

            // Live state (read-only)
            EditorGUILayout.LabelField("Runtime State", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isThinking"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lastAction"));
            GUI.enabled = true;

            // Actions
            EditorGUILayout.Space();
            if (GUILayout.Button("Test Think"))
            {
                if (Application.isPlaying)
                    npc.Observe("Test observation from inspector", ObservationCategory.NpcThought);
                else
                    Debug.LogWarning("Enter Play Mode to test NPC thinking.");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
