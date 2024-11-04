﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal class Rock : WorldEntity
    {
        public static Model Model;

        public Rock(Vector3 position, Vector3 scale, float yaw) : base(position, scale, yaw, Model)
        {
            _world = Matrix.CreateTranslation(new Vector3(10.194115f, -0.923026f, -1.0564915f)) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
            _defaultColors = GetDefaultColors(Model.Meshes.Count);
        }

        public static void LoadContent(ContentManager Content, Effect Effect)
        {
            Model = LoadContent(Content, "rock/rock", Effect);
        }

        protected void Update(GameTime gameTime)
        {
            // ¿¿??
        }

        public override BoundingBoxLocalCoordinates GetLocalBoundingBox(Model model)
        {
            Vector3 min = new Vector3(-11.78202851f, 0.034491f, -0.53142201f);
            Vector3 max = new Vector3(-8.60620149f, 1.799334f, 2.64440501f);
            BoundingBoxLocalCoordinates localBox = new BoundingBoxLocalCoordinates(min, max);
            localBox.ObjectPositionToBoxCenter = new Vector3(0f, -0.0061135f, 0f);
            return localBox;
        }

        public override Vector3[] GetDefaultColors(int meshes)
        {
            var num = Random.NextDouble() * 0.3f + 0.3f;
            float r = (float)(num + Random.NextDouble() * 0.05f);
            float g = (float)(num + Random.NextDouble() * 0.05f);
            float b = (float)(num + Random.NextDouble() * 0.05f);

            Vector3[] colors = new Vector3[meshes];
            colors[0] = new Vector3(r, g, b);
            return colors;
        }

        public override void Draw(Matrix view, Matrix projection, Effect effect)
        {
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);

            for (int i = 0; i < Model.Meshes.Count; i++)
            {
                effect.Parameters["DiffuseColor"].SetValue(_defaultColors[i]);
                effect.Parameters["World"].SetValue(Model.Meshes[i].ParentBone.Transform * _world);
                Model.Meshes[i].Draw();
            }
        }
    }
}
