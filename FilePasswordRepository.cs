using System.Text.Json;

namespace PasswordManager.Core;

public class FilePasswordRepository : IPasswordRepository
{
    private readonly string _filePath;

    public FilePasswordRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<IEnumerable<PasswordEntry>> GetAllAsync()
    {
        try 
        {
            if (!File.Exists(_filePath)) return Enumerable.Empty<PasswordEntry>();
            
            var json = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrWhiteSpace(json)) return Enumerable.Empty<PasswordEntry>();

            return JsonSerializer.Deserialize<List<PasswordEntry>>(json) ?? new List<PasswordEntry>();
        }
        catch (JsonException)
        {
            Console.WriteLine("WARNING: Storage file is corrupted. Returning empty list.");
            return Enumerable.Empty<PasswordEntry>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error reading file: {ex.Message}");
            return Enumerable.Empty<PasswordEntry>();
        }
    }

    public async Task AddAsync(PasswordEntry entry)
    {
        var entries = (await GetAllAsync()).ToList();
        entries.Add(entry);
        var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}