using UnityEngine;

namespace NiftyGlyphs
{
    [CreateAssetMenu(fileName = "UnicodeGlyphSet", menuName = "NiftyCore/UnicodeGlyphSet", order = 1)]
    public class UnicodeGlyphSetConfig : ScriptableObject
    {
        [SerializeField] private UnicodeGlyphMap _glyphMap = new UnicodeGlyphMap();
        public UnicodeGlyphMap GlyphMap => _glyphMap;
        
        //https://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt
        
        public static UnicodeGlyphSetConfig CreateInstance(UnicodeGlyphMap glyphMap)
        {
            var instance = CreateInstance<UnicodeGlyphSetConfig>();
            instance._glyphMap = glyphMap;
            return instance;
        }
        
        [ContextMenu("Import All")]
        public void ImportAll()
        {
            Font font = _glyphMap.Font;
            _glyphMap.Clear();
            _glyphMap.AddAll();
            _glyphMap.SetFont(font);
            _glyphMap.SerializeCache();
        }
        
        [ContextMenu("Import Private")]
        public void ImportPrivate()
        {
            Font font = _glyphMap.Font;
            _glyphMap.Clear();
            _glyphMap.Add(Unicode.PrivateUseArea);
            _glyphMap.SetFont(font);
            _glyphMap.SerializeCache();
        }
        
        [ContextMenu("Import Latin")]
        public void ImportLatin()
        {
            Font font = _glyphMap.Font;
            _glyphMap.Clear();
            _glyphMap.Add(Unicode.LatinRanges);
            _glyphMap.SetFont(font);
            _glyphMap.SerializeCache();
        }
    }
}