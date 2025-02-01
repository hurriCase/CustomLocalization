using UnityEngine;

namespace CustomLocalization.Runtime
{
    public abstract class FontSettings : ScriptableObject
    {
        internal static FontSettings Instance { get; private set; }

        private void OnEnable() => Instance ??= this;

        [field: SerializeField] public LanguageFontMapping[] FontMappings { get; private set; }
    }
}