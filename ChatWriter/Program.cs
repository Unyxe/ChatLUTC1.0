
namespace ChatWriter;

public static class ChatWriter
{
    public static List<string> MutedUsers = new List<string>();
    public static string MainFolderPath = @"C:\Chat\Sessions\";
    public static string RootFolderPath = @"C:\Chat\";

    static void ClearMessageZone(string m)
    {
        Console.SetCursorPosition(0, 6);
        for(int i = 0; i < m.Length/Console.BufferWidth + 1; i++) 
        {
            Console.SetCursorPosition(0, 6+i);
            Console.Write(new String(' ', Console.BufferWidth));
        }
                
        Console.SetCursorPosition(0, 6);
    }
    static void ClearNotificationZone()
    {
        Console.SetCursorPosition(30, 2);
        Console.Write(new String(' ', Console.BufferWidth - 30));
    }
    static void SendMessage(string message, string name, string p)
    {
        FileStream sessionFileStream = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(sessionFileStream);
        reader.ReadLine();
        int i = Int32.Parse(reader.ReadLine()+"");
        reader.Close();
        sessionFileStream = new FileStream(p, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        StreamWriter writer = new StreamWriter(sessionFileStream);
        writer.WriteLine(name + ">" + message);
        writer.WriteLine(i + 1);
        writer.Close();
    }
    static void SendNotification(string message, ConsoleColor color)
    {
        ClearNotificationZone();
        Console.SetCursorPosition(30, 2);
        Console.ForegroundColor = color;
        Console.WriteLine(message);
    }

    static void SendCommand(string[] command)
    {
        switch (command[0])
        {
            case "mute":
                if (!MutedUsers.Contains(command[1]))
                {
                    MutedUsers.Add(command[1]);
                    SendNotification("User " + command[1] + " won't appear in your chat window anymore.",ConsoleColor.Green );
                }
                else
                {
                    SendNotification("User " + command[1] + " was muted already.",ConsoleColor.Red );
                }

                break;
            case "unmute":
                if (MutedUsers.Contains(command[1]))
                {
                    MutedUsers.Remove(command[1]);
                    SendNotification("User " + command[1] + " is successfully unmuted.",ConsoleColor.Green );
                }
                else
                {
                    SendNotification("User " + command[1] + " wasn't muted.",ConsoleColor.Red );
                }
                
                break;
        }
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

        if (!Directory.Exists(RootFolderPath))
        {
            DirectoryInfo di = Directory.CreateDirectory(RootFolderPath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(MainFolderPath);
        }
        if (!Directory.Exists(MainFolderPath))
        {
            Directory.CreateDirectory(MainFolderPath);
        }
        
        Console.WriteLine("Enter the session id:");
        string sessionId = Console.ReadLine()+"";
        string sessionFilePath = MainFolderPath + sessionId;
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine()+"";
        username += "(#" + rn.Next() % 1000 + 1 + ")";
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + sessionId);
        }
        Console.WriteLine("Connected!\n");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string m = Console.ReadLine() + "";
            if (m.Length < 1)
            {
                continue;
            }

            if (m[0] == '!')
            {
                string command = m.Remove(0, 1);
                string[] commands = command.Split(" ");
                SendCommand(commands);
            }
            else if (Filter(m))
            {
                SendMessage(m, username, sessionFilePath);
                SendNotification("Sent", ConsoleColor.Green);
            }
            else
            {
                SendNotification("Your message was not sent because it does not satisfy our filters.", ConsoleColor.Red);
            }
            ClearMessageZone(m);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}