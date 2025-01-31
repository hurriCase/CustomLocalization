using System;
using UnityEngine;

namespace CustomLocalization.Runtime
{
    [Serializable]
    internal class Sheet
    {
        internal string Name;
        internal long Id;
        internal TextAsset TextAsset;
    }
}