using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NiftyEditorMenu;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiftyGlyphs
{
    public class UnicodeGlyphBrowserWindow : EditorWindow
    {
        public enum SelectionMode
        {
            Single,
            Multiple,
            Extended,
        }
        
        public enum CopyBufferMode
        {
            Symbol,
            Code
        }

        private CopyBufferMode _copyMode;
        private UnicodeGlyphSetConfig _config;
        private PaginatedSelectionGrid<UnicodeGlyphData> _selectionGrid;
        public static char Selected;
        public static SerializedProperty DelegateProperty;
        private SearchField _searchField;
        private string _searchString;
        private TreeViewState _treeViewState;
        private AssetTreeView<UnicodeGlyphSetConfig> _treeView;
        private UnicodeGlyphDataEditor _unicodeGlyphEditor;
        private UnicodeGlyphPreview _unicodeGlyphPreview;
        [SerializeField] private UnicodeGlyphDataWrapper _unicodeGlyphWrapper;
        private static Action<UnicodeGlyphData, UnicodeGlyphSetConfig> OnItemSelected;

        [MenuItem("NiftyFramework/Unicode Glyph Browser")]
        static void Init()
        {
            var window = (UnicodeGlyphBrowserWindow)GetWindow(typeof(UnicodeGlyphBrowserWindow));
            var config = FindOrCreateConfig();
            window.titleContent = new GUIContent("Unicode Glyph Browser");
            window.SetConfig(config);
            window.Show();
        }
        
        public static void Init(UnicodeGlyphSetConfig config)
        {
            var window = (UnicodeGlyphBrowserWindow)GetWindow(typeof(UnicodeGlyphBrowserWindow));
            window.titleContent = new GUIContent("Unicode Glyph Browser");
            window.SetConfig(config);
            window.Show();
        }
        
        public static void Init(SerializedProperty delegateTarget, UnicodeGlyphSetConfig config)
        {
            var window = (UnicodeGlyphBrowserWindow)GetWindow(typeof(UnicodeGlyphBrowserWindow));
            DelegateProperty = delegateTarget;
            window.SetConfig(config);
            window.titleContent = new GUIContent("Unicode Glyph Browser");
            window.SetMode(SelectionMode.Multiple);
            window.Show();
        }
        
        public static void Init( Action<UnicodeGlyphData, UnicodeGlyphSetConfig> onSelectItem, UnicodeGlyphSetConfig config)
        {
            var window = (UnicodeGlyphBrowserWindow)GetWindow(typeof(UnicodeGlyphBrowserWindow));
            OnItemSelected = onSelectItem;
            window.titleContent = new GUIContent("Unicode Glyph Browser");
            window.SetConfig(config);
            window.Show();
        }
        
        private void HandleConfigAssetDoubleClick(UnicodeGlyphSetConfig config)
        {
            SetConfig(config);
            _treeView = null;
            _treeViewState = null;
        }

        private void HandleConfigAssetSelected(IEnumerable<Object> obj)
        {
            //throw new NotImplementedException();
        }

        private static UnicodeGlyphSetConfig FindOrCreateConfig()
        {
            var objectsInAssets = AssetDatabase.FindAssets("t:" + nameof(UnicodeGlyphSetConfig));
            if (objectsInAssets.Length == 1)
            {
                var assetId = objectsInAssets[0];
                var path = AssetDatabase.GUIDToAssetPath(assetId);
                return AssetDatabase.LoadAssetAtPath<UnicodeGlyphSetConfig>(path);
            }
            if (objectsInAssets.Length == 0)
            {
                return CreateInstance<UnicodeGlyphSetConfig>();
            }
            return null;
        }

        private static string FindBestAssetsSaveDirectory()
        {
            var objectsInAssets = AssetDatabase.FindAssets("t:" + nameof(UnicodeGlyphSetConfig));
            if (objectsInAssets.Length > 0)
            {
                Dictionary<string, int> directoryFrequency = new Dictionary<string, int>();
                string bestDirectory = "";
                int bestCount = 0;
                for (int i = 0; i < objectsInAssets.Length; i++)
                {
                    var assetId = objectsInAssets[0];
                    var fullPath = AssetDatabase.GUIDToAssetPath(assetId);
                    var directory = Path.GetDirectoryName(fullPath);
                    int currentCount = 0;
                    if (directoryFrequency.TryGetValue(directory, out currentCount))
                    {
                        directoryFrequency[directory] = currentCount + 1;
                    }
                    else
                    {
                        directoryFrequency[directory] = 1;
                        currentCount = 1;
                    }

                    if (currentCount > bestCount)
                    {
                        bestCount = currentCount;
                        bestDirectory = directory;
                    }
                }
                return bestDirectory;
            }
            return "";
        }
        
        private void OnEnable()
        {
            _unicodeGlyphPreview = new UnicodeGlyphPreview();
            if (_config == null)
            {
                DisplayAssetSelect();
            }
            else
            {
                _treeView = null;
            }
        }

        private void DisplayAssetSelect()
        {
            // Check whether there is already a serialized view state (state 
            // that survived assembly reloading)
            if (_treeViewState == null)
            {
                _treeViewState = new TreeViewState ();
            }
            _config = null;

            _treeView = new AssetTreeView<UnicodeGlyphSetConfig>(_treeViewState);
            _treeView.OnSelectionChange += HandleConfigAssetSelected;
            _treeView.OnDoubleClickItem += HandleConfigAssetDoubleClick;
            
            EditorApplication.projectChanged += HandleProjectChange;
        }

        private void HandleProjectChange()
        {
            _treeViewState = new TreeViewState ();
            if (_treeView != null)
            {
                _treeView.Reload();
            }
        }

        private void SetMode(SelectionMode selectionMode)
        {
            //_mode = selectionMode;
        }

        public void SetConfig(UnicodeGlyphSetConfig config)
        {
            _searchField = new SearchField();
            if (config == null)
            {
                return;
            }
            if (_treeView != null)
            {
                _treeView = null;
                _treeViewState = null;
            }
            _config = config;
            _selectionGrid = new PaginatedSelectionGrid<UnicodeGlyphData>(_config.GlyphMap.Values.ToArray(), HandleItemSelected, 100, 32, HandleButtonContent);
            _selectionGrid.OnItemResize += HandleGridItemResize;
            
            HandleGridItemResize(32);
        }

        private GUIContent HandleButtonContent(UnicodeGlyphData unicodeGlyphData)
        {
            return new GUIContent(unicodeGlyphData.Character.ToString());
        }

        private void HandleGridItemResize(int size)
        {
            _selectionGrid.UpdateItemStyle(currentStyle =>
            {
                currentStyle.GridItemStyle = new GUIStyle(GUI.skin.button);
                currentStyle.GridItemStyle.font = _config.GlyphMap.Font;
                currentStyle.GridItemStyle.fontSize = (int)(size *0.5f);
                currentStyle.GridItemStyleSelected.font = _config.GlyphMap.Font;
                currentStyle.GridItemStyleSelected.fontSize = (int)(size  *0.5f);
                currentStyle.GridItemStyleSelected = new GUIStyle(GUI.skin.button);
                currentStyle.GridItemStyleSelected.normal.textColor = Color.green;
            });
        }

        private void HandleCreateFromWizard()
        {
            
        }

        void DrawToolStrip()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (_treeView != null)
            {
                if (GUILayout.Button("Create", EditorStyles.toolbarButton))
                {
                    UnicodeGlyphMapWizard.CreateWizard(HandleCreateFromWizard);
                    //_config = CreateInstance<UnicodeGlyphSetConfig>();
                    //SetConfig(_config);
                }
            }
            else
            {
                if (_config != null && !AssetDatabase.Contains(_config))
                {
                    Color defaultColor = GUI.color;
                    GUI.color = Color.green;
                    if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                    {
                        string defaultFolder = Path.Combine(Application.dataPath, FindBestAssetsSaveDirectory());
                        string saveFolder = EditorUtility.SaveFilePanel("Save Folder", defaultFolder,
                            _config.GlyphMap.Font.name, "asset");
                        if (!string.IsNullOrEmpty(saveFolder))
                        {
                            string relativePath = saveFolder.Replace(Application.dataPath, "Assets/");
                            AssetDatabase.CreateAsset(_config, relativePath);
                        }
                    }
                    GUI.color = defaultColor;
                }
                if (GUILayout.Button("Select", EditorStyles.toolbarButton))
                {
                    DisplayAssetSelect();
                    //EditorGUIUtility.ShowObjectPicker<UnicodeGlyphSetConfig>(_config, false, "t:UnicodeGlyphSetConfig", 1);
                    //EditorGUIUtility.ExitGUI();
                }
            }


            if (_searchField != null)
            {
                string newSearchString = _searchField.OnToolbarGUI(_searchString);
                if (newSearchString != _searchString)
                {
                    if (newSearchString != "" && _config != null)
                    {
                        StringComparison comp = StringComparison.OrdinalIgnoreCase;
                        if (_config.GlyphMap.TryFilterGlyphs(item => item.Name.IndexOf(newSearchString, StringComparison.OrdinalIgnoreCase) >= 0, out UnicodeGlyphData[] filteredItems))
                        {
                            _selectionGrid.SetData(filteredItems);
                            _selectionGrid.SetMessageNoData(null);
                        }
                        else
                        {
                            _selectionGrid.SetData(Array.Empty<UnicodeGlyphData>());
                            _selectionGrid.SetMessageNoData($"No items matching '{newSearchString}'");
                        }
                    }
                    else if (_selectionGrid != null)
                    {
                        _selectionGrid.SetData(_config.GlyphMap.Values.ToArray());
                    }
                    _searchString = newSearchString;
                }
            }
           

            GUILayout.FlexibleSpace();
            if (_config != null)
            {
                if (GUILayout.Button("Copy All", EditorStyles.toolbarButton))
                {
                    StringBuilder sbAll = new StringBuilder();
                    foreach (var item in _config.GlyphMap.Keys)
                    {
                        sbAll.Append(item.ToString());
                    }
                    EditorGUIUtility.systemCopyBuffer = sbAll.ToString();
                }
            }
            
            if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown))
            {
                var sortableMenu = GetSettingsMenu();
                sortableMenu.CreateMenu().DropDown(new Rect(Screen.width - 216 - 40, 0, 0, 16));
                EditorGUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }

        private void HandleCreateFromWizard(UnicodeGlyphMap obj)
        {
            var config = UnicodeGlyphSetConfig.CreateInstance(obj);
            SetConfig(config);
        }

        public SortableMenu GetSettingsMenu()
        {
            SortableMenu menu = new SortableMenu();
            menu.AddSubMenu(_selectionGrid.GetItemsPerPageMenu());
            menu.AddSubMenu(_selectionGrid.GetItemSizeMenu());
            menu.AddSubMenu(GetCopyModeMenu());
            return menu;
        }
        
         public SortableMenu GetCopyModeMenu()
        {
            SortableMenu copyModeMenu = new SortableMenu("Copy Mode");
            copyModeMenu.AddItem(new TypeItem<CopyBufferMode>(CopyBufferMode.Code, value => _copyMode = value, value => value == _copyMode));
            copyModeMenu.AddItem(new TypeItem<CopyBufferMode>(CopyBufferMode.Symbol, value => _copyMode = value, value => value == _copyMode));
            return copyModeMenu;
        }

         private void HandleItemSelected(UnicodeGlyphData data)
         {
             if (_unicodeGlyphPreview != null)
             {
                 _unicodeGlyphPreview.Set(data, _config);
                 _unicodeGlyphPreview.SetFont(_config.GlyphMap.Font);
             }

             Selected = data.Character;
            
            if (OnItemSelected != null)
            {
                OnItemSelected?.Invoke(data, _config);
            }
            if (DelegateProperty != null)
            {
                if (DelegateProperty.propertyType == SerializedPropertyType.Character)
                {
                    DelegateProperty.intValue = Convert.ToInt32(data.Character);
                    DelegateProperty.serializedObject.ApplyModifiedProperties();
                }
                else if (DelegateProperty.propertyType == SerializedPropertyType.String)
                {
                    DelegateProperty.stringValue = data.Character.ToString();
                    DelegateProperty.serializedObject.ApplyModifiedProperties();
                }
                ///TODO dsaunders - this should be done via dispatch to the delegate view.
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
        
        void HandleCopyPasteEvents()
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.ValidateCommand:
                    switch (evt.commandName)
                    {
                        case Unicode.EventCommandNames.Copy:
                        case Unicode.EventCommandNames.Paste:
                            evt.Use();
                            break;
                    }
                    break;
                case EventType.ExecuteCommand:
                    switch (evt.commandName)
                    {
                        case Unicode.EventCommandNames.Copy:
                            HandleCopy();
                            evt.Use();
                            break;

                        case Unicode.EventCommandNames.Paste:
                            string copyBuffer = EditorGUIUtility.systemCopyBuffer;
                            if (copyBuffer.Length == 1)
                            {
                                char character = copyBuffer[0];
                                if (_config.GlyphMap.TryGet(character, out var data))
                                {
                                    HandleItemSelected(data);
                                }
                                GUI.changed = true;
                                evt.Use();
                            }
                            break;
                    }
                    break;
            }
        }
        
        
        public void HandleCopy()
        {
            switch (_copyMode)
            {
                case CopyBufferMode.Symbol:
                    EditorGUIUtility.systemCopyBuffer = Selected.ToString();
                    break;
                case CopyBufferMode.Code:
                    int charIntValue = Convert.ToInt32(Selected);
                    string storageValue = $"\'\\u{charIntValue:X4}\'";
                    EditorGUIUtility.systemCopyBuffer = storageValue;
                    break;
            }
            ShowNotification(new GUIContent($"Copied {EditorGUIUtility.systemCopyBuffer} to Clipboard!"));
            
        }

        void OnGUI()
        {
            if (_config != null && (_selectionGrid == null))
            {
                SetConfig(_config);
            }

            DrawToolStrip();
            if (_treeView != null)
            {
                Rect treeRect = EditorGUILayout.GetControlRect(true, _treeView.totalHeight);
                _treeView.searchString = _searchString;
                _treeView.OnGUI(treeRect);
                
            }
            else
            {
                if (_selectionGrid != null)
                {
                    _selectionGrid.Draw((int)position.width);
                }

                if (_unicodeGlyphPreview != null)
                {
                    _unicodeGlyphPreview.Draw();
                }
                
                HandleObjectPickerEvents();
                HandleCopyPasteEvents();
                /*
                if (_unicodeGlyphEditor != null && _unicodeGlyphEditor.target != null)
                {
                    _unicodeGlyphEditor.Draw();
                }*/
            }
            
            
        }

        private void HandleObjectPickerEvents()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                var selectedObject = EditorGUIUtility.GetObjectPickerObject();
                if (selectedObject is UnicodeGlyphSetConfig glyphSetConfig)
                {
                    SetConfig(glyphSetConfig);
                }
                else if (selectedObject is Font)
                {
                    
                }
            }
        }

        public static SelectionMode GetSelectionMode(SerializedProperty serializedProperty)
        {
            if (serializedProperty.isArray)
            {
                return SelectionMode.Multiple;
            }
            else
            {
                switch (serializedProperty.propertyType)
                {
                    case SerializedPropertyType.Character:
                        return SelectionMode.Single;
                    case SerializedPropertyType.String:
                        return SelectionMode.Extended;
                }
            }
            return SelectionMode.Extended;
        }
    }
}