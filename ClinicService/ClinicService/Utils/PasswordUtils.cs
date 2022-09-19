using ClinicService.Models;
using System.Security.Cryptography;
using System.Text;

namespace ClinicService.Utils
{
    public static  class PasswordUtils
    {
        private const string SecretKey = "Fz8wMguqN2DGWiD1ICvRxQ==";

        public static PasswordHashModel CreatePasswordHash(string password)
        {
            // generate random salt 
            byte[] buffer = new byte[16];
            RNGCryptoServiceProvider secureRandom = new RNGCryptoServiceProvider();
            secureRandom.GetBytes(buffer);

            PasswordHashModel passHash = new PasswordHashModel();

            // create hash 
            passHash.PasswordSalt = Convert.ToBase64String(buffer);
            passHash.PasswordHash = GetPasswordHash(password, passHash.PasswordSalt);

            // done
            return passHash;
        }

        public static bool VerifyPassword(string password, string passwordSalt,
            string passwordHash)
        {
            return GetPasswordHash(password, passwordSalt) == passwordHash;
        }

        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // build password string
            password = $"{password}~{passwordSalt}~{SecretKey}";
            byte[] buffer = Encoding.UTF8.GetBytes(password);

            // compute hash 
            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer);

            // done
            return Convert.ToBase64String(passwordHash);
        }
    }
}
