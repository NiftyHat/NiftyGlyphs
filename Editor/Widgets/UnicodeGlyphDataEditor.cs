using System;
using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    [CustomEditor(typeof(UnicodeGlyphDataWrapper))]
    public class UnicodeGlyphDataEditor : Editor
    {
        protected char _character;
        protected CharacterInfo _info;
        protected Font _font;
        protected GUIStyle _previewStyle;
        private string _renameString = null;
        private SerializedProperty _spData;
        private SerializedProperty _spName;
        private SerializedProperty _spTags;
        

        /*
        public void Set(UnicodeGlyphData data, UnicodeGlyphSetConfig config)
        {
            _data = data;
            _character = data.Character;
            _renameString = null;
            var font = config.GlyphMap.Font;
            if (config.GlyphMap.Font != null)
            {
                font.RequestCharactersInTexture(_character.ToString());
                font.GetCharacterInfo(_character, out var info, font.fontSize);
                _info = info;
                SetFont(font);
            }
        }*/

        private void OnEnable()
        {
            /*
            if (serializedObject.targetObject != null)
            {
                _spData = serializedObject.FindProperty("Data");
                _spName = serializedObject.FindProperty("Data.Name");
                _spTags = serializedObject.FindProperty("Data._tagSet");
                UnicodeGlyphDataWrapper dataWrapper = target as UnicodeGlyphDataWrapper;
                _character = dataWrapper.Data.Character;
                _font = dataWrapper.Data.Font;
                if (_font != null)
                {
                
                    _font.RequestCharactersInTexture(_character.ToString());
                    _font.GetCharacterInfo(_character, out var info, _font.fontSize);
                    _info = info;
                    SetFont(_font);
                }
            }*/
        }


        public void SetFont(Font font)
        {
            if (_previewStyle == null)
            {
                _previewStyle = new GUIStyle(GUI.skin.box);
                _previewStyle.fontSize = 30;
                _previewStyle.alignment = TextAnchor.MiddleCenter;
            }
            if (_previewStyle != null)
            {
                _previewStyle.font = font;
            }
        }

        private void HandleKeyboardEvents()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Return)
                {
                    if (_renameString != null)
                    {
                        _spName.stringValue = _renameString;
                        _renameString = null;
                    }
                }

                if (e.keyCode == KeyCode.Escape)
                {
                    _renameString = null;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            Draw();
            //base.OnInspectorGUI();
        }

        public void Draw()
        {
            if (_previewStyle == null)
            {
                _previewStyle = new GUIStyle(GUI.skin.box);
                _previewStyle.fontSize = 30;
                _previewStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (_spData == null)
            {
                return;
            }

            HandleKeyboardEvents();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(100));
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.SelectableLabel(_character.ToString(), _previewStyle, GUILayout.Width(50),
                        GUILayout.Height(50));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.SelectableLabel(Convert.ToInt32((_character)).ToString("X"),
                        GUILayout.Height(14));
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUIContent editButtonContent = EditorGUIUtility.IconContent("d_editicon.sml");
                        if (GUILayout.Button(editButtonContent, GUILayout.Width(30)))
                        {
                            _renameString = _spName.stringValue;
                            GUI.FocusControl("RenameField");
                        }

                        if (_renameString != null)
                        {
                            GUI.SetNextControlName("RenameField");
                            _renameString = GUILayout.TextField(_renameString);
                        }
                        else
                        {
                            EditorGUILayout.SelectableLabel(_spName.stringValue, EditorStyles.boldLabel, GUILayout.Height(14));
                        }
                    }
                    //EditorGUILayout.PropertyField(_data.Tags);
                    EditorGUILayout.EndHorizontal();
                    SerializedObject so = new SerializedObject(this);
                    EditorGUILayout.PropertyField(_spTags);
                    //_data.TagSet = _tagSet;
                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                {
                    var s = $"Size: {_info.glyphWidth}x{_info.glyphHeight}";
                    s += $"\nMaxX:{_info.maxX} MaxY:{_info.maxY}";
                    s += $"\nMinX:{_info.minX} MinY:{_info.minY}";
                    s += $"\nAdvance: {_info.advance} Bearing: {_info.bearing}";
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox(s, MessageType.None);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
            
        }
    }
}