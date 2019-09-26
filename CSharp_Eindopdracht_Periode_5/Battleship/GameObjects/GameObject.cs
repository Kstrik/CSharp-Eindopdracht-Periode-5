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

        protected Vector3D position;
        protected Vector3D velocity;
        protected Vector3D rotateAxis;
        protected Vector3D scaling;

        public GameObject()
        {
            this.position = new Vector3D(0, 0, 0);
            this.velocity = new Vector3D(0, 0, 0);
            this.rotateAxis = new Vector3D(0, 1, 0);
            this.scaling = new Vector3D(1, 1, 1);

            this.transform = new Transform3DGroup();
            this.translation = new TranslateTransform3D(this.position);
            this.axisAngle = new AxisAngleRotation3D(this.rotateAxis, 0);
            this.rotation = new RotateTransform3D(this.axisAngle);
            this.scale = new ScaleTransform3D(this.scaling);

            this.transform.Children.Add(this.translation);
            this.transform.Children.Add(this.rotation);
            this.transform.Children.Add(this.scale);
        }

        public void Update(float deltatime)
        {
            this.position += new Vector3D(0, 1, 0) * deltatime;
            Transform3DGroup test = new Transform3DGroup();
            this.translation.Transform(this.position + (new Vector3D(0, 1, 0) * deltatime));
            test.Children.Add(this.translation);
            test.Children.Add(this.rotation);
            test.Children.Add(this.scale);
            this.geometryModel.Transform = test;
        }

        private void SetGeometryModel(GeometryModel3D geometryModel)
        {
            this.geometryModel = geometryModel;
            this.geometryModel.Transform = this.transform;

            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource = new BitmapImage(new Uri(@"C:\Users\Kenley Strik\Desktop\Tree_01.png", UriKind.Absolute));
            DiffuseMaterial myDiffuseMaterial = new DiffuseMaterial(colors_brush);
            this.geometryModel.Material = myDiffuseMaterial;
            this.geometryModel.BackMaterial = myDiffuseMaterial;
        }
    }
}
