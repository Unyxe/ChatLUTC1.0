using System.IO;
using System;

public class ChatWindow
{
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
        string mainFolderPath = @"C:\Chat\Sessions\";
        Console.WriteLine("Enter the session id:");
        string sessionId = Console.ReadLine();
        string sessionFilePath = mainFolderPath + sessionId;
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + sessionId);
        }
        Console.WriteLine("Connected!\n");
        FileStream sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(sessionFileStream);
        reader.ReadLine();
        int i = Int32.Parse(reader.ReadLine());
        int i_ = i;
        reader.Close();
        while (true)
        {
            sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new StreamReader(sessionFileStream);
            reader.ReadLine();
            i = Int32.Parse(reader.ReadLine());
            reader.Close();
            if (i != i_)
            {
                i_ = i;
                sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(sessionFileStream);
                Console.WriteLine(reader.ReadLine());
                i = Int32.Parse(reader.ReadLine());
                reader.Close();
            }
        }
    }
}