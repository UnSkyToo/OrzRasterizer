using System.Numerics;
using System.Windows.Forms;

namespace App
{
    public static class Input
    {
        private static readonly bool[] KeyCurState_ = new bool[256];
        private static readonly bool[] KeyPreState_ = new bool[256];
        private static readonly bool[] MouseCurState_ = new bool[4];
        private static readonly bool[] MousePreState_ = new bool[4];
        private static readonly Vector2[] MouseClickPos_ = new Vector2[4];
        private static Vector2 MouseCurDeltaPos_ = Vector2.Zero;
        private static Vector2 MousePreDeltaPos_ = Vector2.Zero;

        static Input()
        {
            for (var Index = 0; Index < KeyCurState_.Length; ++Index)
            {
                KeyCurState_[Index] = false;
                KeyPreState_[Index] = false;
            }

            for (var Index = 0; Index < MouseCurState_.Length; ++Index)
            {
                MouseCurState_[Index] = false;
                MousePreState_[Index] = false;
                MouseClickPos_[Index] = Vector2.Zero;
            }
        }

        public static void Update()
        {
            for (var Index = 0; Index < KeyCurState_.Length; ++Index)
            {
                KeyPreState_[Index] = KeyCurState_[Index];
            }

            for (var Index = 0; Index < MouseCurState_.Length; ++Index)
            {
                MousePreState_[Index] = MouseCurState_[Index];
            }

            MousePreDeltaPos_ = MouseCurDeltaPos_;
        }

        private static int KeyToIndex(Keys Key)
        {
            return (int)Key;
        }

        public static void OnKeyDown(Keys Key)
        {
            var Index = KeyToIndex(Key);
            KeyPreState_[Index] = KeyCurState_[Index];
            KeyCurState_[Index] = true;
        }

        public static void OnKeyUp(Keys Key)
        {
            var Index = KeyToIndex(Key);
            KeyPreState_[Index] = KeyCurState_[Index];
            KeyCurState_[Index] = false;
        }

        public static bool IsKeyDown(Keys Key)
        {
            var Index = KeyToIndex(Key);
            return KeyCurState_[Index];
        }

        public static bool IsKeyUp(Keys Key)
        {
            var Index = KeyToIndex(Key);
            return !KeyCurState_[Index];
        }

        public static bool IsKeyPressed(Keys Key)
        {
            var Index = KeyToIndex(Key);
            return KeyPreState_[Index] == false && KeyCurState_[Index] == true;
        }

        private static int MouseToIndex(MouseButtons Mouse)
        {
            switch (Mouse)
            {
                case MouseButtons.Left:
                    return 0;
                case MouseButtons.Right:
                    return 1;
                case MouseButtons.Middle:
                    return 2;
                default:
                    return 3;
            }
        }

        public static void OnMouseDown(MouseButtons Btn, int X, int Y)
        {
            var Index = MouseToIndex(Btn);
            MousePreState_[Index] = MouseCurState_[Index];
            MouseCurState_[Index] = true;
            MouseClickPos_[Index] = new Vector2(X, Y);

            MousePreDeltaPos_ = MouseCurDeltaPos_;
            MouseCurDeltaPos_ = Vector2.Zero;
        }

        public static void OnMouseUp(MouseButtons Btn, int X, int Y)
        {
            var Index = MouseToIndex(Btn);
            MousePreState_[Index] = MouseCurState_[Index];
            MouseCurState_[Index] = false;
            MouseClickPos_[Index] = Vector2.Zero;

            MousePreDeltaPos_ = MouseCurDeltaPos_;
            MouseCurDeltaPos_ = Vector2.Zero;
        }

        public static void OnMouseMove(MouseButtons Btn, int X, int Y)
        {
            var Index = MouseToIndex(Btn);
            if (MouseCurState_[Index])
            {
                MousePreDeltaPos_ = MouseCurDeltaPos_;
                MouseCurDeltaPos_ = MouseClickPos_[Index] - new Vector2(X, Y);
            }
        }

        public static bool IsMouseDown(MouseButtons Btn)
        {
            var Index = MouseToIndex(Btn);
            return MouseCurState_[Index];
        }

        public static bool IsMouseUp(MouseButtons Btn)
        {
            var Index = MouseToIndex(Btn);
            return !MouseCurState_[Index];
        }

        public static bool IsMousePressed(MouseButtons Btn)
        {
            var Index = MouseToIndex(Btn);
            return MousePreState_[Index] == false && MouseCurState_[Index] == true;
        }

        public static bool IsMouseMove()
        {
            return (MouseCurDeltaPos_ - MousePreDeltaPos_).Length() > 0;
        }

        public static Vector2 GetMouseClickPos(MouseButtons Btn)
        {
            var Index = MouseToIndex(Btn);
            return MouseClickPos_[Index];
        }

        public static Vector2 GetMouseMovePos()
        {
            return MouseCurDeltaPos_;
        }

        public static Vector2 GetMouseDelta()
        {
            return MouseCurDeltaPos_ - MousePreDeltaPos_;
        }
    }
}