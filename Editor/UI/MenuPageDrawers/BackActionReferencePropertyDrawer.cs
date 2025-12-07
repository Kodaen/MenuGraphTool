using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MenuGraphTool.Editor
{
    [CustomPropertyDrawer(typeof(BackActionReference))]
    public class BackActionReferencePropertyDrawer : PropertyDrawer
    {
        private VisualElement _root = new();
        private PropertyField _inputActionReferenceField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _root.LoadUXML();

            BindVisualElements();
            return _root;
        }

        private void BindVisualElements()
        {
            _inputActionReferenceField = _root.Query<PropertyField>("InputActionReference");

            PropertyField enumField = _root.Query<PropertyField>("BackActionType");
            enumField.RegisterValueChangeCallback(evt =>
            {
                UpdateInputRefField((BackActionType)evt.changedProperty.enumValueIndex);
            });
        }

        private void UpdateInputRefField(BackActionType backActionType)
        {
            _inputActionReferenceField.style.display = backActionType switch
            {
                BackActionType.Override => (StyleEnum<DisplayStyle>)DisplayStyle.Flex,
                BackActionType.Default => (StyleEnum<DisplayStyle>)DisplayStyle.None,
                BackActionType.None => (StyleEnum<DisplayStyle>)DisplayStyle.None,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
