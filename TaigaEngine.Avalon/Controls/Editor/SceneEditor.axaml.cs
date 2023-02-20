using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using TundraEngine.Classes;
using TundraEngine.Classes.Data;
using TundraEngine.Rendering;
using TundraEngine.Studio.Util;
using Transform = TundraEngine.Components.Transform;

namespace TundraEngine.Studio.Controls.Editor
{

    public partial class SceneEditor : UserControl
    {
        public SceneResource Scene { get; set; }
        private bool IsInitialized = false;
        public Camera CameraObject;
        
        public SceneEditor()
        {
            DataContext = this;
            InitializeComponent();
        }
        public SceneEditor(SceneResource scene)
        {
            Scene = scene;
            DataContext = this;
            InitializeComponent();
        }
        public async void InitializeSceneEditor()
        {
            // Instantiate Resource Manager
            await TundraStudio.CurrentProject.InitializeResourceManager(MainTundraView.Renderer);

            // Sets the view's scene to the scene
            MainTundraView.Scene = Scene.Instantiate(MainTundraView, TundraStudio.CurrentProject.ResourceManager);;

            MainTundraView.Scene.Initialize();
            MainTundraView.Scene.Update(0);
            // Create an editor camera
            CameraObject = new Camera(MainTundraView.Scene, MainTundraView);
            CameraObject.FlipY = true;
            MainTundraView.Scene.AddObject(CameraObject);
            Console.WriteLine("Initialized");
        }
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            MainTundraView = this.FindControl<TundraView>("MainTundraView");
            if (MainTundraView != null && MainTundraView.Renderer != null)
            {
                if (!IsInitialized)
                {
                    InitializeSceneEditor();
                    IsInitialized = true;
                }
                else if(TundraStudio.CurrentProject.IsRmInitialized)
                    TundraStudio.CurrentProject.EnsureTextureLoaded(MainTundraView.Renderer);
                
                MainTundraView.Scene.Update(0);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

