using UnityEngine;

namespace Sherko.Utils
{
    public static class SherkoUtils
    {
        public static bool IsCursorVisible { get; private set; }
        
        public static void ToggleCursor(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
            IsCursorVisible = visible;
        }

        public static float SquaredDistance(Vector3 a, Vector3 b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return num1*num1 + num2*num2 + num3*num3;
        }
    }
}

