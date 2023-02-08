using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Controls
{

    public class ObjectEditorPropertiesData
    {
        public string Name { get; set; }
        public Type PropType { get; set; }
        public object? Value { get; set; }
        public Control Control { get; set; }
        public ObjectEditorPropertiesData(PropertyInfo info)
        {
            Name = info.Name;
            PropType = info.PropertyType;

            if (PropType == typeof(float))
            {
                var cont = new NumberEditor(Name,0) {
                    Label = Name,
                    Value = 2
                };
                Control = cont;
            }
            else
            {
                Control = new TextBlock() { Text = $"Unsupported data type [{PropType}]" } ;
            }
        }
    }
    public class ObjectEditorComponentData
    {
        public ComponentRegistryData Data { get; set; }
        public ObservableCollection<ObjectEditorPropertiesData> Properties { get; set; }
        public ObjectEditorComponentData(ComponentRegistryData data)
        {
            Data = data;
            Properties = new();

            foreach (var prop in Data.GetProperties())
            {
                Properties.Add(new ObjectEditorPropertiesData(prop));
            }
        }
    }
    public partial class ObjectEditor : UserControl
    {
        public ObservableCollection<ObjectEditorComponentData> Components { get; set; }
        public ObservableCollection<ComponentRegistryData> AvailableComponents { get; set; }

        public ObjectEditor()
        {
            InitializeComponent();
            AvailableComponents = new();
            Components = new();

            foreach (var comp in TundraStudio.ComponentRegistry.Components)
            {
                AvailableComponents.Add(comp.Value);
                Console.WriteLine(comp.Value.Name);
            }
            DataContext = this;
        }

        public void OnComponentMenuDlbClick(object? sender, RoutedEventArgs e)
        {
            AddCompBtn.Flyout.Hide();
            var data = ((e.Source as Control)!.DataContext as ComponentRegistryData)!;
            Components.Add(new ObjectEditorComponentData(data));

        }
    }
}
