using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using ReactiveUI;
using System;
using TundraEngine.Classes.Data;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Controls
{
    public partial class SpritePropEditor : UserControl, IPropertyEditor
    {
        public string Label { get; set; } = "Prop.Label";
        private SpriteResource? _sprite;
        public Avalonia.Media.Imaging.Bitmap? Bitmap
        {
            get => _sprite?.GetAvaloniaBitmap();
        }
        public SpriteResource? Sprite
        {
            get => _sprite;
            set
            {
                // Raise the changed event
                SetAndRaise(SpriteProperty, ref _sprite, value);

                // Store it to the prop data
                Data.Value = value;

                // Sets the previewer image
                if (value != null)
                {
                    MainImage.Source = Bitmap;
                    MainButton.IsEnabled = true;
                    ValueLabel = value!.filename;
                }
                else
                {
                    MainImage.Source = null;
                    MainButton.IsEnabled = false;
                    ValueLabel = "None";
                }
            }
        }

        public static readonly DirectProperty<SpritePropEditor, SpriteResource?> SpriteProperty =
            AvaloniaProperty.RegisterDirect<SpritePropEditor, SpriteResource?>(
                nameof(Sprite),
                o => o.Sprite,
                (o, v) => o.Sprite = v);

        public string ValueLabel { get => MainText.Text; set { MainText.Text = value; } }
        ObjectEditorPropertiesData Data { get; set; }

        public SpritePropEditor()
        {
            InitializeComponent();
        }
        public SpritePropEditor(ObjectEditorPropertiesData data)
        {
            Data = data;
            Label = data.Name;
            DataContext = this;
            _sprite = Data.Value as SpriteResource;
            InitializeComponent();
            DragDrop.SetAllowDrop(MainPanel, true);
            MainPanel.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            MainPanel.AddHandler(DragDrop.DropEvent, OnDrop);
            Sprite = _sprite;
        }

        private async void OnDrop(object? sender, DragEventArgs e)
        {
            var file = e.Data.GetText();
            if (file != null && file.EndsWith(".tspr"))
            {
                var res = await SpriteResource.Load(file);
                Sprite = res;
            }
        }

        public void OnDragEnter(object? sender, DragEventArgs e)
        {
            //Console.WriteLine("Drag over");
        }

        public void OnOpenClicked(object? sender, RoutedEventArgs e)
        {
        }

        public void OnRemoveClicked(object? sender, RoutedEventArgs e)
        {
            Sprite = null;
        }
        public object? GetPropertyValue()
        {
            return _sprite;
        }
        public event IPropertyEditor.OnPropertyChangedHandler? OnPropertyChanged;
    }
}
