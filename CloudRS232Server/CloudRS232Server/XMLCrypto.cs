using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace CloudRS232Server
{
    public class XMLCrypto
    {
        /// <summary>
        /// 密钥矢量
        /// </summary>
        public static Byte[] CryptoIV = new byte[16] { 52, 136, 58, 233, 59, 192, 169, 146, 
                                                       179, 41, 230, 209, 188, 205, 15, 147 };
        /// <summary>
        /// 密钥
        /// </summary>
        public static Byte[] CryptoKey = new byte[32] { 114, 131, 12, 8, 21, 60, 208, 161,
                                                        105, 63,  250, 125, 4, 81, 142, 188, 
                                                        206, 29,  84, 13, 188, 54, 123, 40, 
                                                        159, 103, 30, 69, 126, 75, 194, 125 };
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Doc">要加密的XML文件实例</param>
        /// <param name="ElementName">要加密XML文件中的元素</param>
        /// <param name="Key">密匙</param>
        public static void Encrypt(XmlDocument Doc, string ElementName, SymmetricAlgorithm Key)
        {
            // Check the arguments. 
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementName == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (Key == null)
                throw new ArgumentNullException("Alg");
            ////////////////////////////////////////////////
            // Find the specified element in the XmlDocument
            // object and create a new XmlElemnt object.
            ////////////////////////////////////////////////
            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementName)[0] as XmlElement;
            // Throw an XmlException if the element was not found.
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }
            //////////////////////////////////////////////////
            // Create a new instance of the EncryptedXml class
            // and use it to encrypt the XmlElement with the
            // symmetric key.
            //////////////////////////////////////////////////
            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, Key, false);
            ////////////////////////////////////////////////
            // Construct an EncryptedData object and populate
            // it with the desired encryption information.
            ////////////////////////////////////////////////
            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;
            // Create an EncryptionMethod element so that the
            // receiver knows which algorithm to use for decryption.
            // Determine what kind of algorithm is being used and
            // supply the appropriate URL to the EncryptionMethod element.
            string encryptionMethod = null;
            if (Key is TripleDES)
            {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (Key is DES)
            {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            }
            else if (Key is Rijndael)
            {
                switch (Key.KeySize)
                {
                    case 128:
                        encryptionMethod = EncryptedXml.XmlEncAES128Url;
                        break;
                    case 192:
                        encryptionMethod = EncryptedXml.XmlEncAES192Url;
                        break;
                    case 256:
                        encryptionMethod = EncryptedXml.XmlEncAES256Url;
                        break;
                }
            }
            else
            {
                // Throw an exception if the transform is not in the previouscategories
                throw new CryptographicException("The specified algorithm is notsupported for XML Encryption.");
            }
            edElement.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            // Add the encrypted element data to the
            // EncryptedData object.
            edElement.CipherData.CipherValue = encryptedElement;
            ////////////////////////////////////////////////////
            // Replace the element from the original XmlDocument
            // object with the EncryptedData element.
            ////////////////////////////////////////////////////
            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Doc">要解密的XML文件实例</param>
        /// <param name="Key">密匙</param>
        public static void Decrypt(XmlDocument Doc, SymmetricAlgorithm Key)
        {
            // Check the arguments. 
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (Key == null)
                throw new ArgumentNullException("Alg");
            // Find the EncryptedData element in the XmlDocument.
            XmlElement encryptedElement = Doc.GetElementsByTagName("EncryptedData")[0] as XmlElement;
            // If the EncryptedData element was not found, throw anexception.
            if (encryptedElement == null)
            {
                throw new XmlException("The EncryptedData element was notfound.");
            }

            // Create an EncryptedData object and populate it.
            EncryptedData edElement = new EncryptedData();
            edElement.LoadXml(encryptedElement);
            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml();

            // Decrypt the element using the symmetric key.
            byte[] rgbOutput = exml.DecryptData(edElement, Key);
            // Replace the encryptedData element with the plaintext XMLelement.
            exml.ReplaceData(encryptedElement, rgbOutput);
        }
        /// <summary>
        /// 文件加密
        /// </summary>
        /// <param name="sFileName"></param>
        public static void FileEncrypt(string sFileName)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(sFileName);

                RijndaelManaged key = new RijndaelManaged();
                key.IV = XMLCrypto.CryptoIV;
                key.Key = XMLCrypto.CryptoKey;
                XMLCrypto.Encrypt(xmldoc, "adminName", key);
                XMLCrypto.Encrypt(xmldoc, "adminPassward", key);
                XMLCrypto.Encrypt(xmldoc, "server", key);
                XMLCrypto.Encrypt(xmldoc, "database", key);
                XMLCrypto.Encrypt(xmldoc, "uid", key);
                XMLCrypto.Encrypt(xmldoc, "pwd", key);
                XMLCrypto.Encrypt(xmldoc, "port", key);
                for (int i = 0; i < 5000; i++)
                {
                    ;
                }
                xmldoc.Save(sFileName);
                key.Clear();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 保存文件加密
        /// </summary>
        /// <param name="sFileName"></param>
        public static void SaveEncryptFile(string sFileName, XmlDocument xmldoc)
        {
            try
            {
                RijndaelManaged key = new RijndaelManaged();
                key.IV = XMLCrypto.CryptoIV;
                key.Key = XMLCrypto.CryptoKey;
                XMLCrypto.Encrypt(xmldoc, "adminName", key);
                XMLCrypto.Encrypt(xmldoc, "adminPassward", key);
                XMLCrypto.Encrypt(xmldoc, "server", key);
                XMLCrypto.Encrypt(xmldoc, "database", key);
                XMLCrypto.Encrypt(xmldoc, "uid", key);
                XMLCrypto.Encrypt(xmldoc, "pwd", key);
                XMLCrypto.Encrypt(xmldoc, "port", key);
                for (int i = 0; i < 5000; i++)
                {
                    ;
                }
                xmldoc.Save(sFileName);
                key.Clear();
            }
            catch
            {
                throw;
            }
        }

        public static XmlDocument FileDecrypt(string sFileName, int iCount)
        {
            try
            {
                RijndaelManaged key = new RijndaelManaged();
                key.IV = XMLCrypto.CryptoIV;
                key.Key = XMLCrypto.CryptoKey;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(sFileName);
                for (int i = 0; i < iCount; i++)
                {
                    XMLCrypto.Decrypt(xmlDoc, key);
                }
                return xmlDoc;
            }
            catch
            {
                throw;
            }
        }
    }
}
