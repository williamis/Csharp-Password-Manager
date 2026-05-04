namespace PasswordManager.Core;

/// <summary>
/// Provides an interface for cryptographic operations including encryption, 
/// decryption, and key derivation.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts plain text using an AES key.
    /// </summary>
    byte[] Encrypt(string plainText, byte[] key);

    /// <summary>
    /// Decrypts a cipher byte array back into plain text.
    /// </summary>
    string Decrypt(byte[] cipherText, byte[] key);

    /// <summary>
    /// Derives a secure cryptographic key from a master password using PBKDF2.
    /// </summary>
    byte[] DeriveKey(string password, byte[] salt);
}