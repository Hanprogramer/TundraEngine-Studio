﻿using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;
using TundraEngine.Components;
using Component = TundraEngine.Components.Component;

namespace TundraEngine.Classes.Data
{

    

    public class GameObjectResource : Resource
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public string description { get; set; }
        public GameComponentResource[] components { get; set; }
        public object[] children { get; set; }

        public GameObjectResource(string uuid, string name, string description, GameComponentResource[] components, GameObjectResource[] children) : base(uuid, ResourceType.Object)
        {
            this.uuid = uuid;
            this.name = name;
            this.description = description;
            this.components = components;
            this.children = children;
        }

        public static async Task<GameObjectResource> Load(string path)
        {
            return await Load<GameObjectResource>(path, ".tobj");
        }

        /// <summary>
        /// Create an instance of this game object resource
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GameObject Instantiate(Scene scene, ResourceManager rm)
        {
            var obj = new GameObject(scene);
            foreach (var comp in components)
            {
                var component = comp.Instantiate(obj, rm);
                obj.AddComponent(component);
            }
            
            // TODO: implement children
            throw new Exception("Failed to instantiate this object");
        }
    }



}
