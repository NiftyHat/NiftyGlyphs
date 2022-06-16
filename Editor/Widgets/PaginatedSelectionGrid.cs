using System;
using System.Collections.Generic;
using System.Linq;
using NiftyEditorMenu;
using UnityEditor;
using UnityEngine;

namespace NiftyGlyphs
{
    public class PaginatedSelectionGrid<TItemData>
    {
        public delegate void RequestUpdateStyle(Style style);
        public class Style
        {
            public readonly GUIContent ForwardIcon;
            public readonly GUIContent BackIcon;

            public const int PaginationButtonWidth = 30;
            public const int PaginationBarHeight = 24;
            public GUIStyle GridItemStyle;
            public GUIStyle GridItemStyleSelected;
            public GUIStyle LabelNoDataStyle;
            public readonly Color GridItemSelectedColor = Color.cyan;
            public bool IsStyleChangeQueued = true;
            private readonly List<RequestUpdateStyle> _changeQueue;

            public Style()
            {
                ForwardIcon = EditorGUIUtility.IconContent("forward@2x");
                BackIcon = EditorGUIUtility.IconContent("back@2x");
                
                GridItemStyle = null;
                GridItemStyleSelected = null;
                LabelNoDataStyle = null;
                _changeQueue = new List<RequestUpdateStyle>
                {
                    style =>
                    {
                        if (GridItemStyle == null)
                        {
                            GridItemStyle = new GUIStyle(GUI.skin.button);
                        }

                        if (GridItemStyleSelected == null)
                        {
                            GridItemStyleSelected = new GUIStyle(GUI.skin.button);
                        }
                        
                        if (LabelNoDataStyle == null)
                        {
                            LabelNoDataStyle = new GUIStyle(GUI.skin.label);
                            LabelNoDataStyle.fontSize = 32;
                            LabelNoDataStyle.fontStyle = FontStyle.Bold;
                            LabelNoDataStyle.alignment = TextAnchor.MiddleCenter;
                        }
                    }
                };
            }

            internal void ProcessChangeQueue()
            {
                if (IsStyleChangeQueued)
                {
                    if (_changeQueue.Count > 0)
                    {
                        for (int i = 0; i < _changeQueue.Count; i++)
                        {
                            _changeQueue[i]?.Invoke(this);
                        }
                        _changeQueue.Clear();
                    }
                }
            }

            public void ChangeStyle(RequestUpdateStyle requestStyleRequestUpdate)
            {
                _changeQueue.Add(requestStyleRequestUpdate);
                IsStyleChangeQueued = true;
            }
        }

        [Serializable]
        private class StateData
        {
            public int ItemsPerPage = 50;
            public int LastPageViewed = 0;
        }
        
        private readonly struct PageData<TData>
        {
            private readonly TData[] _items;
            public GUIContent[] GUIContentList { get; }
            public int Length { get; }

            public PageData(TData[] items, Func<TData, GUIContent> func)
            {
                _items = items;
                GUIContentList = _items.Select(func).ToArray();
                Length = GUIContentList.Length;
            }

            public TData GetItem(int index)
            {
                return _items[index];
            }
        }
        
        private readonly StateData _stateData;

        private int _gridItemSize;
        private readonly Func<TItemData, GUIContent> _guiContentProvider;
        private int _gridItemsPerPage;
        private int _page = 1;
        private int _pageTotalCount;
        
        private readonly Style _style = new Style();
        private int _lastSelectedIndex;
        private TItemData[] _data;
        private PageData<TItemData> _pageData;
        private Vector2 _scrollPosition = new Vector2(0, 0);
        private readonly Action<TItemData> _onItemSelected;
        private readonly HashSet<int> _selectedItems = new HashSet<int>();
        private UnicodeGlyphBrowserWindow.SelectionMode _selectionMode = UnicodeGlyphBrowserWindow.SelectionMode.Single;
        private string _messageNoData;
        public event Action<int> OnItemResize;
        
        public PaginatedSelectionGrid(TItemData[] data, Action<TItemData> onItemSelected = null, int gridItemsPerPage = 0, int gridItemSize = 32, Func<TItemData, GUIContent> guiContentProvider = null)
        {
            _stateData = new StateData();
            _data = data;
            _gridItemSize = gridItemSize;
            _guiContentProvider = guiContentProvider;
            if (gridItemsPerPage > 0)
            {
                SetItemsPerPage(gridItemsPerPage);
                SetGridFromPage(_stateData.LastPageViewed);
            }
            _onItemSelected = onItemSelected;
        }

        public int GetBestItemsPerPageForRect(Rect area)
        {
            int itemsPerRow = (int)area.width / _gridItemSize;
            int itemsPerCol = (int)area.height / _gridItemSize;
            int maxItemsPerPage = itemsPerCol * itemsPerRow;
            int[] itemsPerPageOptions = new[] { 10, 25, 50, 100, 250, 500 };
            for (int i = itemsPerPageOptions.Length -1; i >=0 ; i--)
            {
                if (itemsPerPageOptions[i] < maxItemsPerPage)
                {
                    return itemsPerPageOptions[i];
                }
            }
            return itemsPerPageOptions[0];
        }
        
        public SortableMenu GetItemsPerPageMenu()
        {
            SortableMenu itemsPerPageMenu = new SortableMenu("Items Per Page");
            itemsPerPageMenu.AddItem(new TypeItem<int>(10, HandleSelectPerPage, HandlePerPageChecked));
            itemsPerPageMenu.AddItem(new TypeItem<int>(25, HandleSelectPerPage, HandlePerPageChecked));
            itemsPerPageMenu.AddItem(new TypeItem<int>(50, HandleSelectPerPage, HandlePerPageChecked));
            itemsPerPageMenu.AddItem(new TypeItem<int>(100, HandleSelectPerPage, HandlePerPageChecked));
            itemsPerPageMenu.AddItem(new TypeItem<int>(250, HandleSelectPerPage, HandlePerPageChecked));
            itemsPerPageMenu.AddItem(new TypeItem<int>(500, HandleSelectPerPage, HandlePerPageChecked));
            return itemsPerPageMenu;
        }
        
        public bool HandlePerPageChecked(int value)
        {
            return value == _gridItemsPerPage;
        }
        
        public SortableMenu GetItemSizeMenu()
        {
            SortableMenu itemsPerPageMenu = new SortableMenu("Item Size");
            itemsPerPageMenu.AddItem(new TypeItem<int>(24, HandleSelectSize, HandleSelectSizeCheck));
            itemsPerPageMenu.AddItem(new TypeItem<int>(32, HandleSelectSize, HandleSelectSizeCheck));
            itemsPerPageMenu.AddItem(new TypeItem<int>(42, HandleSelectSize, HandleSelectSizeCheck));
            itemsPerPageMenu.AddItem(new TypeItem<int>(64, HandleSelectSize, HandleSelectSizeCheck));
            itemsPerPageMenu.AddItem(new TypeItem<int>(128, HandleSelectSize, HandleSelectSizeCheck));
            itemsPerPageMenu.AddItem(new TypeItem<int>(256, HandleSelectSize, HandleSelectSizeCheck));
            return itemsPerPageMenu;
        }

        private bool HandleSelectSizeCheck(int size)
        {
            return _gridItemSize == size;
        }

        private void HandleSelectSize(int size)
        {
            _gridItemSize = size;
            OnItemResize?.Invoke(size);
        }

        private void HandleSelectPerPage(int amount)
        {
            SetItemsPerPage(amount);
        }

        public void SetSelectionMode(UnicodeGlyphBrowserWindow.SelectionMode selectionMode)
        {
            _selectionMode = selectionMode;
        }

        public void SetItemsPerPage(int count)
        {
            int oldItemCount = _gridItemsPerPage;
            _gridItemsPerPage = count;
            _stateData.ItemsPerPage = count;
            _pageTotalCount = Mathf.CeilToInt((float)(_data.Length - 1) / _gridItemsPerPage);
            int newPage = Math.Max(Mathf.CeilToInt((float)_page * oldItemCount / _gridItemsPerPage), 1);
            if (newPage > _pageTotalCount)
            {
                newPage = _pageTotalCount;
            }
            SetPage(newPage);
        }
        
        public void Draw(int width)
        {
            _style?.ProcessChangeQueue();
            DrawGrid(width);
            DrawPaginationControls();
        }

        public void SetPage(int value)
        {
            _stateData.LastPageViewed = value;
            _page = value;
            _lastSelectedIndex = -1;
            SetGridFromPage(_page);
        }

        private void SetGridFromPage(int page)
        {
            if (_data == null)
            {
                return;
            }
            int start = (page - 1) * _gridItemsPerPage;
            int count = Mathf.Min(_gridItemsPerPage, _data.Length - start);
            start = Mathf.Max(start, 0);
            if (count < 0)
            {
                count = 0;
            }
            if (count > 0)
            {
                TItemData[] pageData = new TItemData[count];
                Array.Copy(_data.ToArray(), start, pageData, 0 , count);
                if (_guiContentProvider != null)
                {
                    _pageData = new PageData<TItemData>(pageData, _guiContentProvider);
                }
                else
                {
                    _pageData = new PageData<TItemData>(pageData, data => new GUIContent(data.ToString()));
                }
            }
        }
            
        private void DrawGrid(int width)
        {
            int itemWidthWithMargin = _gridItemSize + 3;
            int itemPerRow = (int)(width) / itemWidthWithMargin;
            if (_pageData.Length > 0)
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxWidth(width));
                int columns = itemPerRow;
                int rows = Mathf.CeilToInt((float)_pageData.Length / itemPerRow);
                GUILayout.BeginVertical();
                Color defaultColor = GUI.color;
                for (int y = 0; y <= rows; y++)
                {
                    GUILayout.BeginHorizontal();
                    for (int x = 0; x < columns; x++)
                    {
                        int index = (y * columns) + x;
                        if (index < _pageData.GUIContentList.Length)
                        {
                            if (_selectedItems.Contains(index))
                            {
                                GUI.color = _style.GridItemSelectedColor;
                            }
                            else
                            {
                                GUI.color = defaultColor;
                            }

                            GUIContent buttonContent = _pageData.GUIContentList[index];
                            var buttonData = _pageData.GetItem(index);
                            bool isSelected = _selectedItems.Contains(index);
                            if (GUILayout.Button(buttonContent, isSelected ? _style.GridItemStyleSelected : _style.GridItemStyle,
                                GUILayout.Width(_gridItemSize), GUILayout.Height(_gridItemSize)))
                            {
                                if (_selectionMode == UnicodeGlyphBrowserWindow.SelectionMode.Extended)
                                {
                                    if (Event.current.shift)
                                    {
                                        for (int i = _lastSelectedIndex; i < index; i++)
                                        {
                                            _selectedItems.Add(i);
                                        }
                                    }
                                }

                                switch (_selectionMode)
                                {
                                    case UnicodeGlyphBrowserWindow.SelectionMode.Extended:
                                    case UnicodeGlyphBrowserWindow.SelectionMode.Multiple:
                                    {
                                        if (_selectedItems.Contains(index))
                                        {
                                            _selectedItems.Remove(index);
                                        }
                                        else
                                        {
                                            _selectedItems.Add(index);
                                            if (_lastSelectedIndex != index)
                                            {
                                                _lastSelectedIndex = index;
                                                _onItemSelected?.Invoke(buttonData);
                                            }
                                        }

                                        break;
                                    }
                                    case UnicodeGlyphBrowserWindow.SelectionMode.Single:
                                    {
                                        _selectedItems.Clear();
                                        if (_lastSelectedIndex != index)
                                        {
                                            _lastSelectedIndex = index;
                                            _onItemSelected?.Invoke(buttonData);
                                        }

                                        break;
                                    }
                                }
                            }
                            GUI.color = defaultColor;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.BeginVertical();
                if (_messageNoData != null)
                {
                    GUILayout.Label(_messageNoData, _style.LabelNoDataStyle, GUILayout.MaxWidth(width), GUILayout.ExpandHeight(true));
                }
                else
                {
                    GUILayout.Label("No Page Data", _style.LabelNoDataStyle, GUILayout.MaxWidth(width), GUILayout.ExpandHeight(true));
                }
                
                GUILayout.EndVertical();
            }
        }
        
        public void DrawPaginationControls()
        {
            Color defaultColor = GUI.color;
            int newPage = _page;
            GUILayout.BeginHorizontal();
                
            if (_pageTotalCount > 1)
            {
                int pageItemsToDisplay = 15;
                int pageDisplayMidpoint = pageItemsToDisplay / 2;
                int pageCurrent = _page;
                int pageMax = _pageTotalCount;
                int pagePositionStart = Math.Max(1, Mathf.Min(_page - pageDisplayMidpoint, pageMax - pageItemsToDisplay));
                int pagePositionEnd = Math.Min(pageMax, Mathf.Max(_page + pageDisplayMidpoint, pageItemsToDisplay));
                bool hasMoreItemsThanDisplay = pageMax > pageItemsToDisplay;
                
                EditorGUI.BeginDisabledGroup(_page <= 1);
                if (GUILayout.Button(_style.BackIcon, GUILayout.Width(Style.PaginationButtonWidth)))
                {
                    newPage--;
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();

                if (hasMoreItemsThanDisplay && pagePositionStart > 2)
                {
                    if( GUILayout.Button("1", GUILayout.Height(Style.PaginationBarHeight)))
                    {
                        newPage = 1;
                    }
                    GUILayout.Label("...", GUILayout.Width(20));
                }
                for (int i = pagePositionStart; i <= pagePositionEnd; i++)
                {
                    if (i == pageCurrent)
                    {
                        GUI.color = Color.green;
                    }
                    if( GUILayout.Button(i.ToString(), GUILayout.Height(Style.PaginationBarHeight)))
                    {
                        newPage = i;
                    }

                    GUI.color = defaultColor;
                }
                if (hasMoreItemsThanDisplay && pagePositionEnd < pageMax)
                {
                    GUILayout.Label("...", GUILayout.Width(20));
                    if( GUILayout.Button(pageMax.ToString(), GUILayout.Height(Style.PaginationBarHeight)))
                    {
                        newPage = pageMax;
                    }
                }
                   
                GUILayout.EndHorizontal();
                    
                EditorGUI.BeginDisabledGroup(_page >= pageMax);
                if (GUILayout.Button(_style.ForwardIcon, GUILayout.Width(Style.PaginationButtonWidth)))
                {
                    newPage++;
                }
                EditorGUI.EndDisabledGroup();
            }
            int itemsDisplayed = (_page - 1) * _gridItemsPerPage;
            GUILayout.Label($"{itemsDisplayed} / {_data.Length}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            if (newPage != _page)
            {
                SetPage(newPage); 
            }
        }

        public void SetItemStyle(GUIStyle style)
        {
            _style.GridItemStyle = style;
        }

        public void UpdateItemStyle(RequestUpdateStyle requestStyleRequestUpdate)
        {
            _style.ChangeStyle(requestStyleRequestUpdate);
        }

        public void SetMessageNoData(string message)
        {
            _messageNoData = message;
        }

        public void SetData(TItemData[] list)
        {
            int currentItemCount = _data.Length;
            int currentFirstItem = _page * _gridItemsPerPage;
            int newItemCount = list.Length;
            int newCurrentPage = _stateData.LastPageViewed;
            _pageTotalCount = Mathf.CeilToInt((float)(newItemCount - 1) / _gridItemsPerPage);
            //if the new length is shorter, try position on the same relative page. 
            if (newItemCount < currentItemCount)
            {
                int newPageCount = list.Length / _gridItemsPerPage;
                float currentPositionPercentage = currentFirstItem / (float)currentItemCount;
                int newItemOnCurrentPage = Mathf.FloorToInt(currentPositionPercentage * newItemCount);
                newCurrentPage = Math.Max(newItemOnCurrentPage * newPageCount, 1);
            }
            _data = list;
            SetPage(newCurrentPage);
        }
    }
}