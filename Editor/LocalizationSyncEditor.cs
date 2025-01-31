using CustomLocalization.Runtime;
using UnityEditor;
using UnityEngine;

namespace CustomLocalization.Editor
{
    /// <summary>
    ///     Adds "Sync" button to LocalizationSync script.
    /// </summary>
    [CustomEditor(typeof(LocalizationSync))]
    internal sealed class LocalizationSyncEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (LocalizationSync)target;

            if (GUILayout.Button("Sync"))
                component.Sync();
        }
    }
}