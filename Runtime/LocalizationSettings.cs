using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using CustomExtensions.Editor;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace CustomLocalization.Runtime
{
    internal sealed class LocalizationSettings : ScriptableObject
    {
        internal static LocalizationSettings Instance => _instance ?? (_instance = LoadSettings());

        private static LocalizationSettings _instance;

        /// <summary>
        ///     Table Id on Google Sheets.
        ///     Let's say your table has the following URL
        ///     https://docs.google.com/spreadsheets/d/1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4/edit#gid=331980525
        ///     In this case, Table Id is "1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4" and Sheet Id is "331980525" (the gid
        ///     parameter).
        /// </summary>
        [field: SerializeField]
        internal string TableId { get; set; }

        [field: SerializeField] internal List<Sheet> Sheets { get; set; } = new();

        [field: SerializeField] public LanguageFontMapping[] FontMappings { get; private set; }

        private static DateTime Timestamp { get; set; }

        private static readonly string _urlPattern =
            "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

        private static readonly string _resourcesFolderPath =
            "Assets/CustomLocalization/Resources/Localization";

        private static readonly string _localizationSettingsFolderPath =
            "Assets/CustomLocalization/Resources";

        private static readonly string _localizationSettingsAssetName = "LocalizationSettings.asset";

        private Object _saveFolder;

        private static LocalizationSettings LoadSettings()
        {
            var assetPath = $"{_localizationSettingsFolderPath}/{_localizationSettingsAssetName}";

            var settings = Resources.Load<LocalizationSettings>(
                Path.GetFileNameWithoutExtension(assetPath));

            if (settings)
                return settings;

#if UNITY_EDITOR

            settings = CreateInstance<LocalizationSettings>();

            if (AssetDatabase.IsValidFolder(_localizationSettingsFolderPath) is false)
                _localizationSettingsFolderPath.CreateFolder();

            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

#else
            throw new Exception($"Localization settings not found: {_localizationSeettingsFolderPath}");

#endif

            return settings;
        }

#if UNITY_EDITOR

        private void Awake()
        {
            if (Sheets == null || _saveFolder == null)
                Reset();
        }

        private void DownloadGoogleSheets(Action callback = null)
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutineOwnerless(DownloadGoogleSheetsCoroutine(callback));
#endif
        }

        private IEnumerator DownloadGoogleSheetsCoroutine(Action callback = null, bool silent = false)
        {
            if (string.IsNullOrEmpty(TableId) || Sheets.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                yield break;
            }

            if (_saveFolder == null)
            {
                Reset();

                if (_saveFolder == null)
                {
                    EditorUtility.DisplayDialog("Error", "Save Folder is not set.", "OK");
                    yield break;
                }
            }

            if (Sheets.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "Sheets are empty.", "OK");
                yield break;
            }

            if ((DateTime.UtcNow - Timestamp).TotalSeconds < 2)
                if (EditorUtility.DisplayDialog("Message", "Too many requests! Try again later.", "OK"))
                    yield break;

            Timestamp = DateTime.UtcNow;

            if (silent is false) ClearSaveFolder();

            for (var i = 0; i < Sheets.Count; i++)
            {
                var sheet = Sheets[i];
                var url = string.Format(_urlPattern, TableId, sheet.Id);

                Debug.Log($"Downloading <color=grey>{url}</color>");

                var request = UnityWebRequest.Get(url);
                var progress = (float)(i + 1) / Sheets.Count;

                if (EditorUtility.DisplayCancelableProgressBar("Downloading sheets...",
                        $"[{(int)(100 * progress)}%] [{i + 1}/{Sheets.Count}] Downloading {sheet.Name}...", progress))
                    yield break;

                yield return request.SendWebRequest();

                var error = request.error ?? (request.downloadHandler.text.Contains("signin/identifier")
                    ? "It seems that access to this document is denied."
                    : null);

                if (string.IsNullOrEmpty(error))
                {
                    var path = Path.Combine(AssetDatabase.GetAssetPath(_saveFolder), sheet.Name + ".csv");

                    File.WriteAllBytes(path, request.downloadHandler.data);
                    AssetDatabase.Refresh();
                    Sheets[i].TextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    EditorUtility.SetDirty(this);
                    Debug.LogFormat(
                        $"Sheet <color=yellow>{sheet.Name}</color> ({sheet.Id}) saved to <color=grey>{path}</color>");
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error", error.Contains("404") ? "Table Id is wrong!" : error, "OK");

                    yield break;
                }
            }

            yield return null;

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

            callback?.Invoke();

            if (silent is false)
                EditorUtility.DisplayDialog("Message", $"{Sheets.Count} localization sheets downloaded!", "OK");
            yield break;

            void ClearSaveFolder()
            {
                var files = Directory.GetFiles(AssetDatabase.GetAssetPath(_saveFolder));

                foreach (var file in files)
                    File.Delete(file);
            }
        }

        private void OpenGoogleSheets()
        {
            if (string.IsNullOrEmpty(TableId))
                Debug.LogWarning("Table ID is empty.");
            else
                Application.OpenURL(string.Format(SpreedSettings.TableUrlPattern, TableId));
        }

        internal void Reset()
        {
            if (AssetDatabase.IsValidFolder(_resourcesFolderPath) is false)
                _resourcesFolderPath.CreateFolder();

            _saveFolder = AssetDatabase.LoadAssetAtPath<Object>(_resourcesFolderPath);
        }

        private void ResolveGoogleSheets()
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutineOwnerless(ResolveGoogleSheetsCoroutine());
            return;
#endif

            IEnumerator ResolveGoogleSheetsCoroutine()
            {
                if (string.IsNullOrEmpty(TableId))
                {
                    EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");

                    yield break;
                }

                using var request = UnityWebRequest.Get($"{SpreedSettings.SheetResolverUrl}?tableUrl={TableId}");

                if (EditorUtility.DisplayCancelableProgressBar("Resolving sheets...", "Executing Google App Script...",
                        1))
                    yield break;

                yield return request.SendWebRequest();

                EditorUtility.ClearProgressBar();

                if (request.error == null)
                {
                    var error = GetInternalError(request);

                    if (error != null)
                    {
                        EditorUtility.DisplayDialog("Error", "Table not found or public read permission not set.",
                            "OK");

                        yield break;
                    }

                    var sheetsDict =
                        JsonConvert.DeserializeObject<Dictionary<string, long>>(request.downloadHandler.text);

                    if (sheetsDict == null) throw new NullReferenceException(nameof(sheetsDict));

                    Sheets.Clear();

                    foreach (var item in sheetsDict)
                        Sheets.Add(new Sheet { Id = item.Value, Name = item.Key });

                    EditorUtility.DisplayDialog("Message",
                        $"{Sheets.Count} sheets resolved: {string.Join(", ", Sheets.Select(i => i.Name))}.", "OK");
                }
                else
                    throw new Exception(request.error);
            }
        }

        private static string GetInternalError(UnityWebRequest request)
        {
            var matches = Regex.Matches(request.downloadHandler.text, @">(?<Message>.+?)<\/div>");

            if (matches.Count == 0 && request.downloadHandler.text.Contains("Google Script ERROR:") is false)
                return null;

            var error = matches.Count > 0
                ? matches[1].Groups["Message"].Value.Replace("quot;", "")
                : request.downloadHandler.text;

            return error;
        }

        internal void DisplayHelp()
        {
            EditorGUILayout.HelpBox(
                "1. Set Table Id and Save Folder\n2. Press Resolve Sheets*\n3. Press Download Sheets\n*You can set Sheets manually: fill Name and Id, leave Text Asset empty",
                MessageType.None);
        }

        internal void DisplayButtons()
        {
            var buttonStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fixedHeight = 30 };

            if (GUILayout.Button("↺ Resolve Sheets", buttonStyle))
                ResolveGoogleSheets();

            if (GUILayout.Button("▼ Download Sheets", buttonStyle))
                DownloadGoogleSheets();

            if (GUILayout.Button("❖ Open Google Sheets", buttonStyle))
                OpenGoogleSheets();
        }

        public void DisplayWarnings()
        {
            if (TableId == "")
                EditorGUILayout.HelpBox("Table Id is empty.", MessageType.Warning);
            else if (_saveFolder == null)
                EditorGUILayout.HelpBox("Save Folder is not set.", MessageType.Warning);
            else if (Sheets.Count == 0)
                EditorGUILayout.HelpBox("Sheets are empty.", MessageType.Warning);
            else if (Sheets.Any(i => i.TextAsset is null))
                EditorGUILayout.HelpBox("Sheets are not downloaded.", MessageType.Warning);
        }

#endif
    }
}