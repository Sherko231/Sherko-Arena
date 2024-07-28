using UnityEngine;

namespace Sherko.Utils
{
    public static class SherkoUtils
    {
        public static void ToggleCursor(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
    }
}

