using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal class Bush : BasicObject
    {

        public Bush(Vector3 position, ModelManager modelManager) : base(position)
        {
            model = modelManager.Bush;
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
            float height = (float) Random.NextDouble() * 0.5f + 1f;
            float width = (float) Random.NextDouble() * 0.5f + 1f;
            return new Vector3(width, height, width);
        }

        protected override Quaternion GetRandomRotation()
        {
            float yaw = (float) Random.NextDouble() * MathHelper.TwoPi;
            return Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f);
        } 

        protected override Vector3 GetRandomColor()
        {
            float green = (float) Random.NextDouble() * 0.4f + 0.1f;
            return new Vector3(0f, green, 0f);
        }
    }
}
