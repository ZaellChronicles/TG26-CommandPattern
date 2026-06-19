using Core.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        #region Unity API

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (ProgressBarAttribute)attribute;

            var container = new VisualElement();

            var label = new Label(attr.Label);

            var bar = new ProgressBar
            {
                lowValue = 0f,
                highValue = ResolveMax(property, attr),
            };

            bar.style.height = 20f;
            bar.style.unityFont = new StyleFont(Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

            var fillColor = new Color(attr.R, attr.G, attr.B);

            container.Add(label);
            container.Add(bar);

            container.TrackPropertyValue(property, p =>
            {
                UpdateBar(bar, p, property.serializedObject, attr, fillColor);
            });

            UpdateBar(bar, property, property.serializedObject, attr, fillColor);

            return container;
        }

        #endregion


        #region Main API

        private static void UpdateBar(ProgressBar bar, SerializedProperty property, SerializedObject serializedObject, ProgressBarAttribute attr, Color fillColor)
        {
            float current = GetFloat(property);
            float max = ResolveMax(property, attr);

            bar.highValue = max;
            bar.value = current;
            bar.title = $"{current} / {max}";

            var fill = bar.Q(className: "unity-progress-bar__progress");
            if (fill != null)
                fill.style.backgroundColor = fillColor;
        }

        private static float GetFloat(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.Integer => property.intValue,
                _ => 0f
            };
        }

        private static float ResolveMax(SerializedProperty property, ProgressBarAttribute attr)
        {
            if (attr.MaxValueField == null)
                return attr.MaxValue;

            var sibling = property.serializedObject.FindProperty(attr.MaxValueField);

            if (sibling == null)
            {
                Debug.LogWarning($"[ProgressBar] Champ '{attr.MaxValueField}' introuvable.");
                return 1f;
            }

            return sibling.propertyType switch
            {
                SerializedPropertyType.Float => sibling.floatValue,
                SerializedPropertyType.Integer => sibling.intValue,
                _ => 1f
            };
        }

        #endregion
    }
}