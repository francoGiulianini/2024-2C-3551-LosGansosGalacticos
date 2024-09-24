using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP
{
    internal abstract class BasicObject
    {
        public static Random Random = new Random();

        protected Model model; // TODO mejorar cómo se carga esta variable
        protected Matrix world;
        protected Vector3 position;
        protected Vector3 scale;
        protected Quaternion rotation;
        protected Vector3 defaultColor;

        public BasicObject(Vector3 position)
        {
            this.position = position;
            scale = GetRandomScale();
            rotation = GetRandomRotation();
            defaultColor = GetRandomColor();
            world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(this.position);
        }


        public abstract bool Intersects(BasicObject obj);
        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);

        virtual public void Draw(Matrix view, Matrix projection, Effect effect)
        {
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["DiffuseColor"].SetValue(defaultColor);

            foreach (var mesh in model.Meshes)
            {
                effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                mesh.Draw();
            }
        }
        protected abstract Vector3 GetRandomScale();
        protected abstract Quaternion GetRandomRotation();
        protected abstract Vector3 GetRandomColor();


    }

}
