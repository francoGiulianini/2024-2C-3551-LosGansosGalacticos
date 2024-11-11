using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Collisions;


namespace TGC.MonoGame.TP {
    public class WorldEntity {
        private const string ContentFolder3D = "Models/";
        public static Random Random;
        protected Vector3 _position;

        protected BoundingBox _boundingBox;

        // distintos bounding volumes para comparar
        protected BoundingBox _boundingBoxPrecalc;
        protected BoundingBox _boundingBoxCalc;
        protected BoundingCylinder _boundingCylinderPrecalc;
        protected BoundingCylinder _boundingCylinderCalc;

        protected Vector3[] _defaultColors;
        protected Matrix _world;
        protected Vector3 _scale;
        protected float _yaw;
        protected ((int gridX, int gridZ) Min, (int gridX, int gridZ) Max) gridIndices = new();

        public WorldEntity(Vector3 position, Vector3 scale, float yaw, Model model) {
            _position = position;
            _scale = scale;
            _yaw = yaw;

            BoundingBoxLocalCoordinates localBox;

            Stopwatch sw = Stopwatch.StartNew();
            localBox = GetLocalBoundingBox(model);
            _boundingBoxPrecalc.Min = position + (localBox.ObjectPositionToBoxCenter - localBox.Distance) * scale;
            _boundingBoxPrecalc.Max = position + (localBox.ObjectPositionToBoxCenter + localBox.Distance) * scale;
            sw.Stop();
            Debug.WriteLine("_boundingBoxPrecalc: {0} milliseconds", sw.ElapsedMilliseconds);

            sw.Restart();
            localBox = GetLocalBoundingBox(model);
            _boundingCylinderPrecalc = new BoundingCylinder(position + localBox.ObjectPositionToBoxCenter * scale, localBox.Distance.X * scale.X, localBox.Distance.Y * scale.Y);
            sw.Stop();
            Debug.WriteLine("_boundingCylinderPrecalc: {0} milliseconds", sw.ElapsedMilliseconds);

        }

        public Vector3 GetPosition() {
            return _position;
        }

        public static Model LoadContent(ContentManager Content, string modelRelativePath, Effect Effect)
        {
            // Cargo el modelo
            Model model = Content.Load<Model>(ContentFolder3D + modelRelativePath);
            Random = new Random();
            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            foreach (var mesh in model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
            
            return model;
        }

        public virtual void Draw(Matrix view, Matrix projection, Effect effect) {}
        public void DrawBoundingBox(Gizmos.Gizmos gizmos)
        {
            gizmos.DrawCube((_boundingBoxPrecalc.Max + _boundingBoxPrecalc.Min) / 2f, _boundingBoxPrecalc.Max - _boundingBoxPrecalc.Min, Color.Red);
            gizmos.DrawCube((_boundingBoxCalc.Max + _boundingBoxCalc.Min) / 2f, _boundingBoxCalc.Max - _boundingBoxCalc.Min, Color.Blue);
            gizmos.DrawCylinder(_boundingCylinderPrecalc.Transform, Color.Yellow);
            gizmos.DrawCylinder(_boundingCylinderCalc.Transform, Color.White);
        }

        public void DrawPosition(Gizmos.Gizmos gizmos)
        {
            gizmos.DrawSphere(_position, Vector3.One, Color.White);
        }

        public virtual Vector3[] GetDefaultColors(int meshes) {
            Vector3[] colors = new Vector3[meshes];
            for (int i = 0; i < meshes; i++) 
            {
                colors[i] = new Vector3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble());
            }
            return colors;
        }

        public void DebugCollision(CollisionData data) {
            var x = data.gridPosition.x / (float) data.gridSize.x;
            var z = data.gridPosition.z / (float) data.gridSize.z;

            for (int i = 0; i < _defaultColors.Length; i++)
            {
                _defaultColors[i] = new Vector3(x, 0, z);
            }
        }

        public BoundingBox GetBoundingBox() {
            return _boundingBox;
        }

        public virtual BoundingBoxLocalCoordinates GetLocalBoundingBox(Model model)
        {
            return new BoundingBoxLocalCoordinates(model.Meshes[0].BoundingSphere.Center, model.Meshes[0].BoundingSphere.Radius);
        }

        public ((int gridX, int gridZ) Min, (int gridX, int gridZ) Max) GetGridIndices() {
            return gridIndices;
        }

        public void SetGridMinIndex(int gridMinX, int gridMinZ) {
            gridIndices.Min.gridX = gridMinX;
            gridIndices.Min.gridZ = gridMinZ;
        }

        public void SetGridMaxIndex(int gridMaxX, int gridMaxZ) {
            gridIndices.Max.gridX = gridMaxX;
            gridIndices.Max.gridZ = gridMaxZ;
        }

        public void SetGridIndices(int gridMinX, int gridMinZ, int gridMaxX, int gridMaxZ) {
            SetGridMinIndex(gridMinX, gridMinZ);
            SetGridMaxIndex(gridMaxX, gridMaxZ);
        }
    }
}
