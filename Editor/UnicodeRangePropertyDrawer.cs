using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    [CustomPropertyDrawer(typeof(Unicode.Range))]
    public class UnicodeRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spStart = property.FindPropertyRelative("_start");
            SerializedProperty spEnd = property.FindPropertyRelative("_end");
            SerializedProperty spName = property.FindPropertyRelative("Name");
            
            GUI.Label(position, $"0x{spStart.intValue:X2}->0x{spEnd.intValue:X2} {spName.stringValue}");
            /*
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
                spName.stringValue = GUI.TextField(rectName, spName.stringValue);
            }*/
        }
    }
}