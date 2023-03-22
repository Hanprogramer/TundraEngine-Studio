using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.ObjectModel;
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

        private bool isDragged = false;
        private Point lastPoint;

        private float camZoomTo = 1.0f;

        public ObservableCollection<SceneObjectData> Objects { get; set; } = new();

        public SceneEditor()
        {
            DataContext = this;
            InitializeComponent();
        }
        private void InitializeChild()
        {
            foreach (var obj in Scene.objects)
            {
                Objects.Add(obj);
            }
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
            MainTundraView.Scene = Scene.Instantiate(MainTundraView, TundraStudio.CurrentProject.ResourceManager, TundraStudio.Asl);
            try
            {
                //MainTundraView.Scene.Initialize();
                //MainTundraView.Scene.Update(0);
            }
            catch (Exception e)
            {
                // TODO: better exception handling
                Console.WriteLine(e);
            }
            // Create an editor camera
            CameraObject = new Camera(MainTundraView.Scene, MainTundraView);
            CameraObject.FlipY = true;
            MainTundraView.Scene.AddObject(CameraObject);
            Console.WriteLine("Initialized");
            
            InitializeChild();
        }

        /// <summary>
        /// Setup the editor interactions
        /// </summary>
        public void SetupInteractions()
        {
            MainTundraView.PointerPressed += MainTundraViewOnPointerPressed;
            MainTundraView.PointerReleased += MainTundraViewOnPointerReleased;
            MainTundraView.PointerMoved += MainTundraViewOnPointerMoved;
            MainTundraView.PointerWheelChanged += MainTundraViewOnPointerWheelChanged;
        }
        private void MainTundraViewOnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            float zoomFactor = 1.5f;
            if (e.Delta.Y > 0)
                camZoomTo *= zoomFactor; 
            if (e.Delta.Y < 0)
                camZoomTo /= zoomFactor;
            e.Handled = true;
        }
        private void MainTundraViewOnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (isDragged)
            {
                var currentPoint = e.GetPosition((Control)e.Source);
                var delta = new Point(lastPoint.X - currentPoint.X, lastPoint.Y - currentPoint.Y);
                CameraObject.Position.X += MainTundraView.ToActualPixel((float)delta.X) / CameraObject.Zoom;
                CameraObject.Position.Y -= MainTundraView.ToActualPixel((float)delta.Y) / CameraObject.Zoom;
                lastPoint = currentPoint;
            }
        }
        private void MainTundraViewOnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if(e.InitialPressMouseButton == MouseButton.Middle)
            isDragged = false;
        }
        private void MainTundraViewOnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(null).Properties.IsMiddleButtonPressed)
            {
                isDragged = true;
                lastPoint = e.GetPosition((Control)e.Source);
            }
        }
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            MainTundraView = this.FindControl<TundraView>("MainTundraView");
            if (MainTundraView != null && MainTundraView.Renderer != null)
            {
                if (!IsInitialized)
                {
                    SetupInteractions();
                    InitializeSceneEditor();
                    IsInitialized = true;
                }
                else if(TundraStudio.CurrentProject.IsRmInitialized)
                    TundraStudio.CurrentProject.EnsureTextureLoaded(MainTundraView.Renderer);
                
                //MainTundraView.Scene.Update(0);
            }

            if(CameraObject!=null)
            CameraObject.Zoom += (camZoomTo - CameraObject.Zoom) / 2f;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

