﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraEngine.Rendering;

namespace TundraEngine.Classes
{
    public class Scene
    {
        List<GameObject> objects;
        public IGameWindow GameWindow;
        Renderer Renderer { get => GameWindow.Renderer; }
        public Scene(IGameWindow window) {
            objects = new();
            GameWindow = window;
        }

        public void Initialize()
        {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Load();
            }
        }

        public void Update(float dt) {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Update(dt);
            }
        }
        public void Render()
        {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Render(Renderer);
            }
        }

        public GameObject AddObject(GameObject obj) {
            objects.Add(obj);
            return obj;
        }

        public void RemoveObject(GameObject obj) {
            objects.Remove(obj);
        }

        public GameObject? FindObjectByID(string ID)
        {
            for (var i = 0; i < objects.Count; i++)
            {
                if (objects[i].ID == ID)
                    return objects[i];
            }
            return null;
        }
    }
}
