namespace ChatWindow;

public static class ChatWindow
{
    public static List<string> MutedUsers = new List<string>();
    public static string MainFolderPath = @"C:\Chat\Sessions\";
    public static string RootFolderPath = @"C:\Chat\";
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
        if (!File.Exists(sessionFilePath))
        {
            CreateSession(sessionFilePath);
            Console.WriteLine("This session id didn't exist so new one was created. Your unique session id is: " + sessionId);
        }
        Console.WriteLine("Connected!\n");
        
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
                Console.WriteLine(reader.ReadLine());
                reader.Close();
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}