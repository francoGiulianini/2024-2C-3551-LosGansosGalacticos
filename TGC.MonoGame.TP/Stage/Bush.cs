﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP
{
    internal class Bush : WorldEntity
    {
        private static Model Model;
        private static Texture[] Textures;
        private static BoundingBoxHelper ModelBoundingBox;

        public Bush(Vector3 position, Vector3 scale, float yaw) : base(position, scale, yaw, Model)
        {
            _world = Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(0,-0.909759561f, 0) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(position);
        }

        public static void LoadContent(ContentManager Content, Effect Effect)
        {
            Model = LoadContent(Content, "bush/bush1", Effect);
            Textures = new Texture[Model.Meshes.Count];
            Texture branch = Content.Load<Texture2D>("Models/bush/branch1 2");
            Texture leaf = Content.Load<Texture2D>("Models/bush/leaf2");
            Textures[0] = branch;
            Textures[1] = leaf;

            Vector3 min = new Vector3(-1.55790175f, -0.743808021f, -1.55288585f);
            Vector3 max = new Vector3(1.76620225f, 2.097365639f, 1.77121815f);
            ModelBoundingBox = new BoundingBoxHelper(min, max);
        }

        public override void Update(float elapsedTime)
        {
            // ¿¿??
        }

        protected override BoundingBox CreateBoundingBox(Model model, Vector3 position, Vector3 scale)
        {
            return ModelBoundingBox.GetBoundingBox(position, scale);
        }

        public override void Draw(Matrix view, Matrix projection, Effect effect)
        {
            base.Draw(view, projection, effect, Model, Textures); 
        }
    }
}
