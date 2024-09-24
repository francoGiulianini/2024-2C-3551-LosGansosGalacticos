using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP
{
    internal class Bush
    {
        public static Model Model;
        public static Random Random;

        private Matrix _world;
        private Vector3 _position;
        private Vector3 _scale;
        private float _yaw;
        private Vector3 _defaultColor;

        public Bush(Vector3 position, Vector3 scale, float yaw)
        {
            _position = position;
            _scale = scale;
            _yaw = yaw;
            _world =Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);

            _defaultColor = GetDefaultColor();
        }

        public void LoadContent()
        {
            // TODO cargar modelo
        }

        protected void Update(GameTime gameTime)
        {
            // ¿¿??
        }

        public void Draw(Matrix view, Matrix projection, Effect effect)
        {
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["DiffuseColor"].SetValue(_defaultColor);

            foreach (var mesh in Model.Meshes)
            {
                effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _world);
                mesh.Draw();
            }
        }

        public Vector3 GetDefaultColor()
        {
            float r = 0f;
            float g = (float)(Random.NextDouble() * 0.4f) + 0.1f;
            float b = 0f;

            return new Vector3(r, g, b);
        }



    }
}
