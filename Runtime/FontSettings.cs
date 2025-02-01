using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [CreateAssetMenu(fileName = "FontSettings", menuName = "Localization/FontSettings", order = 1)]
    public sealed class FontSettings : ScriptableSingleton<FontSettings>
    {
        [field: SerializeField] public LanguageFontMapping[] FontMappings { get; private set; }
    }
}