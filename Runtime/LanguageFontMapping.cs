using System;
using TMPro;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [Serializable]
    public class LanguageFontMapping
    {
        [SerializeField] internal string Language;
        [SerializeField] internal TMP_FontAsset Font;
    }
}