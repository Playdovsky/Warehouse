using System;
using System.Text;

namespace Main.Logic
{
    /// <summary>
    /// This class serves as random password generator.
    /// </summary>
    public class PasswordGenerator
    {
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string SpecialCharacters = "-_!*#$&";

        public static string GeneratePassword()
        {
            Random random = new Random();

            StringBuilder passwordBuilder = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                int index = random.Next(UppercaseLetters.Length);
                passwordBuilder.Append(UppercaseLetters[index]);
            }

            for (int i = 0; i < 3; i++)
            {
                int index = random.Next(LowercaseLetters.Length);
                passwordBuilder.Append(LowercaseLetters[index]);
            }

            for (int i = 0; i < 2; i++)
            {
                int index = random.Next(Digits.Length);
                passwordBuilder.Append(Digits[index]);
            }

            for (int i = 0; i < 2; i++)
            {
                int index = random.Next(SpecialCharacters.Length);
                passwordBuilder.Append(SpecialCharacters[index]);
            }

            string password = passwordBuilder.ToString();
            password = Shuffle(password);

            return password;
        }

        private static string Shuffle(string input)
        {
            char[] characters = input.ToCharArray();
            Random random = new Random();
            for (int i = characters.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                char temp = characters[i];
                characters[i] = characters[j];
                characters[j] = temp;
            }
            return new string(characters);
        }
    }
}

