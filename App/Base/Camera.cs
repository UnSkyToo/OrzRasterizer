using System.Numerics;
using System.Windows.Forms;
using App.Helper;
using App.Interface;
using XRasterizer.Base;

namespace App.Base
{
    public class Camera : IUpdateable
    {
        private float Fov_ = 1f;
        private float Near_ = 1f;
        private float Far_ = 1000f;
        
        private Vector3 Position_ = new Vector3(0, 0, 5);
        private Vector3 LookDirection_ = new Vector3(0, 0, -1f);
        private float MoveSpeed_ = 10.0f;

        private float Yaw_;
        private float Pitch_;

        private Vector2 PreviousMousePos_;
        private float WindowWidth_;
        private float WindowHeight_;
        
        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }

        public Vector3 Position { get => Position_; set { Position_ = value; UpdateViewMatrix(); } }
        public Vector3 LookDirection => LookDirection_;

        public float FarDistance => Far_;

        public float FieldOfView => Fov_;
        public float NearDistance => Near_;

        public float AspectRatio => WindowWidth_ / WindowHeight_;

        public float Yaw { get => Yaw_; set { Yaw_ = value; UpdateViewMatrix(); } }
        public float Pitch { get => Pitch_; set { Pitch_ = value; UpdateViewMatrix(); } }

        public Camera(float Width, float Height)
        {
            WindowWidth_ = Width;
            WindowHeight_ = Height;
            UpdatePerspectiveMatrix();
            UpdateViewMatrix();
        }
        
        public void Update(float DeltaSeconds)
        {
            var SprintFactor = Input.IsKeyDown(Keys.ControlKey) ? 0.1f : Input.IsKeyDown(Keys.ShiftKey) ? 2.5f : 1f;

            var MotionDir = Vector3.Zero;
            if (Input.IsKeyDown(Keys.A))
            {
                MotionDir += -Vector3.UnitX;
            }
            if (Input.IsKeyDown(Keys.D))
            {
                MotionDir += Vector3.UnitX;
            }
            if (Input.IsKeyDown(Keys.W))
            {
                MotionDir += -Vector3.UnitZ;
            }
            if (Input.IsKeyDown(Keys.S))
            {
                MotionDir += Vector3.UnitZ;
            }
            if (Input.IsKeyDown(Keys.Q))
            {
                MotionDir += -Vector3.UnitY;
            }
            if (Input.IsKeyDown(Keys.E))
            {
                MotionDir += Vector3.UnitY;
            }

            if (MotionDir != Vector3.Zero)
            {
                var LookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
                MotionDir = Vector3.Transform(MotionDir, LookRotation);
                Position_ += MotionDir * MoveSpeed_ * SprintFactor * DeltaSeconds;
                UpdateViewMatrix();
            }
            
            var MouseDelta = Input.GetMouseMovePos() - PreviousMousePos_;
            PreviousMousePos_ = Input.GetMouseMovePos();

            if ((Input.IsMouseDown(MouseButtons.Left) || Input.IsMouseDown(MouseButtons.Right)))
            {
                Yaw += -MouseDelta.X * 0.0015f * SprintFactor;
                Pitch += -MouseDelta.Y * 0.0015f * SprintFactor;
                Pitch = Mathf.Clamp(Pitch, -1.55f, 1.55f);

                UpdateViewMatrix();
            }
        }

        public void WindowResized(float Width, float Height)
        {
            WindowWidth_ = Width;
            WindowHeight_ = Height;
            UpdatePerspectiveMatrix();
        }

        private void UpdatePerspectiveMatrix()
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(Fov_, WindowWidth_ / WindowHeight_, Near_, Far_);
        }

        private void UpdateViewMatrix()
        {
            var LookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            var LookDir = Vector3.Transform(-Vector3.UnitZ, LookRotation);
            LookDirection_ = LookDir;
            ViewMatrix = Matrix4x4.CreateLookAt(Position_, Position + LookDirection_, Vector3.UnitY);
        }
    }
}