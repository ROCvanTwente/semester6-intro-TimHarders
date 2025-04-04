using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // AES sleutel en IV
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF0123456789");

    // Connection string aanpassen naar jouw server en database
    private const string ConnectionString = "Server=localhost;Database=JouwDatabase;Trusted_Connection=True;";

    static void Main()
    {
        // Gegevens invoeren
        var persoon = new Persoon
        {
            Voornaam = Vraag("Voornaam"),
            Achternaam = Vraag("Achternaam"),
            Straat = Vraag("Straat"),
            Huisnummer = Vraag("Huisnummer"),
            Postcode = Vraag("Postcode"),
            Woonplaats = Vraag("Woonplaats"),
            Creditcardnummer = Vraag("Creditcardnummer")
        };

        // Creditcardnummer versleutelen
        byte[] encryptedCard = Encrypt(persoon.Creditcardnummer);
        persoon.CreditcardVersleuteld = encryptedCard;

        // Opslaan in database
        SaveToDatabase(persoon);

        Console.WriteLine("Persoonsgegevens opgeslagen met versleuteld creditcardnummer.");
        Console.ReadKey();
    }

    static string Vraag(string veld)
    {
        Console.Write($"{veld}: ");
        return Console.ReadLine();
    }

    static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plainText);
        sw.Close();

        return ms.ToArray();
    }

    static void SaveToDatabase(Persoon p)
    {
        using SqlConnection conn = new(ConnectionString);
        conn.Open();

        string sql = @"INSERT INTO Personen (Voornaam, Achternaam, Straat, Huisnummer, Postcode, Woonplaats, Creditcardnummer)
                       VALUES (@Voornaam, @Achternaam, @Straat, @Huisnummer, @Postcode, @Woonplaats, @Creditcardnummer)";

        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@Voornaam", p.Voornaam);
        cmd.Parameters.AddWithValue("@Achternaam", p.Achternaam);
        cmd.Parameters.AddWithValue("@Straat", p.Straat);
        cmd.Parameters.AddWithValue("@Huisnummer", p.Huisnummer);
        cmd.Parameters.AddWithValue("@Postcode", p.Postcode);
        cmd.Parameters.AddWithValue("@Woonplaats", p.Woonplaats);
        cmd.Parameters.AddWithValue("@Creditcardnummer", p.CreditcardVersleuteld);

        cmd.ExecuteNonQuery();
    }
}

class Persoon
{
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }
    public string Straat { get; set; }
    public string Huisnummer { get; set; }
    public string Postcode { get; set; }
    public string Woonplaats { get; set; }
    public string Creditcardnummer { get; set; }
    public byte[] CreditcardVersleuteld { get; set; }
}
