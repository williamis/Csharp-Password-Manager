using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Core;

public class AesEncryptionService : IEncryptionService
{
    private const int Iterations = 600_000;

    public byte[] DeriveKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32); 
    }

    public byte[] GenerateRandomSalt() => RandomNumberGenerator.GetBytes(16);

    public byte[] Encrypt(string plainText, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return aes.IV.Concat(cipher).ToArray();
    }

    public string Decrypt(byte[] fullCipher, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        
        byte[] iv = fullCipher.Take(16).ToArray();
        byte[] cipher = fullCipher.Skip(16).ToArray();

        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        byte[] plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plain);
    }
}