using System;
using TMPro;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [Serializable]
    public class LanguageFontMapping
    {
        [SerializeField] public string Language;
        [SerializeField] public TMP_FontAsset Font;
    }
}