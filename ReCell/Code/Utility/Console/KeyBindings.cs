namespace Recellection.Code.Utility.Console
{
    using Microsoft.Xna.Framework.Input;

    // binds keys to characters or strings
    struct KeyBinding
    {
        #region Fields

        public string AltString;

        public Keys Key;

        public string ShiftAltString;

        public string ShiftString;

        public string UnmodifiedString;

        #endregion

        #region Constructors and Destructors

        public KeyBinding(Keys key, string unmodifiedString, string shiftString, string altString, string shiftAltString)
        {
            this.Key = key;
            this.UnmodifiedString = unmodifiedString;
            this.ShiftString = shiftString;
            this.AltString = altString;
            this.ShiftAltString = shiftAltString;
        }

        #endregion
    }

    // defines standard character mappings
    class KeyboardHelper
	{
        #region Static Fields

        static public KeyBinding[] AmericanBindings = new[]
        {
            new KeyBinding( Keys.OemPipe, "\\", "|", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemBackslash, "\\", "|", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemOpenBrackets, "[", "{", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemCloseBrackets, "]", "}", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemSemicolon, ";", ":", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemPlus, "=", "+", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemTilde, "`", "~", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemQuotes, "\"", "\"", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemQuestion, "/", "?", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemComma, ", ", "<", string.Empty, string.Empty), 
            new KeyBinding( Keys.OemPeriod, ".", ">", string.Empty, string.Empty), 
            new KeyBinding( Keys.Decimal, ".", string.Empty, string.Empty, string.Empty), 
            new KeyBinding( Keys.OemMinus, "-", "_", string.Empty, string.Empty), 
            new KeyBinding( Keys.Space, " ", string.Empty, string.Empty, string.Empty), 
            new KeyBinding( Keys.Tab, "\t", string.Empty, string.Empty, string.Empty), 
            new KeyBinding( Keys.D1, "1", "!", string.Empty, string.Empty), 
            new KeyBinding( Keys.D2, "2", "@", string.Empty, string.Empty), 
            new KeyBinding( Keys.D3, "3", "#", string.Empty, string.Empty), 
            new KeyBinding( Keys.D4, "4", "$", string.Empty, string.Empty), 
            new KeyBinding( Keys.D5, "5", "%", string.Empty, string.Empty), 
            new KeyBinding( Keys.D6, "6", "^", string.Empty, string.Empty), 
            new KeyBinding( Keys.D7, "7", "&", string.Empty, string.Empty), 
            new KeyBinding( Keys.D8, "8", "*", string.Empty, string.Empty), 
            new KeyBinding( Keys.D9, "9", "(", string.Empty, string.Empty), 
            new KeyBinding( Keys.D0, "0", ")", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad1, "1", "!", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad2, "2", "@", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad3, "3", "#", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad4, "4", "$", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad5, "5", "%", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad6, "6", "^", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad7, "7", "&", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad8, "8", "*", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad9, "9", "(", string.Empty, string.Empty), 
            new KeyBinding( Keys.NumPad0, "0", ")", string.Empty, string.Empty), 
            new KeyBinding( Keys.A, "a", "A", string.Empty, string.Empty), 
            new KeyBinding( Keys.B, "b", "B", string.Empty, string.Empty), 
            new KeyBinding( Keys.C, "c", "C", string.Empty, string.Empty), 
            new KeyBinding( Keys.D, "d", "D", string.Empty, string.Empty), 
            new KeyBinding( Keys.E, "e", "E", string.Empty, string.Empty), 
            new KeyBinding( Keys.F, "f", "F", string.Empty, string.Empty), 
            new KeyBinding( Keys.G, "g", "G", string.Empty, string.Empty), 
            new KeyBinding( Keys.H, "h", "H", string.Empty, string.Empty), 
            new KeyBinding( Keys.I, "i", "I", string.Empty, string.Empty), 
            new KeyBinding( Keys.J, "j", "J", string.Empty, string.Empty), 
            new KeyBinding( Keys.K, "k", "K", string.Empty, string.Empty), 
            new KeyBinding( Keys.L, "l", "L", string.Empty, string.Empty), 
            new KeyBinding( Keys.M, "m", "M", string.Empty, string.Empty), 
            new KeyBinding( Keys.N, "n", "N", string.Empty, string.Empty), 
            new KeyBinding( Keys.O, "o", "O", string.Empty, string.Empty), 
            new KeyBinding( Keys.P, "p", "P", string.Empty, string.Empty), 
            new KeyBinding( Keys.Q, "q", "Q", string.Empty, string.Empty), 
            new KeyBinding( Keys.R, "r", "R", string.Empty, string.Empty), 
            new KeyBinding( Keys.S, "s", "S", string.Empty, string.Empty), 
            new KeyBinding( Keys.T, "t", "T", string.Empty, string.Empty), 
            new KeyBinding( Keys.U, "u", "U", string.Empty, string.Empty), 
            new KeyBinding( Keys.V, "v", "V", string.Empty, string.Empty), 
            new KeyBinding( Keys.W, "w", "W", string.Empty, string.Empty), 
            new KeyBinding( Keys.X, "x", "X", string.Empty, string.Empty), 
            new KeyBinding( Keys.Y, "y", "Y", string.Empty, string.Empty), 
            new KeyBinding( Keys.Z, "z", "Z", string.Empty, string.Empty)
        };

        static public KeyBinding[] ItalianBindings = new[]
            {
                new KeyBinding( Keys.OemPipe, "\\", "|", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemBackslash, "<", ">", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemOpenBrackets, "\"", "?", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemCloseBrackets, "ì", "^", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemSemicolon, "è", "é", "[", "{"), 
                new KeyBinding( Keys.OemPlus, "+", "*", "]", "}"), 
                new KeyBinding( Keys.OemTilde, "ò", "ç", "@", string.Empty), 
                new KeyBinding( Keys.OemQuotes, "à", "°", "#", string.Empty), 
                new KeyBinding( Keys.OemQuestion, "ù", "§", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemComma, ", ", ";", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemPeriod, ".", ":", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemMinus, "-", "_", string.Empty, string.Empty), 
                new KeyBinding( Keys.Space, " ", string.Empty, string.Empty, string.Empty),            
                new KeyBinding( Keys.Tab, "\t", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.D1, "1", "!", string.Empty, string.Empty), 
                new KeyBinding( Keys.D2, "2", "@", string.Empty, string.Empty), 
                new KeyBinding( Keys.D3, "3", "#", string.Empty, string.Empty), 
                new KeyBinding( Keys.D4, "4", "$", string.Empty, string.Empty), 
                new KeyBinding( Keys.D5, "5", "%", string.Empty, string.Empty), 
                new KeyBinding( Keys.D6, "6", "^", string.Empty, string.Empty), 
                new KeyBinding( Keys.D7, "7", "&", string.Empty, string.Empty), 
                new KeyBinding( Keys.D8, "8", "*", string.Empty, string.Empty), 
                new KeyBinding( Keys.D9, "9", "(", string.Empty, string.Empty), 
                new KeyBinding( Keys.D0, "0", ")", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad1, "1", "!", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad2, "2", "\"", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad3, "3", "£", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad4, "4", "$", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad5, "5", "%", "€", string.Empty), 
                new KeyBinding( Keys.NumPad6, "6", "&", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad7, "7", "/", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad8, "8", "(", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad9, "9", ")", string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad0, "0", "=", string.Empty, string.Empty), 
                new KeyBinding( Keys.A, "a", "A", string.Empty, string.Empty), 
                new KeyBinding( Keys.B, "b", "B", string.Empty, string.Empty), 
                new KeyBinding( Keys.C, "c", "C", string.Empty, string.Empty), 
                new KeyBinding( Keys.D, "d", "D", string.Empty, string.Empty), 
                new KeyBinding( Keys.E, "e", "E", "€", string.Empty), 
                new KeyBinding( Keys.F, "f", "F", string.Empty, string.Empty), 
                new KeyBinding( Keys.G, "g", "G", string.Empty, string.Empty), 
                new KeyBinding( Keys.H, "h", "H", string.Empty, string.Empty), 
                new KeyBinding( Keys.I, "i", "I", string.Empty, string.Empty), 
                new KeyBinding( Keys.J, "j", "J", string.Empty, string.Empty), 
                new KeyBinding( Keys.K, "k", "K", string.Empty, string.Empty), 
                new KeyBinding( Keys.L, "l", "L", string.Empty, string.Empty), 
                new KeyBinding( Keys.M, "m", "M", string.Empty, string.Empty), 
                new KeyBinding( Keys.N, "n", "N", string.Empty, string.Empty), 
                new KeyBinding( Keys.O, "o", "O", string.Empty, string.Empty), 
                new KeyBinding( Keys.P, "p", "P", string.Empty, string.Empty), 
                new KeyBinding( Keys.Q, "q", "Q", string.Empty, string.Empty), 
                new KeyBinding( Keys.R, "r", "R", string.Empty, string.Empty), 
                new KeyBinding( Keys.S, "s", "S", string.Empty, string.Empty), 
                new KeyBinding( Keys.T, "t", "T", string.Empty, string.Empty), 
                new KeyBinding( Keys.U, "u", "U", string.Empty, string.Empty), 
                new KeyBinding( Keys.V, "v", "V", string.Empty, string.Empty), 
                new KeyBinding( Keys.W, "w", "W", string.Empty, string.Empty), 
                new KeyBinding( Keys.X, "x", "X", string.Empty, string.Empty), 
                new KeyBinding( Keys.Y, "y", "Y", string.Empty, string.Empty), 
                new KeyBinding( Keys.Z, "z", "Z", string.Empty, string.Empty)
            };

        static public KeyBinding[] SwedishBindings = new[]
            {
                new KeyBinding( Keys.OemComma, ", ", ";", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemPeriod, ".", ":", string.Empty, string.Empty), 
                new KeyBinding( Keys.Decimal, ".", ":", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemMinus, "-", "_", string.Empty, string.Empty), 
                new KeyBinding( Keys.Space, " ", " ", " ", " "), 
                new KeyBinding( Keys.Tab, "\t", string.Empty, string.Empty, string.Empty), 
            
                new KeyBinding( Keys.D1, "1", "!", string.Empty, "@"), 
                new KeyBinding( Keys.D2, "2", "\"", string.Empty, "£"), 
                new KeyBinding( Keys.D3, "3", "#", string.Empty, "$"), 
                new KeyBinding( Keys.D4, "4", "¤", string.Empty, string.Empty), 
                new KeyBinding( Keys.D5, "5", "%", string.Empty, string.Empty), 
                new KeyBinding( Keys.D6, "6", "&", string.Empty, string.Empty), 
                new KeyBinding( Keys.D7, "7", "/", string.Empty, "{"), 
                new KeyBinding( Keys.D8, "8", "(", string.Empty, "["), 
                new KeyBinding( Keys.D9, "9", ")", string.Empty, "]"), 
                new KeyBinding( Keys.D0, "0", "=", string.Empty, "}"), 
                new KeyBinding( Keys.OemPlus, "+", "?", "\\", string.Empty), 
            
                new KeyBinding( Keys.NumPad1, "1", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad2, "2", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad3, "3", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad4, "4", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad5, "5", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad6, "6", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad7, "7", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad8, "8", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad9, "9", string.Empty, string.Empty, string.Empty), 
                new KeyBinding( Keys.NumPad0, "0", string.Empty, string.Empty, string.Empty), 
            
                new KeyBinding( Keys.A, "a", "A", string.Empty, string.Empty), 
                new KeyBinding( Keys.B, "b", "B", string.Empty, string.Empty), 
                new KeyBinding( Keys.C, "c", "C", string.Empty, string.Empty), 
                new KeyBinding( Keys.D, "d", "D", string.Empty, string.Empty), 
                new KeyBinding( Keys.E, "e", "E", string.Empty, string.Empty), 
                new KeyBinding( Keys.F, "f", "F", string.Empty, string.Empty), 
                new KeyBinding( Keys.G, "g", "G", string.Empty, string.Empty), 
                new KeyBinding( Keys.H, "h", "H", string.Empty, string.Empty), 
                new KeyBinding( Keys.I, "i", "I", string.Empty, string.Empty), 
                new KeyBinding( Keys.J, "j", "J", string.Empty, string.Empty), 
                new KeyBinding( Keys.K, "k", "K", string.Empty, string.Empty), 
                new KeyBinding( Keys.L, "l", "L", string.Empty, string.Empty), 
                new KeyBinding( Keys.M, "m", "M", string.Empty, string.Empty), 
                new KeyBinding( Keys.N, "n", "N", string.Empty, string.Empty), 
                new KeyBinding( Keys.O, "o", "O", string.Empty, string.Empty), 
                new KeyBinding( Keys.P, "p", "P", string.Empty, string.Empty), 
                new KeyBinding( Keys.Q, "q", "Q", string.Empty, string.Empty), 
                new KeyBinding( Keys.R, "r", "R", string.Empty, string.Empty), 
                new KeyBinding( Keys.S, "s", "S", string.Empty, string.Empty), 
                new KeyBinding( Keys.T, "t", "T", string.Empty, string.Empty), 
                new KeyBinding( Keys.U, "u", "U", string.Empty, string.Empty), 
                new KeyBinding( Keys.V, "v", "V", string.Empty, string.Empty), 
                new KeyBinding( Keys.W, "w", "W", string.Empty, string.Empty), 
                new KeyBinding( Keys.X, "x", "X", string.Empty, string.Empty), 
                new KeyBinding( Keys.Y, "y", "Y", string.Empty, string.Empty), 
                new KeyBinding( Keys.Z, "z", "Z", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemSemicolon, "¨", "^", string.Empty, "~"), 
                new KeyBinding( Keys.OemCloseBrackets, "å", "Å", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemQuotes, "ä", "Ä", string.Empty, string.Empty), 
                new KeyBinding( Keys.OemBackslash, "<", ">", string.Empty, "|"), 
                new KeyBinding( Keys.OemTilde, "ö", "Ö", string.Empty, string.Empty)
            };

        #endregion
	}
}
