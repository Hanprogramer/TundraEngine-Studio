using System;
using System.Collections.Generic;
using System.Reflection;
using TundraEngine.Components;
using Transform = TundraEngine.Components.Transform;

namespace TundraEngine.Studio.Util
{
    public class ComponentRegistryData
    {
        public string Name { get; set; }
        public Type ComponentType { get; set; }

        public ComponentRegistryData(Type t) 
        {
            Name = t.Name;
            ComponentType = t;
        }

        public PropertyInfo[] GetProperties()
        {
            return ComponentType.GetProperties();
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
        }

        public void Register<T>() where T : Component
        {
            var comp = new ComponentRegistryData(typeof(T));
            Components[typeof(T).Name] = comp;
            //Console.WriteLine("Registering " + comp.Name);
            //foreach (var c in comp.GetProperties())
            //{
            //    Console.WriteLine("Prop: " + c.Name);
            //}
        }
    }


}
