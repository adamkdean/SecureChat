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
        private const bool USE_OAEP_PADDING = true;

        private Encoding encoding = Encoding.UTF8;
        private RSACryptoServiceProvider privateCrypto;
        private RSACryptoServiceProvider publicCrypto;

        public HashAlgorithm SignatureHashAlgorithm { get; private set; }
        public string PrivateKeyXML { get; private set; }
        public string PublicKeyXML { get; private set; }

        public RSAKey(int dwKeySize = 1024, string publicKeyXML = null, HashAlgorithm signatureHashAlgorithm = null)
        {
            if (signatureHashAlgorithm != null) SignatureHashAlgorithm = signatureHashAlgorithm;
            else SignatureHashAlgorithm = new SHA512CryptoServiceProvider();

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
            privateCrypto = new RSACryptoServiceProvider(dwKeySize);
            privateCrypto.FromXmlString(privateKeyXML);
            PrivateKeyXML = privateKeyXML;
        }

        private void InitializePublicKey(int dwKeySize, string publicKeyXML)
        {
            publicCrypto = new RSACryptoServiceProvider(dwKeySize);
            publicCrypto.FromXmlString(publicKeyXML);
            PublicKeyXML = publicKeyXML;
        }

        public string Encrypt(string plaintext)
        {
            if (publicCrypto == null)
            {
                throw new PublicKeyNotSetException();                
            }

            byte[] plaintextBytes = encoding.GetBytes(plaintext);
            byte[] encryptedBytes = publicCrypto.Encrypt(plaintextBytes, USE_OAEP_PADDING);
            string base64text = Convert.ToBase64String(encryptedBytes);
            
            return base64text;
        }

        public string Decrypt(string encryptedBase64text)
        {
            if (privateCrypto == null)
            {
                throw new PrivateKeyNotSetException();
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64text);
            byte[] decryptedBytes = privateCrypto.Decrypt(encryptedBytes, USE_OAEP_PADDING);
            string decryptedText = encoding.GetString(decryptedBytes);

            return decryptedText;
        }

        public string Sign(string text)
        {
            if (privateCrypto == null)
            {
                throw new PrivateKeyNotSetException();
            }

            byte[] originalData = encoding.GetBytes(text);
            byte[] signatureBytes = privateCrypto.SignData(originalData, new SHA512CryptoServiceProvider());

            return Convert.ToBase64String(signatureBytes);
        }

        public bool Verify(string textToVerify, string signatureBase64text)
        {
            if (publicCrypto == null)
            {
                throw new PublicKeyNotSetException();
            }

            byte[] bytesToVerify = encoding.GetBytes(textToVerify);
            byte[] signatureBytes = Convert.FromBase64String(signatureBase64text);
            bool success = publicCrypto.VerifyData(bytesToVerify, new SHA512CryptoServiceProvider(), signatureBytes);

            return success;
        }
    }
}
