using System;
using UnityEngine;

namespace NiftyGlyphs
{
    [Serializable]
    public class UnicodeGlyphData
    {
        [SerializeField] private Font _font;
        [SerializeField] private char _character;
        [SerializeField] private string _string;
        [SerializeField] private int _index;

        public char Character => _character;

        public string String => _string;
        public int Index => _index;

        public Font Font => _font;
        public string Name;
        
        public void SetFont(Font font)
        {
            _font = font;
        }

        public UnicodeGlyphData(int index, Font font = null)
        {
            _index = index;
            _character = Convert.ToChar(index);
            _string = _character.ToString();
            _font = font;
            Name = "";
        }
        
        public UnicodeGlyphData(char character, Font font = null)
        {
            _index = Convert.ToInt32(character);
            _character = Convert.ToChar(_index);
            _string = _character.ToString();
            _font = font;
            Name = "";
        }
    }
}