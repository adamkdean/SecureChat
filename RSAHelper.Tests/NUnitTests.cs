using System;
using NUnit.Framework;
using RSAHelper.Tests;

namespace RSAHelper.Tests
{
    [TestFixture]
    public class NUnitTests
    {
        [Test]
        public void RSAKey_InitializesOK()
        {
            var key = new RSAKey();
            
            Assert.IsInstanceOf<RSAKey>(key);
            Assert.IsNotNullOrEmpty(key.PrivateKeyXML);
            Assert.IsNotNullOrEmpty(key.PublicKeyXML);
        }

        [Test]
        public void RSAKey_SameKeyEncryptionWorks()
        {
            var key = new RSAKey();
            string message = "hello world!";

            string encrypted = key.Encrypt(message);
            string decrypted = key.Decrypt(encrypted);

            Assert.AreEqual(message, decrypted);
        }

        [Test]
        public void RSAKey_DifferentKeyEncryptionWorks()
        {
            var originalKey = new RSAKey();
            var publicKey = new RSAKey(publicKeyXML: originalKey.PublicKeyXML);
            string message = "hello world!";

            string encrypted = publicKey.Encrypt(message);
            string decrypted = originalKey.Decrypt(encrypted);

            Assert.AreEqual(message, decrypted);
        }
    }
}
