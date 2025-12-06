using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MenuGraphTool.Editor
{
    [CustomPropertyDrawer(typeof(MenuPreview))]
    public class MenuPreviewPropertyDrawer : PropertyDrawer
    {
        #region Fields
        private SerializedProperty _property;
        private MenuPage _menuPage;

        // VisualElements
        private VisualElement _root = new();
        private VisualElement _snapshotPreview;
        private VisualElement _errorScreen;
        #endregion Fields

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _root = new();
            _property = property;
            
            LoadUXML(_root);
            BindVisualElements();
            UpdateSnapshotPreview();

            TryUpdateSnapShotPreview();

            return _root;
        }

        private void BindVisualElements()
        {
            if (_property.boxedValue is not MenuPreview menuPreview)
            {
                return;
            }
            _menuPage = menuPreview.MenuPage;
            _snapshotPreview = _root.Query<VisualElement>("SnapshotPreview");
            _errorScreen = _root.Query<VisualElement>("ErrorScreen");
        }

        // This was necessary as there is no callback when the property changes.
        public async void TryUpdateSnapShotPreview()
        {
            while (true)
            {
                await Task.Delay(1000);

                if (_property.serializedObject.targetObject == null)
                {
                    return;
                }

                _property.serializedObject.Update();

                if (_property.boxedValue is not MenuPreview menuPreview)
                {
                    continue;
                }
                if (_menuPage == menuPreview.MenuPage)
                {
                    continue;
                }

                _menuPage = menuPreview.MenuPage;
                UpdateSnapshotPreview();
            }
        }

        private void UpdateSnapshotPreview()
        {
            if (_menuPage == null)
            {
                return;
            }

            Canvas canvas = _menuPage.GetComponentInChildren<Canvas>();

            if (canvas == null)
            {
                _snapshotPreview.style.backgroundImage = new();
                _errorScreen.style.display = DisplayStyle.Flex;
                return;
            }
            _errorScreen.style.display = DisplayStyle.None;

            CanvasSnapshotMaker canvasSnapshotMaker = new CanvasSnapshotMaker(canvas);
            Texture2D texture = canvasSnapshotMaker.TakeSnapshot();

            _snapshotPreview.style.backgroundImage = texture;
        }

        private void LoadUXML(in VisualElement target, [CallerFilePath] string absoluteScriptFilePath = "")
        {
            string currentFilePath = $"Assets" + absoluteScriptFilePath.Substring(Application.dataPath.Length);
            string uxmlPath = Path.ChangeExtension(currentFilePath, "uxml");
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

            if (uxml == null)
            {
                throw new FileNotFoundException($"Couldn't find a UXML asset at this location \"{uxmlPath}\"");
            }

            uxml.CloneTree(target);
        }
    }
}
