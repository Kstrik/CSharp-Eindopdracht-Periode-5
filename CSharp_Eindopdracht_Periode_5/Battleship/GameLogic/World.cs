using Battleship.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Battleship.GameLogic
{
    public class World
    {
        private Model3DGroup modelGroup;
        private ModelVisual3D modelVisual;

        private List<GameObject> gameObjects;
        private List<Light> lights;
        private List<Camera> cameras;
        public Camera CurrentCamera
        {
            get { return this.currentCamera; }
            set { SetCurrentCamera(value); }
        }
        private Camera currentCamera;

        private Game game;
        private Viewport3D viewport;

        public World(Game game, ref Viewport3D viewport)
        {
            this.modelGroup = new Model3DGroup();
            this.modelVisual = new ModelVisual3D();
            this.modelVisual.Content = this.modelGroup;
            viewport.Children.Add(this.modelVisual);

            this.gameObjects = new List<GameObject>();
            this.lights = new List<Light>();
            this.cameras = new List<Camera>();

            this.game = game;
            this.viewport = viewport;
        }

        public void Update(float deltatime)
        {
            foreach (GameObject gameObject in this.gameObjects)
                gameObject.Update(deltatime);
        }

        public void AddGameObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                this.gameObjects.Add(gameObject);
                this.game.DispatchAction(new Action(() => modelGroup.Children.Add(gameObject.GeometryModel)));
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                this.gameObjects.Remove(gameObject);
                this.game.DispatchAction(new Action(() => modelGroup.Children.Remove(gameObject.GeometryModel)));
            }
        }

        public void AddLight(Light light)
        {
            if (light != null)
            {
                this.lights.Add(light);
                this.game.DispatchAction(new Action(() => modelGroup.Children.Add(light)));
            }
        }

        public void RemoveLight(Light light)
        {
            if (light != null)
            {
                this.lights.Remove(light);
                this.game.DispatchAction(new Action(() => modelGroup.Children.Remove(light)));
            }
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
                this.game.DispatchAction(new Action(() => this.viewport.Camera = this.currentCamera));
            }
        }
    }
}
