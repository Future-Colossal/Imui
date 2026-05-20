using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Imui.IO.Utility
{
    public static class ImUnityInputWrapper
    {
#if ENABLE_INPUT_SYSTEM
        public static Vector2 MousePosition => Mouse.current?.position?.value ?? Touchscreen.current?.position?.value ?? default;
        public static bool TouchScreenSupported => Touchscreen.current?.enabled ?? false;
        public static bool IsControlPressed => Keyboard.current?.ctrlKey.isPressed ?? false;
        
        public static bool IsTouchBeganThisFrame()
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                return false;
            }

            var touches = touchscreen.touches;
            for (int i = 0; i < touches.Count; ++i)
            {
                if (touches[i].phase.value == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    return true;
                }
            }

            return false;
        }
#else
        public static Vector2 MousePosition => Input.mousePosition;
        public static bool TouchScreenSupported => Input.touchSupported;
        public static bool IsControlPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        
        public static bool IsTouchBeganThisFrame()
        {
            var count = Input.touchCount;

            for (int i = 0; i < count; ++i)
            {
                if (Input.GetTouch(i).phase == UnityEngine.TouchPhase.Began)
                {
                    return true;
                }
            }

            return false;
        }
#endif
    }
}