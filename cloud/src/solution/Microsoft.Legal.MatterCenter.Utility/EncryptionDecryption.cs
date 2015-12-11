// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-shpate
// Created          : 10-13-2014
//
// ***********************************************************************
// <copyright file="EncryptionDecryption.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines encryption and decryption functions.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    #endregion

    /// <summary>
    /// Provides the functionality to encrypt and decrypt the refresh token.[
    /// </summary>
    public static class EncryptionDecryption
    {
        /// <summary>
        /// Calls the encrypt method to encrypt the refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh Token for encryption</param>
        /// <returns>Encrypted refresh token</returns>
        public static string Encrypt(string refreshToken)
        {
            string encryptedRefreshToken = null, key = ConfigurationManager.AppSettings["Encryption_Key"];
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(refreshToken))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(refreshToken);
                try
                {
                    if (null != bytes)
                    {
                        encryptedRefreshToken = EncryptDecryptRefreshToken(key, bytes, true);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                }
            }
            return encryptedRefreshToken;
        }
        /// <summary>
        /// Encrypts or decrypts the refresh token.
        /// </summary>
        /// <param name="key">Key used for decryption</param>
        /// <param name="bytes">Refresh Token bytes</param>
        /// <param name="encryptor">AES object for encryption or decryption</param>
        /// <param name="isEncrypt">Checks for encryption/decryption</param>
        /// <returns>Returns the encrypted or decrypted refresh token depending on the AES object being passed</returns>
        private static string EncryptDecryptRefreshToken(string key, byte[] bytes, bool isEncrypt)
        {
            string refreshToken = string.Empty;
            if (!string.IsNullOrEmpty(ConstantStrings.EncryptionVector) && !string.IsNullOrEmpty(key) && null != bytes)
            {
                byte[] vector = new byte[ConstantStrings.EncryptionVector.Length * sizeof(char)];
                System.Buffer.BlockCopy(key.ToCharArray(), 0, vector, 0, vector.Length);
                CryptoStream cryptoStream = null;
                using (Aes encryptor = Aes.Create())
                {
                    using (Rfc2898DeriveBytes encryptObject = new Rfc2898DeriveBytes(key, vector))
                    {
                        encryptor.Key = encryptObject.GetBytes(32); // 32 bytes encryption key 
                        encryptor.IV = encryptObject.GetBytes(16); // 16 bytes initialization vector
                    }
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (isEncrypt)
                        {
                            // To encrypt the refresh token
                            cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write); // Creates an object to apply the cryptographic transformation to a memory stream
                            cryptoStream.Write(bytes, 0, bytes.Length); // Applies the cryptographic transformation to refresh token
                            cryptoStream.Clear();
                            refreshToken = Convert.ToBase64String(memoryStream.ToArray());
                        }
                        else
                        {
                            // To decrypt the refresh token
                            cryptoStream = new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
                            cryptoStream.Write(bytes, 0, bytes.Length);
                            cryptoStream.Clear();
                            refreshToken = Encoding.Unicode.GetString(memoryStream.ToArray());
                        }
                    }
                }
            }
            return refreshToken;
        }

        /// <summary>
        /// Calls the decrypt method to decrypt the refresh token.
        /// </summary>
        /// <param name="encryptedRefreshToken">Encrypted refresh Token</param>
        /// <param name="key">Key used for decryption</param>
        /// <returns>Decrypted refresh token</returns>
        public static string Decrypt(string encryptedRefreshToken, string key)
        {
            string refreshToken = null;
            if (!string.IsNullOrEmpty(encryptedRefreshToken) && !string.IsNullOrEmpty(key))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(encryptedRefreshToken);
                    refreshToken = EncryptDecryptRefreshToken(key, bytes, false);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                }
            }
            return refreshToken;
        }
    }
}
