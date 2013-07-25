using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecureChat
{
    public partial class frmSingleKeyTest : Form
    {
        private RSACryptoServiceProvider rsaPrivate, rsaPublic;
        private string rsaPrivateXml, rsaPublicXml;

        public frmSingleKeyTest()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateKeys();

            lblPrivate.Text = rsaPrivateXml;
            lblPublic.Text = rsaPublicXml;
            btnGenerate.Enabled = false;
            btnEncrypt.Enabled = true;
            btnDecrypt.Enabled = true;
            txtEncrypt.Enabled = true;
            txtDecrypt.Enabled = true;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string result = Encrypt(txtEncrypt.Text);
            txtEncrypt.Text = result;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string result = Decrypt(txtDecrypt.Text);
            txtDecrypt.Text = result;
        }

        private void GenerateKeys()
        {            
            RSACryptoServiceProvider rsaGenKeys = new RSACryptoServiceProvider();
            rsaPrivateXml = rsaGenKeys.ToXmlString(true);
            rsaPublicXml = rsaGenKeys.ToXmlString(false);
        }
        
        private string Encrypt(string text)
        {
            if (rsaPublic == null)
            {
                rsaPublic = new RSACryptoServiceProvider();
                rsaPublic.FromXmlString(rsaPublicXml);
            }

            byte[] toEncryptData = Encoding.UTF8.GetBytes(text);
            byte[] encryptedRSA = rsaPublic.Encrypt(toEncryptData, false);
            
            return Convert.ToBase64String(encryptedRSA);
        }

        private string Decrypt(string text)
        {
            if (rsaPrivate == null)
            {
                rsaPrivate = new RSACryptoServiceProvider();
                rsaPrivate.FromXmlString(rsaPrivateXml);
            }

            byte[] encryptedRSA = Convert.FromBase64String(text);
            byte[] decryptedRSA = rsaPrivate.Decrypt(encryptedRSA, false);

            return Encoding.UTF8.GetString(decryptedRSA);
        }
    }
}