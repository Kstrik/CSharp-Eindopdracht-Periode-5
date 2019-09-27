using Battleship.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Battleship
{
    public class World
    {
        private List<Light> lights;
        private List<GameObject> gameObjects;

        private List<Camera> cameras;
        public Camera CurrentCamera
        {
            get { return this.currentCamera; }
            set { SetCurrentCamera(value); }
        }
        private Camera currentCamera;

        public World()
        {
            this.lights = new List<Light>();
            this.gameObjects = new List<GameObject>();

            this.cameras = new List<Camera>();
        }

        public void AddCamera(Camera camera)
        {
            if (camera != null)
                this.cameras.Add(camera);
        }

        public void RemoveCamera(Camera camera)
        {
            if (camera != null)
            {
                if (this.currentCamera == camera)
                    this.currentCamera = null;

                this.cameras.Remove(camera);
            }
        }

        private void SetCurrentCamera(Camera camera)
        {
            if(camera != null)
            {
                if (!this.cameras.Contains(camera))
                    this.cameras.Add(camera);

                this.currentCamera = camera;
            }
        }
    }
}
