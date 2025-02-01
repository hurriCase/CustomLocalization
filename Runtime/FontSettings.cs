using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [CreateAssetMenu(fileName = "FontSettings", menuName = "Localization/FontSettings", order = 1)]
    internal sealed class FontSettings : ScriptableSingleton<FontSettings>
    {
        [field: SerializeField] internal LanguageFontMapping[] FontMappings { get; private set; }
    }
}