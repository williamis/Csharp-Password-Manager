using PasswordManager.Core;

class Program
{
    private static IEncryptionService _encryption = null!;
    private static IPasswordRepository _repository = null!;

    static async Task Main()
    {
        _encryption = new AesEncryptionService();
        _repository = new FilePasswordRepository("passwords.json");

        Console.Title = "SecureVault Pro v3.1";
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== SecureVault Pro v3.1 ===");
        Console.ResetColor();

        Console.Write("Enter Master Password: ");
        string masterPassword = ReadMaskedPassword();

        if (string.IsNullOrWhiteSpace(masterPassword)) return;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Add New Password");
            Console.WriteLine("2. View All Passwords");
            Console.WriteLine("3. Search by Service");
            Console.WriteLine("4. Exit");
            Console.ResetColor();
            Console.Write("Selection: ");

            switch (Console.ReadLine())
            {
                case "1": await AddPasswordAsync(masterPassword); break;
                case "2": await ShowPasswordsAsync(masterPassword); break;
                case "3": await SearchPasswordsAsync(masterPassword); break;
                case "4": 
                    Console.WriteLine("Closing safely...");
                    return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    private static async Task AddPasswordAsync(string masterPassword)
    {
        Console.Write("\nService (e.g. Google): ");
        string desc = Console.ReadLine() ?? "Unknown";
        
        Console.Write("Password (leave empty to generate): ");
        string pwd = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(pwd))
        {
            pwd = GenerateStrongPassword();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Generated Password: {pwd}");
            Console.ResetColor();
        }

        var aesSvc = (AesEncryptionService)_encryption;
        byte[] salt = aesSvc.GenerateRandomSalt();
        byte[] key = _encryption.DeriveKey(masterPassword, salt);
        byte[] encrypted = _encryption.Encrypt(pwd, key);

        await _repository.AddAsync(new PasswordEntry(
            desc, 
            Convert.ToBase64String(encrypted), 
            Convert.ToBase64String(salt)
        ));
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Successfully secured!");
        Console.ResetColor();
    }

    private static async Task ShowPasswordsAsync(string masterPassword)
    {
        var entries = await _repository.GetAllAsync();
        DisplayEntries(entries, masterPassword);
    }

    private static async Task SearchPasswordsAsync(string masterPassword)
    {
        Console.Write("\nEnter search term: ");
        string query = (Console.ReadLine() ?? "").ToLower();

        var allEntries = await _repository.GetAllAsync();
        var matches = allEntries.Where(e => e.Description.ToLower().Contains(query));

        DisplayEntries(matches, masterPassword);
    }

    private static void DisplayEntries(IEnumerable<PasswordEntry> entries, string masterPassword)
    {
        Console.WriteLine("\n--- Credentials ---");
        if (!entries.Any()) Console.WriteLine("No entries found.");

        foreach (var entry in entries)
        {
            try {
                byte[] salt = Convert.FromBase64String(entry.SaltBase64);
                byte[] key = _encryption.DeriveKey(masterPassword, salt);
                byte[] cipher = Convert.FromBase64String(entry.EncryptedBase64);
                string decrypted = _encryption.Decrypt(cipher, key);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[+] ");
                Console.ResetColor();
                Console.WriteLine($"{entry.Description.PadRight(15)} : {decrypted}");
            } catch {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] {entry.Description.PadRight(15)} : [DECRYPTION FAILED]");
                Console.ResetColor();
            }
        }
    }

    private static string GenerateStrongPassword(int length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string ReadMaskedPassword()
    {
        var pwd = new System.Text.StringBuilder();
        while (true) {
            var i = Console.ReadKey(true);
            if (i.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
            if (i.Key == ConsoleKey.Backspace && pwd.Length > 0) {
                pwd.Remove(pwd.Length - 1, 1); Console.Write("\b \b");
            } else if (!char.IsControl(i.KeyChar)) {
                pwd.Append(i.KeyChar); Console.Write("*");
            }
        }
        return pwd.ToString();
    }
}