using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal class Rock : BasicObject
    {

        public Rock(Vector3 position, ModelManager modelManager) : base(position)
        {
            model = modelManager.Rock;
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
            float height = (float) Random.NextDouble() * 0.5f + 0.5f;
            float width = (float) Random.NextDouble() * 0.5f + 0.5f;
            return new Vector3(width, height, width);
        }

        protected override Quaternion GetRandomRotation()
        {
            float yaw = (float) Random.NextDouble() * MathHelper.TwoPi;
            return Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f);
        }

        protected override Vector3 GetRandomColor()
        {
            float gray = (float) Random.NextDouble() * 0.5f + 0.25f;
            return new Vector3(gray, gray, gray);
        }
    }
}
