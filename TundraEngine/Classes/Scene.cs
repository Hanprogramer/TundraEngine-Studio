using TundraEngine.Rendering;

namespace TundraEngine.Classes
{
    public class Scene
    {
        List<GameObject> objects;
        public IGameWindow GameWindow;
        public Scene(IGameWindow window)
        {
            objects = new();
            GameWindow = window;
        }

        public void Initialize()
        {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Initialize();
            }
        }

        public void Update(float dt)
        {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Update(dt);
            }
        }
        public void Render(Renderer renderer)
        {
            for (var i = 0; i < objects.Count; i++)
            {
                objects[i].Render(renderer);
            }
        }

        public GameObject AddObject(GameObject obj)
        {
            objects.Add(obj);
            return obj;
        }

        public void RemoveObject(GameObject obj)
        {
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

        public T FindObject<T>() where T : GameObject
        {
            for (var i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetType() == typeof(T))
                    return (T)objects[i];
            }
            throw new Exception("Object of type " + typeof(T).Name + " not found in the scene");
        }
    }
}