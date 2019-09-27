using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Battleship.GameObjects
{
    public class GameObject
    {
        private Transform3DGroup transform;
        private TranslateTransform3D translation;
        private RotateTransform3D rotation;
        private AxisAngleRotation3D axisAngle;
        private ScaleTransform3D scale;

        public GeometryModel3D GeometryModel
        {
            get { return this.geometryModel; }
            set { SetGeometryModel(value); }
        }
        private GeometryModel3D geometryModel;

        public Vector3D position;
        public Vector3D velocity;
        public Vector3D rotateAxis;
        public double angle;
        public Vector3D scaling;

        public GameObject()
        {
            this.position = new Vector3D(0, 0, 0);
            this.velocity = new Vector3D(0, 0, 0);
            this.rotateAxis = new Vector3D(0, 1, 0);
            this.angle = 0;
            this.scaling = new Vector3D(1, 1, 1);

            this.transform = new Transform3DGroup();
            UpdateTransformations();

            this.transform.Children.Add(this.translation);
            this.transform.Children.Add(this.rotation);
            this.transform.Children.Add(this.scale);
        }

        public void Update(float deltatime)
        {
            this.position += this.velocity * deltatime;

            UpdateTransformations();
            this.geometryModel.Transform = this.transform;
        }

        private void UpdateTransformations()
        {
            this.transform.Children.Clear();
            this.translation = new TranslateTransform3D(this.position);
            this.axisAngle = new AxisAngleRotation3D(this.rotateAxis, this.angle);
            this.rotation = new RotateTransform3D(this.axisAngle);
            this.scale = new ScaleTransform3D(this.scaling);

            this.transform.Children.Add(this.translation);
            this.transform.Children.Add(this.rotation);
            this.transform.Children.Add(this.scale);
        }

        private void SetGeometryModel(GeometryModel3D geometryModel)
        {
            this.geometryModel = geometryModel;
            this.geometryModel.Transform = this.transform;

            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource = new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\M4_Albedo.png", UriKind.Absolute));
            DiffuseMaterial myDiffuseMaterial = new DiffuseMaterial(colors_brush);
            this.geometryModel.Material = myDiffuseMaterial;
            this.geometryModel.BackMaterial = myDiffuseMaterial;
        }
    }
}
