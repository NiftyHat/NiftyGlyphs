using System;
using UnityEngine;

namespace NiftyGlyphs
{
    public class UnicodeGlyphAttribute : PropertyAttribute
    {
        public int[] Range;
        public string FileName;

        private Font _font = null;
        private UnicodeGlyphSetConfig _config;

        public UnicodeGlyphAttribute(int[] range, string fileName)
        {
            Range = range;
            FileName = fileName;
        }

        public char[] GetCharacters()
        {
            int start = Range[0];
            int end = Range[1];
            int len = end - start;
            char[] characters = new char[len];
            for (int i = 0; i < len; i++)
            {
                characters[i] = Convert.ToChar(start + i);
            }
            return characters;
        }

        public bool TryGetConfig(out UnicodeGlyphSetConfig config)
        {
            if (_config != null)
            {
                config = _config;
                return true;
            }
            if (UnicodeGlyphUtils.TryLoad(FileName, out _config))
            {
                _font = _config.GlyphMap.Font;
                config = _config;
                return true;
            }
            config = null;
            return false;
        }
        
        public bool TryGetData(out Font font, out UnicodeGlyphSetConfig config)
        {
            if (_config != null)
            {
                font = _config.GlyphMap.Font;
                config = _config;
                return true;
            }
            if (_font != null)
            {
                font = _font;
                config = null;
                return true;
            }

            if (_config == null && _font == null)
            {
                if (UnicodeGlyphUtils.TryLoad(FileName, out _config))
                {
                    font = _font = _config.GlyphMap.Font;
                    config = _config;
                    return true;
                }
                else if (UnicodeGlyphUtils.TryLoad(FileName, out _font))
                {
                    font = _font;
                    config = null;
                    return true;
                }
                else
                {
                    //log loading issue
                }
            }
            font = null;
            config = null;
            return false;
        }

    }
}