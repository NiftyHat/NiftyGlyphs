using System;
using UnityEngine;

namespace NiftyGlyphs
{
    [Serializable]
    public class UnicodeGlyphDataWrapper : ScriptableObject
    {
        [SerializeField] public UnicodeGlyphData Data;
    }
}