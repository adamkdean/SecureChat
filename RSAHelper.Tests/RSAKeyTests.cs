using System;
using NUnit.Framework;
using RSAHelper.Tests;

namespace RSAHelper.Tests
{
    [TestFixture]
    public class RSAKeyTests
    {
        [Test]
        public void ShouldInitialize_GivenNoPublicKey()
        {
            var key = new RSAKey();

            Assert.IsInstanceOf<RSAKey>(key);
            Assert.IsNotNullOrEmpty(key.PrivateKeyXML);
            Assert.IsNotNullOrEmpty(key.PublicKeyXML);
        }

        [Test]
        public void ShouldInitialize_GivenPublicKey()
        {
            var key1 = new RSAKey();
            var key2 = new RSAKey(publicKeyXML: key1.PublicKeyXML);

            Assert.IsInstanceOf<RSAKey>(key2);            
            Assert.IsNotNullOrEmpty(key2.PublicKeyXML);
        }

        [Test]
        public void ShouldEncryptAndDecrypt_UsingSameKey()
        {
            var key = new RSAKey();
            string message = "hello world!";

            string encrypted = key.Encrypt(message);
            string decrypted = key.Decrypt(encrypted);

            Assert.AreEqual(message, decrypted);
        }

        [Test]
        public void ShouldEncryptAndDecrypt_UsingDerivedPublicKey()
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
