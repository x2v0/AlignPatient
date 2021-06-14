// $Id: $
// Author: Valeriy Onuchin   29.12.2011

using System;
using System.Text;
using System.Security.Cryptography;

namespace P
{
    public class PasswordHasher
    {
        private static PasswordHasher instance = new PasswordHasher();

        private readonly HashAlgorithm hashAlgorithm = SHA256.Create();
        private readonly Encoding textEncoder = Encoding.UTF8;

        private static readonly String randomJunk = "But as soon as he opened his mouth to speak, the piece of bread fell on the ground below. The clever fox immediately picked up the bread";

        // singleton
        private PasswordHasher() { }

        public static PasswordHasher getInstance()
        {
            return instance;
        }

        public byte[] HashPassword(String password)
        {
            if (password == null || password.Length == 0) {
                return null;
            }

            return hashAlgorithm.ComputeHash(textEncoder.GetBytes(password + randomJunk));
        }
    }
}
