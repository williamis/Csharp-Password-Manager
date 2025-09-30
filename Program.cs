using System;

class Program
{
    // Tämä on se "master"-salasana, jolla ohjelma avataan
    // myöhemmin tämän voisi tallentaa jonnekin tiedostoon
    static string masterPassword = "salainen";

    static void Main()
    {
        // Kysytään käyttäjältä salasanaa
        Console.Write("Anna master-salasana: ");
        string input = Console.ReadLine();

        // Jos salasana ei ole oikea, ohjelma loppuu heti
        if (input != masterPassword)
        {
            Console.WriteLine("Väärä salasana, ohjelma sulkeutuu.");
            return;
        }

        // Jos salasana on oikein, mennään päävalikkoon
        while (true)
        {
            Console.WriteLine("\n--- Password Manager ---");
            Console.WriteLine("1. Lisää salasana (ei vielä toteutettu)");
            Console.WriteLine("2. Näytä salasanat (ei vielä toteutettu)");
            Console.WriteLine("3. Lopeta");
            Console.Write("Valinta: ");

            string choice = Console.ReadLine();

            // Valikon käsittely
            switch (choice)
            {
                case "1":
                    Console.WriteLine(">> Tässä vaiheessa ei vielä lisätä salasanaa");
                    break;
                case "2":
                    Console.WriteLine(">> Tässä vaiheessa ei vielä näytetä salasanoja");
                    break;
                case "3":
                    Console.WriteLine("Ohjelma sulkeutuu...");
                    return; // lopettaa ohjelman
                default:
                    Console.WriteLine("Virheellinen valinta.");
                    break;
            }
        }
    }
}

