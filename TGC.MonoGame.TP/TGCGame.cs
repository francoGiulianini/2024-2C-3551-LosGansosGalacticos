﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Tank;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder = "Content";
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
        private Point ScreenCenter;
        private SpriteBatch SpriteBatch;
        private Gizmos.Gizmos Gizmos;
        private bool DrawBoundingBoxes = false;
        private bool DrawPositions = false;

        private Effect TerrainEffect;
        private Effect ObjectEffect;
        private SkyBox Sky;

        private BoundingFrustum BoundingFrustum;
        private CameraType SelectedCamera;
        private TargetCamera FollowCamera;
        private StaticCamera AerialCamera;
        private Camera _camera;
        private Camera Camera { get => _camera; }
        private static float CameraNearPlaneDistance = 1f;
        private static float CameraFarPlaneDistance = 2000f;
        private const float camX = 0.2f;
        private const float camY = -0.1f;

        // TODO crear clase para tanque jugador
        private Model Model { get; set; }
        private Steamroller tank;

        // terreno
        private Terrain terrain;
        private float terrainSize;
        private float heightScale;
        private List<WorldEntity> Entities;

        // Mapeo de teclas
        //private Dictionary<Keys, BindingAction> KeyBindings;
        private KeyboardState keyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState mouseState;
        private MouseState previousMouseState;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            ScreenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition(ScreenCenter.X, ScreenCenter.Y);

            // deshabilito el backface culling
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //KeyBindings = new() {
            //    {Keys.W, BindingLogic.PositiveDirection},
            //    {Keys.Up, BindingLogic.PositiveDirection},
            //    {Keys.A, BindingLogic.PositiveRotation},
            //    {Keys.Left, BindingLogic.PositiveRotation},
            //    {Keys.S, BindingLogic.NegativeDirection},
            //    {Keys.Down, BindingLogic.NegativeDirection},
            //    {Keys.D, BindingLogic.NegativeRotation},
            //    {Keys.Right, BindingLogic.NegativeRotation},
            //};

            FollowCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            FollowCamera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, CameraNearPlaneDistance, CameraFarPlaneDistance);
            _camera = FollowCamera;

            AerialCamera = new StaticCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.UnitY * 1000f, new Vector3(-0.001f, -1f, -0.001f), Vector3.Up);
            AerialCamera.RightDirection = Vector3.UnitX;
            AerialCamera.BuildView();

            // Create a bounding frustum to check bounding volumes against it
            BoundingFrustum = new BoundingFrustum(FollowCamera.View * FollowCamera.Projection);

            Entities = new List<WorldEntity>();
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

            Gizmos = new Gizmos.Gizmos();
            Gizmos.LoadContent(GraphicsDevice, new ContentManager(Content.ServiceProvider, ContentFolder));
            Gizmos.Enabled = true;

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            TerrainEffect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            ObjectEffect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Cargo el tanque
            // TODO mover esto a su clase
            Stopwatch sw = Stopwatch.StartNew();
            Model = Content.Load<Model>(ContentFolder3D + "tank/tank");
            sw.Stop();
            Debug.WriteLine("Load model tank/tank: {0} milliseconds", sw.ElapsedMilliseconds);


            ApplyEffect(Model, ObjectEffect);
            tank = new Steamroller();
            tank.Position = new Vector3(0f, 2f, 0f); // TODO posición inicial tanque
            tank.World = Matrix.CreateTranslation(tank.Position);
            tank.Load(Model);


            terrainSize = 512f;
            heightScale = 0.5f;
            terrain = new(this, TerrainEffect, GraphicsDevice, (terrainSize, terrainSize), heightScale);


            // TODO setear position.Y, pitch y roll del tanque en la posición inicial

            Tree.LoadContent(Content, ObjectEffect);
            Rock.LoadContent(Content, ObjectEffect);
            Bush.LoadContent(Content, ObjectEffect);
            LoadSurfaceObjects();

            Model skyBox = Content.Load<Model>(ContentFolder3D + "geometries/cube");
            TextureCube skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "skybox/day_skybox");
            Effect skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "Skybox");
            Sky = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 1200);

            base.LoadContent();
            previousKeyboardState = Keyboard.GetState();
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


            // Capturar Input teclado y mouse
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            float mouseDeltaX = previousMouseState.Position.X - mouseState.Position.X;
            float mouseDeltaY = previousMouseState.Position.Y - mouseState.Position.Y;

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                // Salgo del juego.
                Exit();
            }

            // gizmos
            if (keyboardState.IsKeyDown(Keys.B) && previousKeyboardState.IsKeyUp(Keys.B))
            {
                DrawBoundingBoxes = !DrawBoundingBoxes;
            }
            if (keyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P))
            {
                DrawPositions = !DrawPositions;
            }
            if (keyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))
            {
                if (SelectedCamera == CameraType.Follow)
                {
                    SelectedCamera = CameraType.Aerial;
                    _camera = AerialCamera;
                }
                else if (SelectedCamera == CameraType.Aerial)
                {
                    SelectedCamera = CameraType.Follow;
                    _camera = FollowCamera;
                }
            }

            // rozamiento
            if (tank.Propulsion > 0)
            {
                tank.Propulsion = MathHelper.Clamp(tank.Propulsion - Steamroller.Friction, 0, Steamroller.SpeedLimit);
            }
            else if (tank.Propulsion < 0)
            {
                tank.Propulsion = MathHelper.Clamp(tank.Propulsion + Steamroller.Friction, -Steamroller.SpeedLimit, 0);
            }

            // dirección rotación
            if ((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)))
            {
                tank.Yaw -= elapsedTime;
                tank.SteerRotation -= elapsedTime;
            }
            else if ((keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)))
            {
                tank.Yaw += elapsedTime;
                tank.SteerRotation += elapsedTime;
            }

            // avance/retroceso
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                tank.Propulsion = MathHelper.Clamp(tank.Propulsion + Steamroller.SpeedIncrease, -Steamroller.SpeedLimit, Steamroller.SpeedLimit);
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                tank.Propulsion = MathHelper.Clamp(tank.Propulsion - Steamroller.SpeedIncrease, -Steamroller.SpeedLimit, Steamroller.SpeedLimit);
            }

            // torreta y cañon
            tank.TurretRotation += mouseDeltaX * elapsedTime * camX;
            tank.CannonRotation += mouseDeltaY * elapsedTime * camY;

            Matrix RotationMatrix = Matrix.CreateRotationY(tank.Yaw);
            Matrix CameraRotationMatrix = Matrix.CreateFromYawPitchRoll(tank.Yaw + tank.TurretRotation, -tank.CannonRotation, 0f);

            Vector3 movement = RotationMatrix.Forward * tank.Speed * elapsedTime;
            tank.WheelRotation += (tank.Speed * elapsedTime / 8f); // TODO revisar esta fórmula
            tank.Position = tank.Position + movement;
            tank.Position.Y = Terrain.GetPositionHeight(tank.Position.X, tank.Position.Z);

            float distanceForward = 3.303362f;
            float distanceRight = 3.032239f;
            float clampPitch = elapsedTime / 4;
            float clampRoll = elapsedTime / 4;

            // pendiente hacia adelante/atrás 
            Vector3 positionForward = tank.Position + RotationMatrix.Forward * distanceForward;
            positionForward.Y = Terrain.GetPositionHeight(positionForward.X, positionForward.Z);
            float currentPitch = (tank.Position.Y - positionForward.Y) / (tank.Position - positionForward).Length();
            float deltaPitch = currentPitch - tank.Pitch;
            tank.Pitch += MathHelper.Clamp(deltaPitch, -clampPitch, clampPitch);

            // velocidad en pendiente
            tank.Downhill = tank.Propulsion * (float)Math.Sin(currentPitch);

            // pendiente hacia los costados
            Vector3 positionRight = tank.Position + RotationMatrix.Right * distanceRight;
            positionRight.Y = Terrain.GetPositionHeight(positionRight.X, positionRight.Z);
            float currentRoll = (tank.Position.Y - positionRight.Y) / (tank.Position - positionRight).Length();
            float deltaRoll = currentRoll - tank.Roll;
            tank.Roll += MathHelper.Clamp(deltaRoll, -clampRoll, clampRoll);


            tank.World = Matrix.CreateScale(0.01f) * Matrix.CreateFromYawPitchRoll(tank.Yaw + MathHelper.Pi, tank.Pitch, tank.Roll) * Matrix.CreateTranslation(tank.Position); // TODO definir escala tanque



            FollowCamera.TargetPosition = tank.Position + CameraRotationMatrix.Forward * 40; // TODO revisar posición objetivo 
            FollowCamera.Position = tank.Position + CameraRotationMatrix.Backward * 20 + Vector3.UnitY * 12; // TODO revisar posición cámara
            FollowCamera.BuildView();


            // Update the view projection matrix of the bounding frustum
            BoundingFrustum.Matrix = FollowCamera.View * FollowCamera.Projection;

            Gizmos.UpdateViewProjection(Camera.View, Camera.Projection);


            //// colisiones entre tanque y objetos del escenario
            foreach (WorldEntity e in Entities)
            {
                if (e.Status != WorldEntityStatus.Destroyed)
                {
                    if (tank.Intersects(e.GetHitBox()))
                    {
                        e.Status = WorldEntityStatus.Destroyed;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(new Color(23 / 255.0f, 171 / 255.0f, 237 / 255.0f));

            // terreno
            terrain.Draw(Camera.View, Camera.Projection);

            int drawWorldEntity = 0;
            foreach (WorldEntity e in Entities)
            {
                if (e.Status != WorldEntityStatus.Destroyed && BoundingFrustum.Intersects(e.GetDrawBox()))
                {
                    terrain.spacialMap.Update(e);
                    e.Draw(Camera.View, Camera.Projection, ObjectEffect);
                    drawWorldEntity += 1;
                }
            }
            Debug.WriteLine(drawWorldEntity);

            tank.Draw(tank.World, Camera.View, Camera.Projection, ObjectEffect);

            Sky.Draw(Camera.View, Camera.Projection, Camera.Position);

            // gizmos
            if (DrawBoundingBoxes || DrawPositions)
            {
                foreach (WorldEntity e in Entities)
                {
                    if (e.Status != WorldEntityStatus.Destroyed && BoundingFrustum.Intersects(e.GetDrawBox()))
                    {
                        if (DrawBoundingBoxes)
                        {
                            e.DrawBoundingBox(Gizmos);
                        }
                        if (DrawPositions)
                        {
                            e.DrawPosition(Gizmos);
                        }
                    }
                }

                if (DrawBoundingBoxes)
                {
                    tank.DrawBoundingBox(Gizmos);
                    Gizmos.DrawFrustum(FollowCamera.View * FollowCamera.Projection, Color.White);
                }

                Gizmos.Draw();
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

        private void LoadSurfaceObjects()
        {
            Random rnd = new Random();
            int treeCount = 0;
            int bushCount = 0;
            int rockCount = 0;

            for (int i = 0; i < 200; i++)
            {
                // posición
                float x = (float)rnd.NextDouble() * terrainSize - terrainSize / 2;
                float z = (float)rnd.NextDouble() * terrainSize - terrainSize / 2;
                float y = Terrain.GetPositionHeight(x, z);

                // escala
                float height = (float)rnd.NextDouble() * 0.4f + 0.8f;
                float width = (float)rnd.NextDouble() * 0.4f + 0.8f;

                // rotación
                float rot = (float)rnd.NextDouble() * MathHelper.TwoPi;
                float objType = (float)rnd.NextDouble();

                if (objType > 0.4f)
                {
                    Tree t = new(new Vector3(x, y, z), new Vector3(width, height, width), rot);
                    Entities.Add(t);
                    terrain.spacialMap.Add(t);
                    treeCount += 1;
                }
                else if (objType > 0.2f)
                {
                    Bush b = new(new Vector3(x, y, z), new Vector3(width, height, width), rot);
                    Entities.Add(b);
                    terrain.spacialMap.Add(b);
                    bushCount += 1;
                }
                else
                {
                    Rock r = new(new Vector3(x, y, z), new Vector3(width, height, width), rot);
                    Entities.Add(r);
                    terrain.spacialMap.Add(r);
                    rockCount += 1;
                }

            }

            Debug.WriteLine("Trees: {0}", treeCount);
            Debug.WriteLine("Bushes: {0}", bushCount);
            Debug.WriteLine("Rocks: {0}", rockCount);
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
