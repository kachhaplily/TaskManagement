using System.Security.Cryptography;
namespace TaskManagement.Helper
{
    public class PasswordHasher
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 16;
        private static readonly int Iterration = 10000;
        public static string HashPassword(string password)
        {
            byte[] salt;
            rng.GetBytes(salt=new byte[SaltSize]);
            var Key = new Rfc2898DeriveBytes(password, salt, Iterration);
            var Hash = Key.GetBytes(HashSize);

            var HashByte = new Byte[SaltSize + HashSize];
            Array.Copy(salt,0,HashByte,0,SaltSize); ;
            Array.Copy(Hash,0,HashByte,SaltSize,HashSize);
            

            var base64Hash = Convert.ToBase64String(HashByte);
            return base64Hash;
    }

       public static bool Verifypassword(string password,string base64Hash)
        {
            var hashbtye=Convert.FromBase64String(base64Hash);
            var salt = new byte[SaltSize];
            Array.Copy(hashbtye,0,salt,0,SaltSize);
            var key = new Rfc2898DeriveBytes(password, salt, Iterration);
            byte[] hash = key.GetBytes(HashSize);

            for(var i =0;i< HashSize;i++)
            {
                if (hashbtye[i+SaltSize] != hash[i])
                    return false;
            }
            return true;

        }

    }

  


}
