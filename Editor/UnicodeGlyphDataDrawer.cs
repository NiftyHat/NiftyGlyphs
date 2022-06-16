using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    [CustomPropertyDrawer(typeof(UnicodeGlyphData))]
    public class UnicodeGlyphDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spIndex = property.FindPropertyRelative("_index");
            SerializedProperty spString = property.FindPropertyRelative("_string");
            SerializedProperty spFont = property.FindPropertyRelative("_font");
            SerializedProperty spName = property.FindPropertyRelative("Name");
            
            GUIStyle glyphDisplayStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16
            };
            if (spFont != null && spFont.objectReferenceValue != null && spFont.objectReferenceValue is Font font)
            {
                glyphDisplayStyle.font = font;
            }
            Rect rectGlyph = new Rect(position.x, position.y, 24, 24);
            Rect rectIndex = new Rect(rectGlyph.xMax , position.y, 60, position.height);
            Rect rectName = new Rect(rectIndex.xMax , position.y, position.width - rectIndex.xMax, position.height);
            if (spString != null)
            {
                GUI.Label(rectGlyph, spString.stringValue, glyphDisplayStyle);
            }

            if (spIndex != null)
            {
                GUI.Label(rectIndex, spIndex.intValue.ToString("x4"));
            }

            if (spName != null)
            {
                EditorGUI.PropertyField(rectName, spName, new GUIContent(""));
                //spName.stringValue = GUI.TextField(rectName, spName.stringValue);
            }
        }
    }
}