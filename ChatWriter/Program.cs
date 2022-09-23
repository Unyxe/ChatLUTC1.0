using System.IO;
using System;
using System.Text;

public class ChatWriter
{
    static void SendMessage(string message, string name, string p)
    {
        FileStream sessionFileStream = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(sessionFileStream);
        reader.ReadLine();
        int i = Int32.Parse(reader.ReadLine());
        reader.Close();
        sessionFileStream = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        StreamWriter writer = new StreamWriter(sessionFileStream);
        writer.WriteLine(name + ">" +message);
        writer.WriteLine(i+1);
        writer.Close();
    }

    static bool Filter(string message)
    {
        if (message.Length > 100)
        {
            return false;
        }

        return true;
    }
    static void CreateSession(string p)
    {
        FileStream file = File.Create(p);
        StreamWriter writer = new StreamWriter(file);
        writer.WriteLine();
        writer.WriteLine("0");
        writer.Close();
    }
    public static void Main(string[] args)
    {
        Random rn = new Random();
        string mainFolderPath = @"C:\Chat\Sessions\";
        Console.WriteLine("Enter the session id:");
        string sessionId = Console.ReadLine();
        string sessionFilePath = mainFolderPath + sessionId;
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine();
        username += "(#" +rn.Next()%1000+1+")";
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + sessionId);
        }
        Console.WriteLine("Connected!\n");

        while (true)
        {
            string m = Console.ReadLine() + "";
            if (m.Length < 1)
            {
                continue;
            }
            if (Filter(m))
            {
                SendMessage(m, username, sessionFilePath);
            }
            else
            {
                Console.WriteLine("Your message was not sent because it does not satisfy our filters.");
            }
        }
    }
}