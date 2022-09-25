namespace ChatWindow;

public static class ChatWindow
{
    public static List<string> MutedUsers = new List<string>();
    public static string RootFolderPath = @"C:\Chat\";  //Network address
    public static string MainFolderPath = RootFolderPath + @"Sessions\";
    public static string LocalOptionsPath = @"C:\Private\Chat\";  //Private address
    public static string? SessionId;
    public static List<List<string>> OptionsData = new List<List<string>>();
    
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
        StreamWriter writer = new StreamWriter(file);
        writer.WriteLine("-Mute:");
        writer.Close();
    }
    static void PrintOptionsData()
    {
        var c = Console.GetCursorPosition();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(30, 2);
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.Write(new String(' ', Console.BufferWidth)+"\n");
        Console.SetCursorPosition(30, 2);
        bool first = true;
        if (OptionsData.Count == 0)
        {
            Console.Write(OptionsData.Count);
        }

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

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(c.Left, c.Top);
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
                
                string mod = lines[i].Remove(0, 2);
                list.Add(mod);
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
        SyncOptionsWithMuted();
    }

    static bool CheckMute(string m)
    {
        LoadOptionsFile();
        string name = "";
        for (int i = 0; i < m.Length; i++)
        {
            if (m[i] != '>')
            {
                name += m[i];
            }
            else
            {
                break;
            }
        }

        foreach (string user in MutedUsers)
        {
            if (user == name)
            {
                return false;
            }
        }

        return true;
    }

    static void SyncOptionsWithMuted()
    {
        foreach (List<String> list_ in OptionsData)
        {
            
            if (list_[0] == "Mute")
            {
                MutedUsers = new List<string>(list_);
                MutedUsers.Remove("Mute");
                break;
            }
        }
        
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
        CheckDirectories();
        
        Console.WriteLine("Enter the session id:");
        SessionId = Console.ReadLine()+"";
        string sessionFilePath = MainFolderPath + SessionId;
        
        string localOptionsFilePath = LocalOptionsPath + SessionId + "_opt";
        
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + SessionId);
        }
        if (!File.Exists(localOptionsFilePath))
        {
            CreateLocalOptionsFile(localOptionsFilePath);
        }
        LoadOptionsFile();
        Console.SetCursorPosition(0, 2);
        Console.WriteLine("Connected!\n");
        
        Console.SetCursorPosition(0, 6);
        
        FileStream sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(sessionFileStream);
        reader.ReadLine();
        int i = Int32.Parse(reader.ReadLine()+"");
        var iBuff = i;
        reader.Close();
        while (true)
        {
            sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new StreamReader(sessionFileStream);
            reader.ReadLine();
            i = Int32.Parse(reader.ReadLine()+"");
            reader.Close();
            if (i != iBuff)
            {
                iBuff = i;
                sessionFileStream = new FileStream(sessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(sessionFileStream);
                string message = reader.ReadLine();
                if (CheckMute(message))
                {
                    Console.WriteLine(message);
                }
                
                reader.Close();
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}