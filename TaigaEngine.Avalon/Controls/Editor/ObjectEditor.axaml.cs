using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
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

        public delegate void OnChangedEventHandler();
        public event OnChangedEventHandler OnChanged;

        /// <summary>
        /// Get the content control. The content control must be recreated everytime it's get. Otherwise will do error
        /// </summary>
        /// <returns></returns>
        public Control GetContent()
        {
            Control? editor = null;
            if (PropType == typeof(float) || PropType == typeof(int))
                editor = new NumberEditor(this);
            if (PropType == typeof(SpriteResource))
                editor = new SpritePropEditor(this);
            if (PropType == typeof(bool))
                editor = new BooleanEditor(this);
            if (PropType == typeof(string))
                editor = new StringPropEditor(this);


            // Else it's unknown type
            if (editor == null)
                return new TextBlock()
                {
                    Text = $"Unsupported data type [{PropType}]"
                };

            // Put the on changed event
            (editor as IPropertyEditor)!.OnPropertyChanged += () =>
                OnChanged.Invoke();
            return editor;
        }
        public ObjectEditorPropertiesData(PropertyInfo info)
        {
            Name = info.Name;
            PropType = info.PropertyType;
        }

        public object? GetValue() =>
            (Content as IPropertyEditor)!.GetPropertyValue();

        public void RefreshValue() => Value = GetValue();


    }
    public class ObjectEditorComponentData
    {
        public delegate void OnChangedEvent();
        public event OnChangedEvent OnChanged;

        public ComponentRegistryData Data { get; set; }
        public ObservableCollection<ObjectEditorPropertiesData> Properties { get; set; }
        public string ClassName;
        public ObjectEditorComponentData(ComponentRegistryData data, string className)
        {
            Data = data;
            Properties = new();
            ClassName = className;
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
        public GameObjectResource resource;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ObjectEditor()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();
            Initialize();
        }

        public ObjectEditor(GameObjectResource resource)
        {
            this.resource = resource;
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
                    var comp_data = new ObjectEditorComponentData(comp, c.component);

                    // Component props
                    var props = c.properties;
                    foreach (var pair in props)
                    {
                        var propValue = pair.Value;
                        var prop = pair.Key;
                        if (comp.HasProperty(prop))
                        {
                            if (propValue is string)
                            {
                                if (Guid.TryParse(propValue as string, out var guid))
                                {
                                    // It's a UUID, parse as resource
                                    if (TundraStudio.CurrentProject.ResourceManager.Resources.ContainsKey((string)propValue))
                                    {
                                        var res = TundraStudio.CurrentProject.ResourceManager.Resources[(string)propValue]!;
                                        comp_data.SetPropertyValue(prop, res);
                                        continue; // finish with this prop
                                    }

                                    // Else resource not found
                                    throw new Exception("Resource not found: " + propValue);
                                }
                            }
                            // Else treat as a regular property
                            comp_data.SetPropertyValue(prop, propValue);
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
            Components.Add(new ObjectEditorComponentData(data, data.ComponentType.FullName));
        }

        public async void Save()
        {
            // Updating the resource properties
            foreach (var c in Components)
            {
                foreach (var c2 in resource.components)
                {
                    if (c2.component == c.ClassName)
                    {
                        // Found the match, update the values
                        foreach (var p in c.Properties)
                        {
                            if (p.Value is Resource rs)
                                c2.properties[p.Name] = rs.uuid;
                            else
                                c2.properties[p.Name] = p.Value;
                        }
                        break;
                    }
                }
            }

            // Writing to file
            await resource.SaveToFile();
        }
    }
}
