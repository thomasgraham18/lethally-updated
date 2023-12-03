using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Threading.Channels;
using System.Runtime.CompilerServices;

class Program
{
    private const string DownloadUrl = "https://thomasg.ca/uploads/Lethal Company.zip";
    private static string? extractPath;

    private const string BackupFolderName = "BepInEx";
    private const string BackupFileNameFormat = "Backup_{0}.zip";

    static void Main()
    {
        Banner();
        CheckAndSetDefaultPath();
        Menu();
    }

    private static void Menu()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Options: \r\n");
        Console.WriteLine("1. Install \r\n");
        Console.WriteLine("2. Install & Launch \r\n");
        Console.WriteLine("3. Backup \r\n");
        Console.WriteLine("4. Restore \r\n");

        string userInput = Console.ReadLine();
        Console.Clear();

        if (userInput != null)
        {
            string cleanedInput = userInput.Trim().ToLower();

            switch (cleanedInput)
            {
                case "1":
                case "install":
                case "download":
                case "i":
                    DownloadAndExtract();
                    Console.Clear();
                    break;

                case "2":
                case "launch":
                case "play":
                    DownloadAndExtract();
                    LaunchGame();
                    Console.Clear();
                    break;

                case "3":
                case "backup":
                case "b":
                    Backup();
                    Console.Clear();
                    break;

                case "4":
                case "restore":
                case "r":
                    Restore();
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.\r\n");
                    break;
            }

            Menu(); // Loop back to the menu
        }
    }

    private static void LaunchGame()
    {
        Console.WriteLine("Launching game... \r\n");
        Thread.Sleep(1000);
        System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\steam.exe", "steam://rungameid/1966720");
    }

    private static void Backup()
    {
        try
        {
            Console.WriteLine("Backing up current mods...\r\n");

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string backupFileName = string.Format(BackupFileNameFormat, timestamp);
            string backupFilePath = Path.Combine(extractPath!, backupFileName);
            string pluginsFolderPath = Path.Combine(extractPath!, BackupFolderName);

            ZipFile.CreateFromDirectory(pluginsFolderPath, backupFilePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Backup created successfully: {extractPath}\\{backupFileName}\r\n");
        }
        catch (Exception ex)
        {
            HandleError($"An error occurred during backup: {ex.Message}");
        }
    }

    private static void Restore()
    {
        try
        {
            Console.WriteLine("Restoring mods from backup...\r\n");

            Console.WriteLine("Enter the full path of the backup file (Right click -> Copy as path):");
            string? backupFilePath = Console.ReadLine()?.Trim('"');
            Console.WriteLine("\r\n");

            if (File.Exists(backupFilePath))
            {
                string bepInExFolderPath = Path.Combine(extractPath!, BackupFolderName);
                DeleteDirectory(bepInExFolderPath);
                Directory.CreateDirectory(bepInExFolderPath);
                ZipFile.ExtractToDirectory(backupFilePath, bepInExFolderPath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mods restored successfully\r\n");

                Menu();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Backup file not found. Please make sure the file path is correct and try again.\r\n");
            }
        }
        catch (Exception ex)
        {
            HandleError($"An error occurred during restore: {ex.Message}");
        }
    }

    private static void Banner()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\r\n    __    _________   __  __\r\n   / /   / ____/   | / / / /\r\n  / /   / /   / /| |/ / / / \r\n / /___/ /___/ ___ / /_/ /  \r\n/_____/\\____/_/  |_\\____/   \r\n                            \r\n");
        Console.WriteLine("Lethal Company Auto Updater - Banada Edition \r\n");
        Console.WriteLine("Mod List: \r\n");
        Console.WriteLine("Always Hear Walkies");
        Console.WriteLine("Better Saves");
        Console.WriteLine("Bigger Lobby");
        Console.WriteLine("Helmet Camera");
        Console.WriteLine("Item Quick Switch");
        Console.WriteLine("LC API");
        Console.WriteLine("LC Presence");
        Console.WriteLine("Lethally Wide");
        Console.WriteLine("Lethal Rebinding");
        Console.WriteLine("Let Me Look Down");
        Console.WriteLine("More Suits");
        Console.WriteLine("Reserved Flashlight Slot");
        Console.WriteLine("Reserved Walkie Slot");
        Console.WriteLine("Ship Lobby");
        Console.WriteLine("Ship Loot");
        Console.WriteLine("Skip To Multiplayer Menu\r\n");
    }

    private static void CheckAndSetDefaultPath()
    {
        string[] possiblePaths = new string[]
        {
        @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company",
        @"G:\SteamLibrary\steamapps\common\Lethal Company",
        @"D:\SteamLibrary\steamapps\common\Lethal Company",
        @"F:\SteamLibrary\steamapps\common\Lethal Company",
        @"E:\SteamLibrary\steamapps\common\Lethal Company",
        @"G:\Games\SteamLibrary\steamapps\common\Lethal Company",
        @"D:\Games\SteamLibrary\steamapps\common\Lethal Company",
        @"F:\Games\SteamLibrary\steamapps\common\Lethal Company",
        @"E:\Games\SteamLibrary\steamapps\common\Lethal Company",
        @"G:\Game\SteamLibrary\steamapps\common\Lethal Company",
        @"D:\Game\SteamLibrary\steamapps\common\Lethal Company",
        @"F:\Game\SteamLibrary\steamapps\common\Lethal Company",
        @"E:\Game\SteamLibrary\steamapps\common\Lethal Company"
        };

        foreach (string path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                extractPath = path;
                return;
            }
        }

        Console.WriteLine("The default path doesn't exist. Please enter the installation directory:");
        string userInput = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(userInput) || !Directory.Exists(userInput))
        {
            Console.WriteLine("Invalid directory. Please enter a valid installation directory:");
            userInput = Console.ReadLine();
        }

        extractPath = userInput;
    }


    private static void DownloadAndExtract()
    {
        try
        {
            Console.WriteLine("Downloading file...\r\n");

            // Create a WebClient for downloading
            using (WebClient webClient = new())
            {
                // Download the ZIP file
                webClient.DownloadFile(new Uri(DownloadUrl), "Lethal Company.zip");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("File downloaded...\r\n");
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Red;            // Delete BepInEx folder, and two other files if they exist
            Console.WriteLine("Deleting old BepInEx folder...");
            DeleteDirectory(Path.Combine(extractPath!, "BepInEx"));
            Thread.Sleep(1000);
            Console.WriteLine("Deleting old winthttp file...");
            DeleteFile(Path.Combine(extractPath!, "winhttp.dll"));
            Thread.Sleep(1000);
            Console.WriteLine("Deleting old doorstop_config file...\r\n");
            DeleteFile(Path.Combine(extractPath!, "doorstop_config.ini"));
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Old mods removed...\r\n");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Extracting mods...\r\n");
            Thread.Sleep(1000);
            ZipFile.ExtractToDirectory("Lethal Company.zip", extractPath!);
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mods extracted...\r\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Deleting downloaded zip...\r\n");
            Thread.Sleep(1000);
            File.Delete("Lethal Company.zip");
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("File deleted...\r\n");

            // Display success message
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // Display an error message if an exception occurs
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void DeleteDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static void HandleError(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
    }
}
