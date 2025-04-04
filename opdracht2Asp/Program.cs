using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // Vaste sleutel (32 bytes = 256 bits)
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");

    // Vaste IV (16 bytes = 128 bits)
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF0123456789");

    static void Main()
    {
        Console.Write("Voer een creditcardnummer in: ");
        string creditCardNumber = Console.ReadLine();

        string encrypted = Encrypt(creditCardNumber);
        Console.WriteLine($"Versleuteld: {encrypted}");

        string decrypted = Decrypt(encrypted);
        Console.WriteLine($"Ontsleuteld: {decrypted}");

        Console.WriteLine("Druk op een toets om af te sluiten...");
        Console.ReadKey();
    }

    static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plainText);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    static string Decrypt(string cipherText)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream ms = new(buffer);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);

        return sr.ReadToEnd();
    }
}
