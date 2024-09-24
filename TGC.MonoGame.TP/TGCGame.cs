using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Geometries.Textures;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }


        private Random rnd = new Random();

        private TargetCamera Camera { get; set; }
        public static float CameraNearPlaneDistance { get; set; } = 1f;
        public static float CameraFarPlaneDistance { get; set; } = 2000f;


        // TODO crear clase para tanque jugador
        private ModelManager ModelMgr;
        private Effect Effect;
        private Model Model { get; set; }
        private float Rotation { get; set; }
        private Vector3 Position { get; set; }
        private Matrix World { get; set; }




        // terreno
        private QuadPrimitive Terrain;
        private List<BasicObject> StageObjects;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            Terrain = new QuadPrimitive(Graphics.GraphicsDevice);

            Camera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            Camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, CameraNearPlaneDistance, CameraFarPlaneDistance);

            // Configuramos nuestras matrices de la escena.
            Position = new Vector3(0f, 2f, 0f); // TODO posición inicial tanque
            World = Matrix.CreateTranslation(Position);

            StageObjects = new List<BasicObject>();
            Random rnd = new Random();

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // cargar modelos
            ModelMgr = new ModelManager(Content, ContentFolder3D, ContentFolderEffects);
            Effect = ModelMgr.BasicShader;
            Model = ModelMgr.Steamroller;

            LoadStageObjects();

            base.LoadContent();
        }



        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Capturar Input teclado
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                // Salgo del juego.
                Exit();
            }

            float direction = 0f;
            
            // TODO revisar dirección rotación y avance/retroceso
            if ((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)))
            {
                Rotation -= elapsedTime;
            }
            else if ((keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)))
            {
                Rotation += elapsedTime;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                direction = elapsedTime;
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                direction = -elapsedTime;
            }


            Matrix RotationMatrix = Matrix.CreateRotationY(Rotation);
            Vector3 movement = direction * RotationMatrix.Forward * 25;  // TODO definir velocidad
            Position = Position + movement;
            World = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(Position); // TODO definir escala tanque

            Terrain.World = Matrix.Identity * Matrix.CreateScale(100f); // TODO definir escala terreno

            Camera.TargetPosition = Position + RotationMatrix.Forward * 10; // TODO revisar posición objetivo 
            Camera.Position = Position + RotationMatrix.Backward * 20 + Vector3.UnitY * 10; // TODO revisar posición cámara
            Camera.BuildView();


            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(new Color(0.529f, 0.808f, 0.922f));

            // terreno
            Terrain.Draw(Camera.View, Camera.Projection, Effect, new Vector3(0.50f, 1.00f, 0.00f));


            // objetos del escenario
            foreach (BasicObject obj in StageObjects)
            {
                obj.Draw(Camera.View, Camera.Projection, Effect);
            }

            // tanque
            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            Effect.Parameters["View"].SetValue(Camera.View);
            Effect.Parameters["Projection"].SetValue(Camera.Projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.33f,0.33f,0.20f));

            foreach (var mesh in Model.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World);
                mesh.Draw();
            }

        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

        private void LoadStageObjects()
        {
            // árboles
            for (int i = 0; i < 100; i++)
            {
                StageObjects.Add(new Tree(GetRandomPostion(), ModelMgr));
            }
            // arbustos
            for (int i = 0; i < 50; i++)
            {
                StageObjects.Add(new Bush(GetRandomPostion(), ModelMgr));
            }
            // rocas
            for (int i = 0; i < 20; i++)
            {
                StageObjects.Add(new Rock(GetRandomPostion(), ModelMgr));
            }

        }

        private Vector3 GetRandomPostion()
        {
            // posición
            float x = (float)rnd.NextDouble() * 200f - 100f;
            float y = 0;
            float z = (float)rnd.NextDouble() * 200f - 100f;
            return new Vector3(x, y, z);
        }

    }
}