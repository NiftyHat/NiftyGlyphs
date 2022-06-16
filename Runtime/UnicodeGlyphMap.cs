using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NiftyGlyphs
{
    [Serializable]
    public class UnicodeGlyphMap : ISerializationCallbackReceiver
    {
        [SerializeField] private Font _font;
        [SerializeField] private UnicodeGlyphData[] _serializedCache = Array.Empty<UnicodeGlyphData>();
        [SerializeField] private bool _includesInvisibleCharacters = false;
        private Dictionary<char, UnicodeGlyphData> _cache = new Dictionary<char, UnicodeGlyphData>();
        private HashSet<char> _missingGlyphs = new HashSet<char>();
        
        public Font Font => _font;
        public IEnumerable<char> Keys => _cache.Keys.ToList();
        public IEnumerable<UnicodeGlyphData> Values => _cache.Values.ToList();

        public UnicodeGlyphMap(char[] characters, Font font)
        {
            _font = font;
            if (_font != null)
            {
                _font.RequestCharactersInTexture(new string(characters));
            }
            foreach (var item in characters)
            {
                UpdateCache(font, item);
            }
        }

        public bool TryFilterGlyphs(Predicate<UnicodeGlyphData> predicate, out UnicodeGlyphData[] list)
        {
            bool hasItems = false;
            list = new UnicodeGlyphData[_cache.Count];
            int length = 0;
            foreach (var kvp in _cache)
            {
                var cacheData = kvp.Value;
                if (predicate(cacheData))
                {
                    list[length] = kvp.Value;
                    hasItems = true;
                    length++;
                }
            }
            Array.Resize(ref list, length);
            return hasItems;
        }

        public UnicodeGlyphMap(Font font)
        {
            _font = font;
            _cache = new Dictionary<char, UnicodeGlyphData>();
        }
        
        public UnicodeGlyphMap()
        {
            _cache = new Dictionary<char, UnicodeGlyphData>();
        }

        public void Clear()
        {
            _font = null;
            _cache.Clear();
            Array.Clear(_serializedCache, 0,0);
        }
        
        public void OnBeforeSerialize()
        {
            _cache?.Clear();
            foreach (var item in _serializedCache)
            {
                if (!_cache.ContainsKey(item.Character))
                {
                    _cache.Add(item.Character, item);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            _cache?.Clear();
            foreach (var item in _serializedCache)
            {
                if (!_cache.ContainsKey(item.Character))
                {
                    _cache.Add(item.Character, item);
                }
            }
        }

        public void SerializeCache()
        {
            _serializedCache = _cache.Values.ToArray();
        }

        private static int GetFontImportSize(Font font)
        {
            if (font.dynamic)
            {
                return 2;
            }
            return font.fontSize;
        }

        public void SetFont(Font font)
        {
            if (_font != null && font != _font)
            {
                ClearCache();
            }
            //RequestCharactersInTexture is a huge baby that will just print debug error on a Surrogate character and not
            //something useful, like a handleable exception. So we pre-prune them so RequestCharactersInTexture doesn't just fail
            string allCharacters = new string(_cache.Keys.Where(key => !char.IsSurrogate(key)).ToArray());
            _font = font;
            int importSize = GetFontImportSize(_font);
            _font.RequestCharactersInTexture(allCharacters, importSize);
            foreach (var character in allCharacters)
            {
                UpdateCache(_font, character);
            }
            if (_missingGlyphs.Count > 0)
            {
                Debug.Log($"Added {_missingGlyphs.Count} are missing front font {_font.name}");
            }
            Debug.Log($"Added {_cache.Values.Count} from {allCharacters.Length}");
        }

        private void ClearCache()
        {
            if (_cache != null)
            {
                _cache.Clear();
            }
            else
            {
                _cache = new Dictionary<char, UnicodeGlyphData>();
            }
        }

        private void UpdateCache(Font font, char character)
        {
            if (font != null && !_includesInvisibleCharacters)
            {
                font.GetCharacterInfo(character, out var info, GetFontImportSize(font));
                if (IsEmptyCharacter(info))
                {
                    if (_cache.TryGetValue(character, out var data))
                    {
                        if (string.IsNullOrEmpty(data.Name))
                        {
                            if (_cache.ContainsKey(character))
                            {
                                _cache.Remove(character);
                            }
                        }
                        else
                        {
                            _missingGlyphs.Add(character);
                        }
                    }
                }
                else
                {
                    if (_cache.TryGetValue(character, out var data))
                    {
                        if (data.Font != font)
                        {
                            data.SetFont(font);
                        }
                    }
                    else
                    {
                        _cache.Add(character, new UnicodeGlyphData(character, _font));
                    }
                }
            }
            else
            {
                if (!_cache.ContainsKey(character))
                {
                    _cache.Add(character, new UnicodeGlyphData(character, _font));
                }
            }
        }

        public void Add(char character)
        {
            if (_font)
            {
                UpdateCache(_font, character);
            }
        }
        public void Add(char[] characterList)
        {
            foreach (var item in characterList)
            {
                UpdateCache(_font, item);
            }
        }
        
        public void Add(Unicode.Range range)
        {
            for (int i = range.Start; i <= range.End; i++)
            {
                char character = Convert.ToChar(i);
                UpdateCache(_font, character);
            }
        }
        
        public void Add(Unicode.Range[] rangeList)
        {
            foreach (var item in rangeList)
            {
                Add(item);
            }
        }
        
        public void Add(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                char character = Convert.ToChar(i);
                UpdateCache(_font, character);
            }
        }
        
        public void AddAll()
        {
            Add(0, ushort.MaxValue);
        }
        
        private static bool IsEmptyCharacter(CharacterInfo info)
        {
            return info.advance <= 0;
        }

        public bool TryGet(char character, out UnicodeGlyphData data)
        {
            return _cache.TryGetValue(character, out data);
        }

        public void Remove(Predicate<UnicodeGlyphData> predicate)
        {
            var keys = _cache.Keys.ToList();
            foreach (var key in keys)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    if (predicate(value))
                    {
                        _cache.Remove(key);
                    }
                }
            }
        }
    }
}