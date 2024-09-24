using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal class Tree : BasicObject
    {

        public Tree(Vector3 position, ModelManager modelManager) : base(position)
        {
            model = modelManager.Tree;
        }

        public override bool Intersects(BasicObject obj)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 GetRandomScale()
        {
            float height = (float) Random.NextDouble() * 0.4f + 0.8f;
            float width = (float) Random.NextDouble() * 0.4f + 0.8f;
            return new Vector3(width, height, width);
        }

        protected override Quaternion GetRandomRotation()
        {
            float pitch = -MathHelper.PiOver2;
            float yaw = (float)Random.NextDouble() * MathHelper.TwoPi;
            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0f);
        }

        protected override Vector3 GetRandomColor()
        {
            float red = (float) Random.NextDouble() * 0.2f + 0.2f;
            float green = (float) Random.NextDouble() * 0.2f + 0.8f;
            float blue = (float) Random.NextDouble() * 0.5f;
            return new Vector3(red, green, blue);
        }
    }
}
