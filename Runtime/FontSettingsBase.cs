using UnityEngine;

namespace CustomLocalization.Runtime
{
    public abstract class FontSettingsBase : ScriptableObject
    {
        internal static FontSettingsBase Instance { get; private set; }

        private void OnEnable() => Instance ??= this;

        [field: SerializeField] public LanguageFontMapping[] FontMappings { get; private set; }
    }
}