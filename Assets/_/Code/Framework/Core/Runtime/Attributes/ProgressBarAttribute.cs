using UnityEngine;

namespace Core.Runtime
{
    public class ProgressBarAttribute : PropertyAttribute
    {
        public string Label { get; }
        public float MaxValue { get; }
        public string MaxValueField { get; }
        public float R { get; }
        public float G { get; }
        public float B { get; }

        public ProgressBarAttribute(string label, float maxValue, float r = 0.35f, float g = 0.65f, float b = 0.35f)
        {
            Label = label;
            MaxValue = maxValue;
            MaxValueField = null;
            R = r;
            G = g;
            B = b;
        }

        public ProgressBarAttribute(string label, string maxValueField, float r = 0.35f, float g = 0.65f, float b = 0.35f)
        {
            Label = label;
            MaxValue = -1f;
            MaxValueField = maxValueField;
            R = r;
            G = g;
            B = b;
        }
    }
}