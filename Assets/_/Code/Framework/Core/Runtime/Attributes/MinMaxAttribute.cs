using UnityEngine;

namespace Core.Runtime
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public int Decimals { get; }

        public MinMaxAttribute(float min, float max, int decimals = 2)
        {
            Min = min;
            Max = max;
            Decimals = decimals;
        }
    }
}