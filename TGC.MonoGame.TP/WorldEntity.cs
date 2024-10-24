using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP {
    public class WorldEntity {
        private const string ContentFolder3D = "Models/";
        public static Random Random;
        protected Vector3 _position;
        protected BoundingBox _boundingBox;
        protected Vector3 _defaultColor;
        protected Matrix _world;
        protected Vector3 _scale;
        protected float _yaw;
        protected ((int gridX, int gridZ) Min, (int gridX, int gridZ) Max) gridIndices = new();

        public WorldEntity(Vector3 position, Vector3 scale, float yaw, Model model) {
            _position = position;
            _scale = scale;
            _yaw = yaw;

            float radius = model.Meshes[0].BoundingSphere.Radius;
            Vector3 center = model.Meshes[0].BoundingSphere.Center + _position;
            Vector3 offset = new(radius, radius, radius);
            _boundingBox.Min = center - offset;
            _boundingBox.Max = center + offset;
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

        public virtual Vector3 GetDefaultColor() {
            return new Vector3((float) Random.NextDouble() * 255, (float) Random.NextDouble() * 255, (float) Random.NextDouble() * 255);
        }

        public void DebugCollision(CollisionData data) {
            var x = data.gridPosition.x / (float) data.gridSize.x;
            var z = data.gridPosition.z / (float) data.gridSize.z;
            _defaultColor = new Vector3(x, 0, z);
        }

        public BoundingBox GetBoundingBox() {
            return _boundingBox;
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
