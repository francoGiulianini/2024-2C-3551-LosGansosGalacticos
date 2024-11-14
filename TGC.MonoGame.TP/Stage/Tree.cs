﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP
{
    internal class Tree : WorldEntity
    {
        private static Model Model;
        private static BoundingBoxHelper ModelBoundingBox;
        private static BoundingBoxHelper ModelDrawBox;
        private BoundingBox _drawBox;

        public Tree(Vector3 position, Vector3 scale, float yaw) : base(position, scale, yaw, Model)
        {
            _world = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
            _defaultColors = GetDefaultColors(Model.Meshes.Count);

            _drawBox = ModelDrawBox.GetBoundingBox(position, scale);
        }

        public static void LoadContent(ContentManager Content, Effect Effect)
        {
            Model = LoadContent(Content, "tree/tree", Effect);

            Vector3 min = new Vector3(-0.48368357f, -0.015338364f, -0.44941229f);
            Vector3 max = new Vector3(0.28571863f, 15.816038f, 0.31998991f);
            ModelBoundingBox = new BoundingBoxHelper(min, max);

            Vector3 drawMin = new Vector3(-6.42365616f, -0.015338f, -5.749832324f);
            Vector3 drawMax = new Vector3(5.19512384f, 15.816038f, 5.868947676f);
            ModelDrawBox = new BoundingBoxHelper(drawMin, drawMax);
        }

        protected void Update(GameTime gameTime)
        {
            // ¿¿??
        }

        protected override BoundingBox CreateBoundingBox(Model model, Vector3 position, Vector3 scale)
        {
            return ModelBoundingBox.GetBoundingBox(position, scale);
        }

        protected override Vector3[] GetDefaultColors(int meshes)
        {
            float g = (float)(Random.NextDouble() * 0.2f) + 0.33f;
            float rb = (float)(Random.NextDouble() * 0.1f) + 0.05f;
            Vector3 green = new Vector3(rb, g, rb);

            float r = (float)(Random.NextDouble() * 0.11f) + 0.25f;
            float gb = (float)(Random.NextDouble() * 0.12f) + 0.05f;
            Vector3 brown = new Vector3(r, gb, gb);

            Vector3[] colors = new Vector3[meshes];
            colors[0] = brown;
            colors[1] = brown;
            colors[2] = brown;
            colors[3] = green;
            colors[4] = green;
            colors[5] = green;
            return colors;
        }

        public override BoundingBox GetDrawBox()
        {
            return _drawBox;
        }

        public override void DrawBoundingBox(Gizmos.Gizmos gizmos)
        {
            gizmos.DrawCube((_boundingBox.Max + _boundingBox.Min) / 2f, _boundingBox.Max - _boundingBox.Min, Color.Red);
            gizmos.DrawCube((_drawBox.Max + _drawBox.Min) / 2f, _drawBox.Max - _drawBox.Min, Color.Blue);
        }

        public override void Draw(Matrix view, Matrix projection, Effect effect)
        {
            base.Draw(view, projection, effect, Model);
        }
    }
}