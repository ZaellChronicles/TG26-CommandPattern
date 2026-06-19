using Core.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Core.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        #region Unity API

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new PropertyField(property);
            field.SetEnabled(false);
            return field;
        }

        #endregion
    }
}