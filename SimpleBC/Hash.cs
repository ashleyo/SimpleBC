using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SimpleBC
{
    public static class Hash
    {
        // SHA1 is regarded as insecure against a well-resourced attack
        // In a non-demo application use SHA-3 (512 bit) instead
        public static HashAlgorithm CSP { get; set; } = new SHA1CryptoServiceProvider();

        public static int HASHLEN => CSP.HashSize >> 2; 
        
        public static string HashZero => new string('0', HASHLEN);

        /// <summary>
        /// Takes a number of string parameters or an array of strings and returns the string
        /// representing their hash. Algorithm used defaults to SHA1 but can be altered by 
        /// writing to the CSP (CryptServiceProvider) property of the class.
        /// </summary>
        /// <param name="strings">The strings to hash They are run together without separators,
        /// so ("Hello", "World") and ("HelloWorld") should hash identically.</param>
        /// <returns></returns>
        public static string HashGen(params string[] strings)
        {
            //UnicodeEncoding enc = new UnicodeEncoding();
            //HASHLEN = CSP.HashSize >> 2; // number of bits / 8 * 2 == no of chars;
            byte[] hash = new byte[CSP.HashSize >> 4];
            //create expandable stream (for final application a fixed size could be used for efficiency
            // and recycled) 
            using (MemoryStream memStream = new MemoryStream())
            {
                //Get params, convert each to byte array writing them into the stream
                foreach (string s in strings)
                {
                    byte[] dataArray = (new UnicodeEncoding()).GetBytes(s);
                    memStream.Write(dataArray, 0, dataArray.Length);
                }
                memStream.Seek(0, SeekOrigin.Begin);
                hash = CSP.ComputeHash(memStream);
            } //using

            StringBuilder builder = new StringBuilder(HASHLEN);
            foreach (byte b in hash) builder.AppendFormat("{0:x2}", b);
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        } //main
    }
}

