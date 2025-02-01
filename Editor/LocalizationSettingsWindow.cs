using System.Linq;
using CustomExtensions.Runtime;
using CustomLocalization.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomLocalization.Editor
{
    internal sealed class LocalizationSettingsWindow : EditorWindow
    {
        private static SerializedObject _serializedObject;
        private Vector2 _scrollPosition;

        private static LocalizationSettings SettingsBase => LocalizationSettings.Instance;

        [MenuItem("--Project--/Localization/Settings")]
        internal static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>("Localization Settings");
        }

        [MenuItem("--Project--/Localization/Reset")]
        internal static void ResetSettings()
        {
            if (EditorUtility.DisplayDialog(
                    "Simple Localization",
                    "Do you want to reset settings?",
                    "Yes",
                    "No"))
                LocalizationSettings.Instance.Reset();
        }

        private void MakeSettingsWindow()
        {
            minSize = new Vector2(300, 500);
            SettingsBase.DisplayHelp();
            SettingsBase.TableId = EditorGUILayout.TextField("Table Id", SettingsBase.TableId, GUILayout.MinWidth(200));
            DisplaySheets();
            SettingsBase.SaveFolder =
                EditorGUILayout.ObjectField("Save Folder", SettingsBase.SaveFolder, typeof(Object), false);

            var fontMappingsProperty =
                _serializedObject.FindProperty(nameof(SettingsBase.FontMappings).ConvertToBackingField());

            EditorGUILayout.PropertyField(fontMappingsProperty, includeChildren: true);

            SettingsBase.DisplayButtons();
            SettingsBase.DisplayWarnings();

            _serializedObject.ApplyModifiedProperties();
        }

        private static void DisplaySheets()
        {
            if (_serializedObject?.targetObject is null)
                _serializedObject = new SerializedObject(SettingsBase);
            else
                _serializedObject.Update();

            var property = _serializedObject
                .FindProperty(nameof(LocalizationSettings.Instance.Sheets).ConvertToBackingField());

            if (property == null)
                return;

            EditorGUILayout.PropertyField(property, new GUIContent("Sheets"), true);

            if (property.isArray)
            {
                property.Next(true);
                property.Next(true);

                var length = property.intValue;

                property.Next(true);

                SettingsBase.Sheets.Clear();

                var lastIndex = length - 1;

                for (var i = 0; i < length; i++)
                {
                    SettingsBase.Sheets.Add(new Sheet
                    {
                        Name = property.FindPropertyRelative(nameof(Sheet.Name).ConvertToBackingField()).stringValue,
                        Id = property.FindPropertyRelative(nameof(Sheet.Id).ConvertToBackingField()).longValue,
                        TextAsset = property.FindPropertyRelative(nameof(Sheet.TextAsset)
                            .ConvertToBackingField()).objectReferenceValue as TextAsset
                    });

                    if (i < lastIndex)
                        property.Next(false);
                }
            }

            _serializedObject.ApplyModifiedProperties();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            MakeSettingsWindow();

            EditorGUILayout.EndScrollView();
        }
    }
}