using System;
using System.IO;

class Program
{
    static string masterPassword = "salainen";
    static string dataFile = "passwords.txt";

    static void Main()
    {
        Console.Write("Anna master-salasana: ");
        string input = Console.ReadLine();

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
            Console.WriteLine("4. Lopeta");
            Console.Write("Valinta: ");

            string choice = Console.ReadLine();

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
        string description = Console.ReadLine();

        Console.Write("Anna salasana: ");
        string password = Console.ReadLine();

        File.AppendAllText(dataFile, $"{description}:{password}\n");
        Console.WriteLine("Salasana tallennettu!");
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
            Console.WriteLine(line);
        }
    }

    // Varsinainen salasanageneraattori
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

    // Tämä on käyttöliittymä salasanageneraattorille
    static void GeneratePasswordMenu()
    {
        Console.Write("Kuinka pitkä salasana luodaan? (oletus 12): ");
        string input = Console.ReadLine();

        int length = 12;
        if (int.TryParse(input, out int parsedLength))
        {
            length = parsedLength;
        }

        string newPassword = GeneratePassword(length);
        Console.WriteLine("Luotu salasana: " + newPassword);

        Console.Write("Tallennetaanko tämä salasana tiedostoon? (k/e): ");
        string save = Console.ReadLine();

        if (save.ToLower() == "k")
        {
            Console.Write("Anna kuvaus (esim. Gmail): ");
            string description = Console.ReadLine();

            File.AppendAllText(dataFile, $"{description}:{newPassword}\n");
            Console.WriteLine("Salasana tallennettu!");
        }
    }
}


