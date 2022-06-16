using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    [CustomPropertyDrawer(typeof(UnicodeGlyphAttribute))]
    public class UnicodeGlyphAttributeDrawer : PropertyDrawer
    {
        private const int BUTTON_SIZE = 32;
        private GUIStyle _buttonStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is UnicodeGlyphAttribute unicodeGlyphAttribute)
            {
                unicodeGlyphAttribute.TryGetData(out var font, out var config);
                
                if (font != null)
                {
                    char character;
                    
                    if (property.propertyType == SerializedPropertyType.Character && property.intValue > 0)
                    {
                        character = (char)property.intValue;
                    }
                    else if (property.propertyType == SerializedPropertyType.String && property.stringValue.Length > 0)
                    {
                        character = property.stringValue[0];
                    }
                    else
                    {
                        character = Unicode.ReplacementCharacter;
                    }
                    string displayedCharacter = character.ToString();
                    position = EditorGUI.PrefixLabel(position, label);
                    Rect buttonRect = new Rect(position.x, position.y, BUTTON_SIZE, BUTTON_SIZE);
                    Rect labelRect = new Rect(buttonRect.xMax, position.y, position.width - buttonRect.width,
                        BUTTON_SIZE);
                        DrawSelectorButton(buttonRect, property, font, displayedCharacter);
                    if (config != null && config.GlyphMap.TryGet(character, out var data))
                    {
                        DrawCharacterName(labelRect, data.Name);
                    }
                    else
                    {
                        DrawMissingConfig(labelRect, unicodeGlyphAttribute.FileName);
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, $"[UnicodeGlyph] fileName:'{unicodeGlyphAttribute.FileName}' not found in AssetDatabase");
                }
            }
        }

        private void DrawMissingConfig(Rect position, string fileName)
        {
            GUI.color = Color.yellow;
            if (GUI.Button(position, "Create Config {fileName}.asset"))
            {
                //todo make the config file
            }
        }

        private void DrawCharacterName(Rect position, string content)
        {
            GUI.Label(position, content);
        }
        
        private void DrawSelectorButton(Rect position, SerializedProperty property, Font font, string label)
        {
            if (_buttonStyle == null)
            {
                if (font.dynamic) //attempted fix to prevent unity spewing warnings about using sizes/styles with non dynamic fonts (it doesn't work, lol)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        font = font,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 16
                    };
                }
                else
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        font = font,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 0
                    };
                }
            }

            if (_buttonStyle != null)
            {
                if (GUI.Button(position, new GUIContent(label), _buttonStyle))
                {
                    ShowPicker(property);
                }
            }
        }

        private void ShowPicker(SerializedProperty property)
        {
            if (attribute is UnicodeGlyphAttribute unicodeGlyphAttribute && unicodeGlyphAttribute.TryGetConfig( out var config))
            {
                UnicodeGlyphBrowserWindow.Init(property, config);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return BUTTON_SIZE;
        }
    }
}