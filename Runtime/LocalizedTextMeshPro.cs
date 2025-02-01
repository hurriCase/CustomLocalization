using TMPro;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    /// <summary>
    ///     Localize text component with font support.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    internal sealed class LocalizedTextMeshPro : MonoBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; private set; }

        private TMP_Text _textComponent;
        private TMP_FontAsset _originalFont;

        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();

            _originalFont = _textComponent.font;
        }

        private void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            _textComponent ??= GetComponent<TMP_Text>();

            _textComponent.text = LocalizationManager.Localize(LocalizationKey);

            var isFontSpecified =
                FontSettings.instance.TryGetFontForLanguage(LocalizationManager.Language, out var languageFontMapping);

            _textComponent.font = isFontSpecified ? languageFontMapping.Font : _originalFont;
        }


        public void ResetFont()
        {
            _textComponent ??= GetComponent<TMP_Text>();

            _textComponent.font = _originalFont;
        }
    }
}