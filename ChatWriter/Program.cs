
namespace ChatWriter;

public static class ChatWriter
{
    public static List<string> MutedUsers = new List<string>();
    public static string RootFolderPath = @"C:\Chat\";  //Network address
    public static string MainFolderPath = RootFolderPath + @"Sessions\";
    public static string LocalOptionsPath = @"C:\Private\Chat\";  //Private address
    public static string? SessionId;
    public static List<List<string>> OptionsData = new List<List<string>>();

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
        OptionsData.Clear();
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
        MutedUsers.Insert(0, "Mute");
        OptionsData.Add(MutedUsers);
        PrintOptionsData();
        WriteOptionsFile(OptionsData);
        MutedUsers.Remove("Mute");
    }

    static void PrintOptionsData()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 8);
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.SetCursorPosition(0, 8);
        bool first = true;
        foreach (List<string> list in OptionsData)
        {
            first = true;
            foreach (string str in list)
            {
                Console.Write(str);
                if (first)
                {
                    first = false;
                    Console.Write(":");
                }
                Console.Write("\t");
            }
            Console.Write("\n");
        }
        Console.SetCursorPosition(0, 6);
    }
    static void ClearOptionsFile()
    {
        FileStream file = new FileStream(LocalOptionsPath + SessionId + "_opt", FileMode.Open, FileAccess.ReadWrite,
            FileShare.Read);
        
        StreamReader reader = new StreamReader(file);
        int count = 0;
        while (true)
        {
            string line = reader.ReadLine()+"";
            if (line == "")
            {
                break;
            }

            count++;
        }
        reader.Close();
        file = new FileStream(LocalOptionsPath + SessionId + "_opt", FileMode.Open, FileAccess.ReadWrite,
            FileShare.Write);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(new String(' ', 6400));
        writer.Close();
    }
    static void WriteOptionsFile(List<List<string>> data)
    {
        FileStream file;
        StreamWriter writer;
        ClearOptionsFile();
        file = new FileStream(LocalOptionsPath + SessionId + "_opt", FileMode.Open, FileAccess.ReadWrite,
            FileShare.ReadWrite);
        writer = new StreamWriter(file);
        
        for(int j = 0; j < data.Count();j++){
            for (int i = 0; i < data[j].Count(); i++)
            {
                if (i==0)
                {
                    writer.WriteLine("-" + data[j][0]);
                }
                else
                {
                    writer.WriteLine("--" + data[j][i]);
                }
            }
        }
        writer.Close();
    }
    static void LoadOptionsFile()
    {
        FileStream file = new FileStream(LocalOptionsPath + SessionId + "_opt", FileMode.Open, FileAccess.ReadWrite,
            FileShare.Read);
        StreamReader reader = new StreamReader(file);
        List<string> lines = new List<string>();
        List<List<string>> data = new List<List<string>>();
        while (true)
        {
            string line = reader.ReadLine()+"";
            if (line == "")
            {
                break;
            }
            lines.Add(line);
        }
        reader.Close();
        for (int i = 0; i < lines.Count(); i++)
        {
            lines[i] = lines[i].Replace(" ", "");
            if (lines[i] == "")
            {
                lines.Remove("");
            }

        }
        List<string> list = new List<string>();
        for (int i = 0; i < lines.Count(); i++)
        {
            if (lines[i][0] == '-' && lines[i][1] == '-')
            {
                Console.SetCursorPosition(0, 24);
                string mod = lines[i].Remove(0, 2);
                list.Add(mod);
                Console.WriteLine();
            }
            else
            {
                if (list.Count() < 1)
                {
                    list.Add(lines[i].Remove(0, 1));
                    if (i == lines.Count() - 1)
                    {
                        data.Add(new List<string>(list));
                        list.Clear();
                    }
                    continue;
                }
                data.Add(list);
                list.Clear();
                
            }

            if (i == lines.Count() - 1)
            {
                data.Add(new List<string>(list));
                list.Clear();
            }
        }
        OptionsData = new List<List<string>>(data);
        data.Clear();
        foreach (List<String> list_ in OptionsData)
        {
            if (list_[0] == "Mute")
            {
                MutedUsers = new List<string>(list_);
                MutedUsers.Remove("Mute");
            }
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

    static void CreateLocalOptionsFile(string p)
    {
        FileStream file = File.Create(p);
        file.Close();
    }

    static void CheckDirectories()
    {
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
        if (!Directory.Exists(LocalOptionsPath))
        {
            Directory.CreateDirectory(LocalOptionsPath);
        }
    }
    public static void Main(string[] args)
    {
        Random rn = new Random();

        CheckDirectories();
        
        Console.WriteLine("Enter the session id:");
        SessionId = Console.ReadLine()+"";
        string sessionFilePath = MainFolderPath + SessionId;

        string localOptionsFilePath = LocalOptionsPath + SessionId + "_opt";
        
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine()+"";
        username += "(#" + rn.Next() % 1000 + 1 + ")";
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + SessionId);
        }

        if (!File.Exists(localOptionsFilePath))
        {
            CreateLocalOptionsFile(localOptionsFilePath);
        }
        //ClearOptionsFile();
        
        Console.SetCursorPosition(0, 8);
        
        //Console.Write(new String('O', 10000));
        LoadOptionsFile();
        PrintOptionsData();
        Console.SetCursorPosition(0, 4);
        Console.WriteLine("Connected!\n");
        Console.SetCursorPosition(0, 6);
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