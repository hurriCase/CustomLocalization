using UnityEngine;
using UnityEngine.UI;

namespace CustomLocalization.Runtime
{
    /// <summary>
    ///     Localize text component.
    /// </summary>
    [RequireComponent(typeof(Text))]
    internal sealed class LocalizedText : MonoBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; private set; }

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
            GetComponent<Text>().text = LocalizationManager.Localize(LocalizationKey);
        }
    }
}