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
    }
}

