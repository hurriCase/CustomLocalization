using UnityEngine;
using UnityEngine.UI;

namespace CustomLocalization.Runtime
{
    /// <summary>
    ///     Localize dropdown component.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    internal sealed class LocalizedDropdown : MonoBehaviour
    {
        [field: SerializeField] internal string[] LocalizationKeys { get; private set; }

        internal void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        internal void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            var dropdown = GetComponent<Dropdown>();

            for (var i = 0; i < LocalizationKeys.Length; i++)
                dropdown.options[i].text = LocalizationManager.Localize(LocalizationKeys[i]);

            if (dropdown.value < LocalizationKeys.Length)
                dropdown.captionText.text = LocalizationManager.Localize(LocalizationKeys[dropdown.value]);
        }
    }
}