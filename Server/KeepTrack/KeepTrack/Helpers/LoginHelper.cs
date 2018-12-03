using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KeepTrack.Helpers
{
    public class LoginHelper
    {
        private const int MaxSaltBytes = 10;
        private const int MinSaltBytes = 1;
        private const int ExpectedHashLengthInBytes = 64;

        public static async Task<string> GenerateHash(string input)
        {
            // Generate salt
            var rnd = new Random();
            var saltByteCount = rnd.Next(MinSaltBytes, MaxSaltBytes);
            var cryptoGen = new RNGCryptoServiceProvider();
            var saltBytes = new byte[saltByteCount];
            cryptoGen.GetNonZeroBytes(saltBytes);
            return await GenerateHash(input, saltBytes);
        }

        public static async Task<bool> VerifyHash(string input, string hash)
        {
            // Generate salt
            var hashBytes = Convert.FromBase64String(hash);
            var saltByteCount = hashBytes.Length - ExpectedHashLengthInBytes;
            using (var ms = new MemoryStream())
            {
                await ms.WriteAsync(hashBytes, 0, saltByteCount);
                var saltArr = ms.ToArray();
                var newHash = await GenerateHash(input, saltArr);
                return string.Equals(newHash, hash, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static async Task<string> GenerateHash(string input, byte[] salt)
        {
            // Add the salt and compute the hash
            var hashManager = new SHA512Managed();
            var encodedBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash;
            using (var ms = new MemoryStream())
            {
                ms.SetLength(salt.Length + encodedBytes.Length);
                await ms.WriteAsync(salt, 0, salt.Length);
                await ms.WriteAsync(encodedBytes, 0, encodedBytes.Length);
                hash = hashManager.ComputeHash(ms.ToArray());
                if (hash.Length != ExpectedHashLengthInBytes)
                {
                    throw new Exception("GenerateHash::Unexpected hash size");
                }
            }

            // Add the salt for later
            using (var ms = new MemoryStream())
            {
                ms.SetLength(salt.Length + hash.Length);
                await ms.WriteAsync(salt, 0, salt.Length);
                await ms.WriteAsync(hash, 0, hash.Length);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}