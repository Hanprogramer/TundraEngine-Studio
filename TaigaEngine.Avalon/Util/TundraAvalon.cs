using System;

namespace TundraEngine.Studio.Util
{
    /// <summary>
    /// Helper class to communicate AvalonaUI and Tundra
    /// </summary>
    public static class TundraAvalon
    {
        /// <summary>
        /// Translates Avalonia Key class to Tundra/Silk.NET key
        /// </summary>
        /// <param name="key">Avalona Key</param>
        /// <returns>Tundra/Silk.NET key</returns>
        public static Classes.Key? TranslateAvaloniaKeys(Avalonia.Input.Key key)
        {
            string name = Enum.GetName(typeof(Avalonia.Input.Key), key)!;
            if (Enum.TryParse<Classes.Key>(name, false, out var result))
                return result;
            else
            {
                Console.WriteLine("Key can't be translated: " + name);
                return null;
            }
        }
    }
}
