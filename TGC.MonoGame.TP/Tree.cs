﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal class Tree : WorldEntity
    {
        public static Model Model;

        public Tree(Vector3 position, Vector3 scale, float yaw) : base(position, scale, yaw, Model)
        {
            _world = Matrix.CreateScale(scale) *  Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
            _defaultColor = GetDefaultColor();
        }

        public static void LoadContent(ContentManager Content, Effect Effect)
        {
            Model = LoadContent(Content, "tree/tree", Effect);
        }

        protected void Update(GameTime gameTime)
        {
            // ¿¿??
        }

        public override Vector3 GetDefaultColor()
        {
            float r = (float)(Random.NextDouble() * 0.2f) + 0.2f;
            float g = (float)(Random.NextDouble() * 0.2f) + 0.8f;
            float b = (float)(Random.NextDouble() * 0.5f);

            return new Vector3(r, g, b);
        }

        public override void Draw(Matrix view, Matrix projection, Effect effect)
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
    }
}
