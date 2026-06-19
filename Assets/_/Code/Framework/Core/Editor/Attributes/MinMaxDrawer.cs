using Core.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        #region Unity API

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new IMGUIContainer(() => DrawIMGUI(property));
            return container;
        }

        #endregion


        #region Main API

        private void DrawIMGUI(SerializedProperty property)
        {
            var attr = (MinMaxAttribute)attribute;

            float minValue = property.vector2Value.x;
            float maxValue = property.vector2Value.y;
            float multiplier = Mathf.Pow(10f, attr.Decimals);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(property.displayName, GUILayout.MinWidth(120), GUILayout.ExpandWidth(false));

            minValue = EditorGUILayout.FloatField(minValue, GUILayout.Width(50));
            EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, attr.Min, attr.Max);
            maxValue = EditorGUILayout.FloatField(maxValue, GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();

            minValue = Mathf.Round(Mathf.Clamp(minValue, attr.Min, maxValue) * multiplier) / multiplier;
            maxValue = Mathf.Round(Mathf.Clamp(maxValue, minValue, attr.Max) * multiplier) / multiplier;

            property.vector2Value = new Vector2(minValue, maxValue);
            property.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}