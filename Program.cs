using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string masterPassword = "salainen";
    static string dataFile = "passwords.txt";

    static void Main()
    {
        Console.Write("Anna master-salasana: ");
        string input = Console.ReadLine() ?? "";

        if (input != masterPassword)
        {
            Console.WriteLine("Väärä salasana, ohjelma sulkeutuu.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n--- Password Manager ---");
            Console.WriteLine("1. Lisää salasana");
            Console.WriteLine("2. Näytä salasanat");
            Console.WriteLine("3. Luo vahva salasana");
            Console.WriteLine("4. Hae salasana");
            Console.WriteLine("5. Lopeta");
            Console.Write("Valinta: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    AddPassword();
                    break;
                case "2":
                    ShowPasswords();
                    break;
                case "3":
                    GeneratePasswordMenu();
                    break;
                case "4":
                    SearchPassword();
                    break;
                case "5":
                    Console.WriteLine("Ohjelma sulkeutuu...");
                    return;
                default:
                    Console.WriteLine("Virheellinen valinta.");
                    break;
            }
        }
    }

    static void AddPassword()
{
    Console.Write("Anna palvelun kuvaus (esim. Gmail): ");
    string description = Console.ReadLine() ?? "";

    Console.Write("Anna salasana: ");
    string password = Console.ReadLine() ?? "";

    // johdetaan avain master-salasanasta (yksinkertaisuuden vuoksi sama salt aina)
    byte[] salt = System.Text.Encoding.UTF8.GetBytes("yksinkertainen_suola");
    byte[] key = DeriveKey(masterPassword, salt);

    // salataan salasana
    byte[] encrypted = EncryptString(password, key);
    string encryptedBase64 = Convert.ToBase64String(encrypted);

    // tallennetaan muodossa: kuvaus|SALATTU
    File.AppendAllText(dataFile, $"{description}|{encryptedBase64}\n");
    Console.WriteLine("Salasana tallennettu (salattuna)!");
}

    static void ShowPasswords()
{
    if (!File.Exists(dataFile))
    {
        Console.WriteLine("Ei tallennettuja salasanoja.");
        return;
    }

    Console.WriteLine("\n--- Tallennetut salasanat ---");
    string[] lines = File.ReadAllLines(dataFile);

    foreach (string line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        // odotetaan muotoa: kuvaus|base64
        var parts = line.Split('|');
        if (parts.Length != 2)
        {
            Console.WriteLine(line); // vanhat selkokieliset rivit näytetään sellaisenaan
            continue;
        }

        string description = parts[0];
        string encryptedBase64 = parts[1];

        try
        {
            byte[] salt = System.Text.Encoding.UTF8.GetBytes("yksinkertainen_suola");
            byte[] key = DeriveKey(masterPassword, salt);

            byte[] encrypted = Convert.FromBase64String(encryptedBase64);
            string decryptedPassword = DecryptToString(encrypted, key);

            Console.WriteLine($"{description}: {decryptedPassword}");
        }
        catch
        {
            Console.WriteLine($"{description}: [virhe purkamisessa]");
        }
    }
}

    static string GeneratePassword(int length = 12)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
        Random rand = new Random();
        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] = chars[rand.Next(chars.Length)];
        }

        return new string(password);
    }

    static void GeneratePasswordMenu()
    {
        Console.Write("Kuinka pitkä salasana luodaan? (oletus 12): ");
        string input = Console.ReadLine() ?? "";

        int length = 12;
        if (int.TryParse(input, out int parsedLength))
        {
            length = parsedLength;
        }

        string newPassword = GeneratePassword(length);
        Console.WriteLine("Luotu salasana: " + newPassword);

        Console.Write("Tallennetaanko tämä salasana tiedostoon? (k/e): ");
        string save = Console.ReadLine() ?? "";

        if (save.ToLower() == "k")
{
    Console.Write("Anna kuvaus (esim. Gmail): ");
    string description = Console.ReadLine() ?? "";

    // johdetaan avain
    byte[] salt = System.Text.Encoding.UTF8.GetBytes("yksinkertainen_suola");
    byte[] key = DeriveKey(masterPassword, salt);

    // salataan generoitu salasana
    byte[] encrypted = EncryptString(newPassword, key);
    string encryptedBase64 = Convert.ToBase64String(encrypted);

    File.AppendAllText(dataFile, $"{description}|{encryptedBase64}\n");
    Console.WriteLine("Salasana tallennettu (salattuna)!");
}
    }

    // Hakutoiminto
    static void SearchPassword()
{
    if (!File.Exists(dataFile))
    {
        Console.WriteLine("Ei tallennettuja salasanoja.");
        return;
    }

    Console.Write("Anna hakusana (esim. Gmail): ");
    string search = Console.ReadLine().ToLower();

    string[] lines = File.ReadAllLines(dataFile);
    bool found = false;

    Console.WriteLine("\n--- Hakutulokset ---");
    foreach (string line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        // odotetaan muotoa: kuvaus|base64
        var parts = line.Split('|');
        if (parts.Length != 2)
        {
            if (line.ToLower().Contains(search))
            {
                Console.WriteLine(line); // näytetään vanhat rivit sellaisenaan
                found = true;
            }
            continue;
        }

        string description = parts[0];
        string encryptedBase64 = parts[1];

        if (description.ToLower().Contains(search))
        {
            try
            {
                byte[] salt = System.Text.Encoding.UTF8.GetBytes("yksinkertainen_suola");
                byte[] key = DeriveKey(masterPassword, salt);

                byte[] encrypted = Convert.FromBase64String(encryptedBase64);
                string decryptedPassword = DecryptToString(encrypted, key);

                Console.WriteLine($"{description}: {decryptedPassword}");
            }
            catch
            {
                Console.WriteLine($"{description}: [virhe purkamisessa]");
            }
            found = true;
        }
    }

    if (!found)
    {
        Console.WriteLine("Ei osumia hakusanalla.");
    }
}
// ============ AES-salauksen metodit ============

// Johdetaan AES-avain käyttäjän master-salasanasta.
// Tämä tehdään PBKDF2-algoritmilla (Rfc2898DeriveBytes).
static byte[] DeriveKey(string masterPassword, byte[] salt, int iterations = 100000)
{
    using var pbkdf2 = new Rfc2898DeriveBytes(
        password: masterPassword,
        salt: salt,
        iterations: iterations,
        hashAlgorithm: HashAlgorithmName.SHA256
    );

    return pbkdf2.GetBytes(32); // 256-bittinen avain
}

// Salaa annetun tekstin ja palauttaa tavutaulukon (IV + ciphertext).
static byte[] EncryptString(string plainText, byte[] key)
{
    using Aes aes = Aes.Create();
    aes.Key = key;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;

    aes.GenerateIV(); // uusi IV jokaiselle salaukselle

    using var encryptor = aes.CreateEncryptor();
    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    byte[] cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

    // yhdistetään IV + cipher samaan taulukkoon
    byte[] output = new byte[aes.IV.Length + cipher.Length];
    Buffer.BlockCopy(aes.IV, 0, output, 0, aes.IV.Length);
    Buffer.BlockCopy(cipher, 0, output, aes.IV.Length, cipher.Length);

    return output;
}

// Purkaa annetun IV+cipher -taulukon selkokieleksi.
static string DecryptToString(byte[] ivPlusCipher, byte[] key)
{
    using Aes aes = Aes.Create();
    aes.Key = key;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;

    int ivLen = aes.BlockSize / 8;
    byte[] iv = new byte[ivLen];
    byte[] cipher = new byte[ivPlusCipher.Length - ivLen];

    Buffer.BlockCopy(ivPlusCipher, 0, iv, 0, ivLen);
    Buffer.BlockCopy(ivPlusCipher, ivLen, cipher, 0, cipher.Length);

    aes.IV = iv;
    using var decryptor = aes.CreateDecryptor();
    byte[] plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

    return Encoding.UTF8.GetString(plain);
}


}


