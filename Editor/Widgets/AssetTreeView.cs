using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiftyGlyphs
{
    public class AssetTreeView<TAsset> : TreeView where TAsset : UnityEngine.Object
    {
        public class AssetTreeViewItem : TreeViewItem
        {
            internal string path;

            public TAsset Load()
            {
                return AssetDatabase.LoadAssetAtPath<TAsset>(path);
            }
        }

        public event Action<IEnumerable<Object>> OnSelectionChange;
        public event Action<TAsset> OnDoubleClickItem;

        public AssetTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();
        }

        protected override void DoubleClickedItem(int selectedId)
        {
            var rows = FindRows(new int[] { selectedId });
            if (rows.Count == 1)
            {
                var itemRow = rows[0];
                if (itemRow is AssetTreeViewItem treeViewItem)
                {
                    OnDoubleClickItem?.Invoke(treeViewItem.Load());
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            /*
            var rows = FindRows(selectedIds);
            var selectedAssets = rows.Select(item =>
            {
                if (item is AssetTreeViewItem treeViewItem)
                {
                    return treeViewItem.asset;
                }
                return null;
            });
            OnSelectionChange?.Invoke(selectedAssets);
            base.SelectionChanged(selectedIds);*/
        }

        private static string[] FindAssetGUIDs(string[] folders)
        {
            string filter = $"t:{typeof(TAsset).Name}";
            if (folders != null && folders.Length > 0)
            {
                return AssetDatabase.FindAssets(filter, folders);
            }

            return FindAssetGUIDs();
        }

        private static string[] FindAssetGUIDs(string folder)
        {
            if (folder != null)
            {
                return FindAssetGUIDs(new[] { folder });
            }
            return FindAssetGUIDs();
        }

        private static string[] FindAssetGUIDs()
        {
            string filter = $"t:{typeof(TAsset).Name}";
            return AssetDatabase.FindAssets(filter);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            string[] assetGUIDs = FindAssetGUIDs();
            int id = 0;
            foreach (var guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(assetPath);
                var typeIcon = AssetDatabase.GetCachedIcon(assetPath);
                id++;
                AssetTreeViewItem typeItem = new AssetTreeViewItem
                    { id = id, depth = 1, displayName = $"{assetPath}", path = assetPath };
                typeItem.icon = typeIcon as Texture2D;
                root.AddChild(typeItem);
            }
            return root;
        }
    }
}