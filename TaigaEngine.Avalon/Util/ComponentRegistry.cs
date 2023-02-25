using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TundraEngine.Components;
using Transform = TundraEngine.Components.Transform;

namespace TundraEngine.Studio.Util
{
    public class ComponentRegistryData
    {
        public string Name { get; set; }
        public Type ComponentType { get; set; }

        private PropertyInfo[]? _properties;
        public PropertyInfo[] Properties
        {
            get { 
                if (_properties == null)
                    _properties = ComponentType.GetProperties();
                return _properties; 
            }
        }

        public ComponentRegistryData(Type t)
        {
            Name = t.Name;
            ComponentType = t;
        }

        public bool HasProperty<T>(string name)
        {
            foreach (var prop in Properties)
            {
                if(prop.Name == name && prop.PropertyType == typeof(T)) return true;
            }
            throw new Exception($"Component {Name} doesn't have property of name {name} of type {typeof(T).FullName}");
        }
        public bool HasProperty(string name)
        {
            foreach (var prop in Properties)
            {
                if (prop.Name == name) return true;
            }
            throw new Exception($"Component {Name} doesn't have property of name {name}");
        }
    }
    public class ComponentRegistry
    {
        public Dictionary<string, ComponentRegistryData> Components;
        public ComponentRegistry()
        {
            Components = new();

            Register<Transform>();
            Register<SpriteRenderer>();
            Register<TestComponent>();
        }

        public void Register<T>() where T : Component
        {
            var comp = new ComponentRegistryData(typeof(T));
            Components[typeof(T).FullName] = comp;
            //Console.WriteLine("Registering " + comp.Name);
            //foreach (var c in comp.GetProperties())
            //{
            //    Console.WriteLine("Prop: " + c.Name);
            //}
        }

        public void Register(Type t)
        {
            var comp = new ComponentRegistryData(t);
            Components[t.FullName] = comp;
        }

        /// <summary>
        /// Gets a component from the registry
        /// </summary>
        /// <param name="name">Component's class name</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ComponentRegistryData GetComponent(string name)
        {
            if (Components.ContainsKey(name))
                return Components[name];
            throw new Exception("Component class not found: " + name);
        }
    }


}
