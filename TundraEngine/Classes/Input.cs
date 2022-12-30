using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TundraEngine.Classes
{
    //
    // Summary:
    //     Represents the keys on a keyboard.
    public enum Key
    {
        //
        // Summary:
        //     An unknown key.
        Unknown = -1,
        //
        // Summary:
        //     The spacebar key.
        Space = 0x20,
        //
        // Summary:
        //     The apostrophe key.
        Apostrophe = 39,
        //
        // Summary:
        //     The comma key.
        Comma = 44,
        //
        // Summary:
        //     The minus key.
        Minus = 45,
        //
        // Summary:
        //     The period key.
        Period = 46,
        //
        // Summary:
        //     The slash key.
        Slash = 47,
        //
        // Summary:
        //     The 0 key.
        Number0 = 48,
        //
        // Summary:
        //     The 0 key; alias for Silk.NET.Input.Key.Number0
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        Number1 = 49,
        //
        // Summary:
        //     The 2 key.
        Number2 = 50,
        //
        // Summary:
        //     The 3 key.
        Number3 = 51,
        //
        // Summary:
        //     The 4 key.
        Number4 = 52,
        //
        // Summary:
        //     The 5 key.
        Number5 = 53,
        //
        // Summary:
        //     The 6 key.
        Number6 = 54,
        //
        // Summary:
        //     The 7 key.
        Number7 = 55,
        //
        // Summary:
        //     The 8 key.
        Number8 = 56,
        //
        // Summary:
        //     The 9 key.
        Number9 = 57,
        //
        // Summary:
        //     The semicolon key.
        Semicolon = 59,
        //
        // Summary:
        //     The equal key.
        Equal = 61,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left bracket(opening bracket) key.
        LeftBracket = 91,
        //
        // Summary:
        //     The backslash.
        BackSlash = 92,
        //
        // Summary:
        //     The right bracket(closing bracket) key.
        RightBracket = 93,
        //
        // Summary:
        //     The grave accent key.
        GraveAccent = 96,
        //
        // Summary:
        //     Non US keyboard layout key 1.
        World1 = 161,
        //
        // Summary:
        //     Non US keyboard layout key 2.
        World2 = 162,
        //
        // Summary:
        //     The escape key.
        Escape = 0x100,
        //
        // Summary:
        //     The enter key.
        Enter = 257,
        //
        // Summary:
        //     The tab key.
        Tab = 258,
        //
        // Summary:
        //     The backspace key.
        Backspace = 259,
        //
        // Summary:
        //     The insert key.
        Insert = 260,
        //
        // Summary:
        //     The delete key.
        Delete = 261,
        //
        // Summary:
        //     The right arrow key.
        Right = 262,
        //
        // Summary:
        //     The left arrow key.
        Left = 263,
        //
        // Summary:
        //     The down arrow key.
        Down = 264,
        //
        // Summary:
        //     The up arrow key.
        Up = 265,
        //
        // Summary:
        //     The page up key.
        PageUp = 266,
        //
        // Summary:
        //     The page down key.
        PageDown = 267,
        //
        // Summary:
        //     The home key.
        Home = 268,
        //
        // Summary:
        //     The end key.
        End = 269,
        //
        // Summary:
        //     The caps lock key.
        CapsLock = 280,
        //
        // Summary:
        //     The scroll lock key.
        ScrollLock = 281,
        //
        // Summary:
        //     The num lock key.
        NumLock = 282,
        //
        // Summary:
        //     The print screen key.
        PrintScreen = 283,
        //
        // Summary:
        //     The pause key.
        Pause = 284,
        //
        // Summary:
        //     The F1 key.
        F1 = 290,
        //
        // Summary:
        //     The F2 key.
        F2 = 291,
        //
        // Summary:
        //     The F3 key.
        F3 = 292,
        //
        // Summary:
        //     The F4 key.
        F4 = 293,
        //
        // Summary:
        //     The F5 key.
        F5 = 294,
        //
        // Summary:
        //     The F6 key.
        F6 = 295,
        //
        // Summary:
        //     The F7 key.
        F7 = 296,
        //
        // Summary:
        //     The F8 key.
        F8 = 297,
        //
        // Summary:
        //     The F9 key.
        F9 = 298,
        //
        // Summary:
        //     The F10 key.
        F10 = 299,
        //
        // Summary:
        //     The F11 key.
        F11 = 300,
        //
        // Summary:
        //     The F12 key.
        F12 = 301,
        //
        // Summary:
        //     The F13 key.
        F13 = 302,
        //
        // Summary:
        //     The F14 key.
        F14 = 303,
        //
        // Summary:
        //     The F15 key.
        F15 = 304,
        //
        // Summary:
        //     The F16 key.
        F16 = 305,
        //
        // Summary:
        //     The F17 key.
        F17 = 306,
        //
        // Summary:
        //     The F18 key.
        F18 = 307,
        //
        // Summary:
        //     The F19 key.
        F19 = 308,
        //
        // Summary:
        //     The F20 key.
        F20 = 309,
        //
        // Summary:
        //     The F21 key.
        F21 = 310,
        //
        // Summary:
        //     The F22 key.
        F22 = 311,
        //
        // Summary:
        //     The F23 key.
        F23 = 312,
        //
        // Summary:
        //     The F24 key.
        F24 = 313,
        //
        // Summary:
        //     The F25 key.
        F25 = 314,
        //
        // Summary:
        //     The 0 key on the key pad.
        Keypad0 = 320,
        //
        // Summary:
        //     The 1 key on the key pad.
        Keypad1 = 321,
        //
        // Summary:
        //     The 2 key on the key pad.
        Keypad2 = 322,
        //
        // Summary:
        //     The 3 key on the key pad.
        Keypad3 = 323,
        //
        // Summary:
        //     The 4 key on the key pad.
        Keypad4 = 324,
        //
        // Summary:
        //     The 5 key on the key pad.
        Keypad5 = 325,
        //
        // Summary:
        //     The 6 key on the key pad.
        Keypad6 = 326,
        //
        // Summary:
        //     The 7 key on the key pad.
        Keypad7 = 327,
        //
        // Summary:
        //     The 8 key on the key pad.
        Keypad8 = 328,
        //
        // Summary:
        //     The 9 key on the key pad.
        Keypad9 = 329,
        //
        // Summary:
        //     The decimal key on the key pad.
        KeypadDecimal = 330,
        //
        // Summary:
        //     The divide key on the key pad.
        KeypadDivide = 331,
        //
        // Summary:
        //     The multiply key on the key pad.
        KeypadMultiply = 332,
        //
        // Summary:
        //     The subtract key on the key pad.
        KeypadSubtract = 333,
        //
        // Summary:
        //     The add key on the key pad.
        KeypadAdd = 334,
        //
        // Summary:
        //     The enter key on the key pad.
        KeypadEnter = 335,
        //
        // Summary:
        //     The equal key on the key pad.
        KeypadEqual = 336,
        //
        // Summary:
        //     The left shift key.
        ShiftLeft = 340,
        //
        // Summary:
        //     The left control key.
        ControlLeft = 341,
        //
        // Summary:
        //     The left alt key.
        AltLeft = 342,
        //
        // Summary:
        //     The left super key.
        SuperLeft = 343,
        //
        // Summary:
        //     The right shift key.
        ShiftRight = 344,
        //
        // Summary:
        //     The right control key.
        ControlRight = 345,
        //
        // Summary:
        //     The right alt key.
        AltRight = 346,
        //
        // Summary:
        //     The right super key.
        SuperRight = 347,
        //
        // Summary:
        //     The menu key.
        Menu = 348
    }

    public class InputManager
    {
        List<Key> pressedKeys;
        public InputManager()
        {
            pressedKeys = new();
        }

        public void Press(Key key)
        {
            pressedKeys.Add(key);
        }
        public void Release(Key key)
        {
            pressedKeys.Remove(key);
        }
        public bool IsKeyPressed(Key key)
        {
            return pressedKeys.Contains(key);
        }
    }
    public static class Input
    {
        static InputManager _input;
        public static InputManager InputManager { 
            get { 
                if(_input == null)
                    _input = new InputManager();
                return _input; 
            } 
        }

        public static bool IsKeyPressed(Key key)
        {
            return InputManager.IsKeyPressed(key);
        }
    }
}
