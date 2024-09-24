using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    internal class ModelManager
    {

        private ContentManager Content;
        private string ContentFolder3D;
        private string ContentFolderEffects;

        public Effect BasicShader { get; set; }
        public Model Steamroller { get; set; }
        public Model Tree { get; set; }
        public Model Bush { get; set; }
        public Model Rock { get; set; }

        public ModelManager(ContentManager content, string folder3D, string folderEffects)
        {
            Content = content;
            ContentFolder3D = folder3D;
            ContentFolderEffects = folderEffects;

            LoadContent();
        }

        private void LoadContent()
        {
            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            BasicShader = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Cargo el tanque
            Steamroller = LoadModel(ContentFolder3D + "tank/tank");

            // Cargo el árbol
            Tree = LoadModel(ContentFolder3D + "tree/tree");

            // Cargo la roca
            Rock = LoadModel(ContentFolder3D + "rock/Rock1");

            // Cargo el arbusto
            Bush = LoadModel(ContentFolder3D + "bush/Bush1");
        }

        private Model LoadModel(string path)
        {
            Model model = Content.Load<Model>(path);
            ApplyEffect(model, BasicShader);
            return model;
        }

        private void ApplyEffect(Model model, Effect effect)
        {
            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            foreach (var mesh in model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect;
                }
            }
        }

    }
}
