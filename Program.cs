using System;
using System.IO; // tarvitaan tiedostojen käsittelyyn

class Program
{
    // Master-salasana ohjelman avaamiseen
    static string masterPassword = "salainen";

    // Tiedoston nimi, johon salasanat tallennetaan
    static string dataFile = "passwords.txt";

    static void Main()
    {
        // Kysytään master-salasana
        Console.Write("Anna master-salasana: ");
        string input = Console.ReadLine();

        if (input != masterPassword)
        {
            Console.WriteLine("Väärä salasana, ohjelma sulkeutuu.");
            return;
        }

        // Päävalikko
        while (true)
        {
            Console.WriteLine("\n--- Password Manager ---");
            Console.WriteLine("1. Lisää salasana");
            Console.WriteLine("2. Näytä salasanat");
            Console.WriteLine("3. Lopeta");
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
                    Console.WriteLine("Ohjelma sulkeutuu...");
                    return;
                default:
                    Console.WriteLine("Virheellinen valinta.");
                    break;
            }
        }
    }

    // Metodi salasanan lisäämiselle
    static void AddPassword()
    {
        Console.Write("Anna palvelun kuvaus (esim. Gmail): ");
        string description = Console.ReadLine();

        Console.Write("Anna salasana: ");
        string password = Console.ReadLine();

        // Tallennetaan tiedostoon muodossa: kuvaus:salasana
        File.AppendAllText(dataFile, $"{description}:{password}\n");
        Console.WriteLine("Salasana tallennettu!");
    }

    // Metodi salasanojen näyttämiselle
    static void ShowPasswords()
    {
        // Jos tiedostoa ei ole olemassa, kerrotaan käyttäjälle
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
}

