using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TundraEngine.Classes.Data;
using TundraEngine.Studio.Util;

namespace TundraEngine.Studio.Controls
{

    public class ObjectEditorPropertiesData
    {
        public string Name { get; set; }
        public Type PropType { get; set; }
        public dynamic? Value { get; set; }
        public Control Content { get => GetContent(); }

        /// <summary>
        /// Get the content control. The content control must be recreated everytime it's get. Otherwise will do error
        /// </summary>
        /// <returns></returns>
        public Control GetContent()
        {
            if (PropType == typeof(float) || PropType == typeof(int))
                return new NumberEditor(this);
            if (PropType == typeof(SpriteResource))
                return new SpritePropEditor(this);
            if (PropType == typeof(bool))
                return new BooleanEditor(this);
            if (PropType == typeof(string))
                return new StringPropEditor(this);
            
            return new TextBlock() { Text = $"Unsupported data type [{PropType}]" };
        }
        public ObjectEditorPropertiesData(PropertyInfo info)
        {
            Name = info.Name;
            PropType = info.PropertyType;
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

            foreach (var prop in Data.Properties)
            {
                Properties.Add(new ObjectEditorPropertiesData(prop));
            }
        }

        public void SetPropertyValue(string name, object value)
        {
            foreach (var prop in Properties)
            {
                if (prop.Name == name)
                {
                    prop.Value = value;
                    return;
                }
            }
            throw new Exception($"Failed to set property, property not found: {name}");
        }
    }
    public partial class ObjectEditor : UserControl
    {
        public ObservableCollection<ObjectEditorComponentData> Components { get; set; }
        public ObservableCollection<ComponentRegistryData> AvailableComponents { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ObjectEditor()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();
            Initialize();
        }

        public ObjectEditor(GameObjectResource resource)
        {

            Initialize();
            InitializeComponent();
            TbObjectName.Text = resource.name;
            // Initialize Components
            foreach (var c in resource.components)
            {
                try
                {
                    var comp_class = c.component;
                    // Component class
                    var comp = TundraStudio.ComponentRegistry.GetComponent(comp_class);
                    var comp_data = new ObjectEditorComponentData(comp);

                    // Component props
                    var props = c.properties;
                    foreach (var pair in props)
                    {
                        var uuid = pair.Value;
                        var prop = pair.Key;
                        if (comp.HasProperty(prop))
                        {
                            if (TundraStudio.CurrentProject.ResourceManager.Resources.ContainsKey(uuid))
                            {
                                var res = TundraStudio.CurrentProject.ResourceManager.Resources[uuid]!;
                                comp_data.SetPropertyValue(prop, res);
                            }
                            else
                            {
                                throw new Exception($"Resource not found {uuid}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Unknown property: {prop}");
                        }
                    }

                    // Add to components list
                    Components!.Add(comp_data);
                }
                catch (Exception e)
                {
                    //TODO: do better error handling
                    Console.WriteLine(e);
                    continue;
                }

            }
        }

        public void Initialize()
        {
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
