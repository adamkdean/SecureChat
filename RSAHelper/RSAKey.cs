using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSAHelper
{
    public class RSAKey
    {
        private const bool USE_OAEP_PADDING = false;

        private Encoding encoding = Encoding.UTF8;
        private RSACryptoServiceProvider privateRSACryptoServiceProvider;
        private RSACryptoServiceProvider publicRSACryptoServiceProvider;

        public string PrivateKeyXML { get; private set; }
        public string PublicKeyXML { get; private set; }

        public RSAKey(int dwKeySize = 1024, string publicKeyXML = null)
        {
            if (publicKeyXML != null)
            {
                InitializePublicKey(dwKeySize, publicKeyXML);
            }
            else
            {
                using (var rsaGenKeys = new RSACryptoServiceProvider(dwKeySize))
                {
                    InitializePrivateKey(dwKeySize, rsaGenKeys.ToXmlString(true));
                    InitializePublicKey(dwKeySize, rsaGenKeys.ToXmlString(false));                    
                }
            }
        }

        private void InitializePrivateKey(int dwKeySize, string privateKeyXML)
        {
            privateRSACryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            privateRSACryptoServiceProvider.FromXmlString(privateKeyXML);
            PrivateKeyXML = privateKeyXML;
        }

        private void InitializePublicKey(int dwKeySize, string publicKeyXML)
        {
            publicRSACryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            publicRSACryptoServiceProvider.FromXmlString(publicKeyXML);
            PublicKeyXML = publicKeyXML;
        }

        public string Encrypt(string plaintext)
        {
            if (publicRSACryptoServiceProvider == null)
            {
                throw new PublicKeyNotSetException();                
            }

            byte[] plaintextBytes = encoding.GetBytes(plaintext);
            byte[] encryptedBytes = publicRSACryptoServiceProvider.Encrypt(plaintextBytes, USE_OAEP_PADDING);
            string base64text = Convert.ToBase64String(encryptedBytes);

            return base64text;
        }

        public string Decrypt(string encryptedBase64text)
        {
            if (privateRSACryptoServiceProvider == null)
            {
                throw new PrivateKeyNotSetException();
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64text);
            byte[] decryptedBytes = privateRSACryptoServiceProvider.Decrypt(encryptedBytes, USE_OAEP_PADDING);
            string decryptedText = encoding.GetString(decryptedBytes);

            return decryptedText;
        }
    }
}
