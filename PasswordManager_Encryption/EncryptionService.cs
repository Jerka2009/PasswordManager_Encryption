using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager_Encryption
{
    public static class EncryptionService
    {
        // Фиксированный ключ на базе мастер-пароля. В будущем его можно передавать извне.
        private static readonly byte[] MasterKey = SHA256.HashData(Encoding.UTF8.GetBytes("SuperSecretMasterPassword123"));
        private const int IvLength = 16; // Для AES размер IV всегда 16 байт

        /// <summary>
        /// Шифрует строку и возвращает массив байт, содержащий [IV (16 байт) + Шифротекст]
        /// </summary>
        public static byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = MasterKey;
                // aesAlg.IV генерируется автоматически (случайно) при вызове Aes.Create()

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // 1. Записываем IV в самое начало потока
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    // 2. Дописываем зашифрованные данные
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// Расшифровывает массив байт, извлекая IV из первых 16 байт структуры
        /// </summary>
        public static string Decrypt(byte[] cipherTextWithIv)
        {
            if (cipherTextWithIv == null || cipherTextWithIv.Length < IvLength)
                throw new ArgumentException("Некорректные или слишком короткие зашифрованные данные.", nameof(cipherTextWithIv));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = MasterKey;

                // 1. Извлекаем первые 16 байт — это оригинальный IV для этой записи
                byte[] iv = new byte[IvLength];
                Array.Copy(cipherTextWithIv, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                // 2. Создаем дешифратор
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // 3. Читаем остаток массива (пропуская первые 16 байт IV)
                int cipherTextLength = cipherTextWithIv.Length - IvLength;
                using (MemoryStream msDecrypt = new MemoryStream(cipherTextWithIv, IvLength, cipherTextLength))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
