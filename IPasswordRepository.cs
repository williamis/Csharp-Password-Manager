namespace PasswordManager.Core;

public record PasswordEntry(string Description, string EncryptedBase64, string SaltBase64);

public interface IPasswordRepository
{
    Task<IEnumerable<PasswordEntry>> GetAllAsync();
    Task AddAsync(PasswordEntry entry);
}