using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    public class UnicodeGlyphMapWizard : ScriptableWizard
    {
        [SerializeField] public Font Font;
        [SerializeField] public Unicode.Range[] SelectedRange;
        
        public static Action<UnicodeGlyphMap> OnCreateCallback;

        [MenuItem("GameObject/Creat Unicode Glyph Set")]
        private static UnicodeGlyphMapWizard CreateWizard()
        {
            return ScriptableWizard.DisplayWizard<UnicodeGlyphMapWizard>("Create Glyph Set", "Create");
        }
        
        public static UnicodeGlyphMapWizard CreateWizard(Action<UnicodeGlyphMap> callback)
        {
            OnCreateCallback = callback;
            return ScriptableWizard.DisplayWizard<UnicodeGlyphMapWizard>("Create Glyph Set", "Create");
        }

        private void DrawUnicodeSelector()
        {

        }

        private void OnEnable()
        {
            //_unicodeRangeSelectMenu = new UnicodeUtils.UnicodeRangeSelectMenu(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(HashSet<Unicode.Range> obj)
        {
            SelectedRange = obj.ToArray();
            //throw new NotImplementedException();
        }

        protected override bool DrawWizardGUI()
        {
            if (GUILayout.Button("Select Range"))
            {
                //_unicodeRangeSelectMenu.ShowAsContext();
            }
            return base.DrawWizardGUI();
        }

        void OnWizardCreate()
        {
            if (Font == null)
            {
                errorString = "Select a font!";
                return;
            }
            UnicodeGlyphMap unicodeGlyphMap = new UnicodeGlyphMap();
            foreach (var item in SelectedRange)
            {
                unicodeGlyphMap.Add(item);
            }
            unicodeGlyphMap.SetFont(Font);
            unicodeGlyphMap.SerializeCache();
            if (OnCreateCallback != null)
            {
                OnCreateCallback?.Invoke(unicodeGlyphMap);
            }
            
        }
        
        void OnWizardUpdate()
        {
            if (Font == null)
            {
                helpString = "Select a font!";
            }
        }
        
        // When the user presses the "Apply" button OnWizardOtherButton is called.
        void OnWizardOtherButton()
        {
            
        }
    }
}