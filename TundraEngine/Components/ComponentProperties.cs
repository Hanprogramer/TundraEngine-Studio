using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TundraEngine.Components
{
    public class ComponentProperties
    {
        Dictionary<string, dynamic> Values;
        public ComponentProperties() {
            Values = new();
        }

        /// <summary>
        /// Get a value 
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="name">Name of the property</param>
        /// <returns>Value of the property</returns>
        /// <exception cref="Exception">When the value is not found</exception>
        public T GetValue<T>(string name) {
            Values.TryGetValue(name, out var value);
            if (value == null)
                throw new Exception("Can't find component property: " + name);
            return value;
        }

        /// <summary>
        /// Adds a new property to the tabel
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">Value of the property</param>
        public void Put(string name, dynamic value)
        {
            Values.Add(name, value);
        }
    }
}
