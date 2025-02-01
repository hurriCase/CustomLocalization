using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [CreateAssetMenu(fileName = "FontSettings", menuName = "Localization/FontSettings", order = 1)]
    internal sealed class FontSettings : ScriptableSingleton<FontSettings>
    {
        [SerializeField] private LanguageFontMapping[] _fontMappings;

        internal bool TryGetFontForLanguage(string language, out LanguageFontMapping fontMapping)
        {
            fontMapping = null;

            if (_fontMappings == null) return false;

            foreach (var mapping in _fontMappings)
            {
                if (mapping.Language.Equals(language, StringComparison.OrdinalIgnoreCase) is false)
                    continue;

                fontMapping = mapping;
                return true;
            }

            return false;
        }
    }
}