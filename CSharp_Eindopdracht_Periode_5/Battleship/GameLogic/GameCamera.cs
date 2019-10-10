using Battleship.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Battleship.GameLogic
{
    public class GameCamera : GameObject
    {
        private PerspectiveCamera camera;

        private double yaw;
        private double pitch;
        private double speed;
        private double sensitivity;

        private bool constrainPitch;
        private bool firstMouse;
        private double lastX;
        private double lastY;

        private Vector3D front;
        private Vector3D up;
        private Vector3D right;

        public GameCamera(Game game)
            : base(game)
        {
            this.camera = new PerspectiveCamera();
            this.camera.Position = new Point3D(0, 10, 20);
            this.camera.FieldOfView = 60;

            this.yaw = -90.0f;
            this.pitch = -25.0f;
            this.speed = 5f;
            this.sensitivity = 0.1f;

            this.constrainPitch = true;
            this.firstMouse = true;

            this.front = new Vector3D(0, 0, -1);
            this.up = new Vector3D(0, 1, 0);
            this.right = new Vector3D(1, 0, 0);

            UpdateVectors();

            GameInput.KeyDown += OnKeyDown;
            GameInput.KeyUp += OnKeyUp;
            GameInput.MouseDown += OnMouseDown;
            GameInput.MouseMove += OnMouseMove;
        }

        private void OnKeyDown(Key key)
        {
            this.Velocity = new Vector3D(0, 0, 0);

            if (GameInput.IsKeyDown(Key.W))
                this.Velocity += this.front * this.speed;
            if (GameInput.IsKeyDown(Key.S))
                this.Velocity += -this.front * this.speed;
            if (GameInput.IsKeyDown(Key.A))
                this.Velocity += -this.right * this.speed;
            if (GameInput.IsKeyDown(Key.D))
                this.Velocity += this.right * this.speed;

            this.camera.Transform = this.transform;
        }

        private void OnKeyUp(Key key)
        {
            if (!GameInput.IsKeyDown(Key.W) && !GameInput.IsKeyDown(Key.S) && !GameInput.IsKeyDown(Key.A) && !GameInput.IsKeyDown(Key.D))
                this.Velocity = new Vector3D(0, 0, 0);
        }

        private void OnMouseDown(MouseButton button, Point position)
        {
            if (button == MouseButton.Middle)
                this.firstMouse = true;
        }

        private void OnMouseMove(Point position)
        {
            if (this.firstMouse)
            {
                this.lastX = position.X;
                this.lastY = position.Y;
                this.firstMouse = false;
            }
            double xoffset = this.lastX - position.X;
            double yoffset = this.lastY - position.Y;
            lastX = position.X;
            lastY = position.Y;

            xoffset *= this.sensitivity;
            yoffset *= this.sensitivity;
            this.yaw -= xoffset;
            this.pitch += yoffset;
            if (this.constrainPitch)
            {
                if (this.pitch > 89.0f)
                    this.pitch = 89.0f;
                if (this.pitch < -89.0f)
                    this.pitch = -89.0f;
            }

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            this.front.X = Math.Cos(DegreesToRadians(this.yaw)) * Math.Cos(DegreesToRadians(this.pitch));
            this.front.Y = Math.Sin(DegreesToRadians(this.pitch));
            this.front.Z = Math.Sin(DegreesToRadians(this.yaw)) * Math.Cos(DegreesToRadians(this.pitch));
            this.front.Normalize();

            this.right = Vector3D.CrossProduct(this.front, new Vector3D(0, 1, 0));
            this.up = Vector3D.CrossProduct(this.right, this.front);
            this.right.Normalize();
            this.up.Normalize();

            this.camera.LookDirection = this.front;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public PerspectiveCamera GetCamera()
        {
            return this.camera;
        }
    }
}
