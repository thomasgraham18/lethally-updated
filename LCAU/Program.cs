using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Threading.Channels;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

class Program
{
    private const string DownloadUrl = "https://thomasg.ca/uploads/Lethal Company.zip";
    private static string? extractPath;

    private const string BackupFolderName = "BepInEx";
    private const string BackupFileNameFormat = "Backup_{0}.zip";

    #region Methods

    static void Main()
    {
        CheckAndSetDefaultPath();
        Menu();
    }

    private static void CheckAndSetDefaultPath()
    {
        string[] possibleExtractPaths = new string[]
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

        foreach (string path in possibleExtractPaths)
        {
            if (Directory.Exists(path))
            {
                extractPath = path;
                return;
            }
        }

        Console.WriteLine("The default path doesn't exist. Please enter the installation directory: \r\n(Steam -> Lethal Company -> Right Click -> Properties -> Installed Files -> Browse -> Copy Address Bar)");

        string userInput = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(userInput) || !Directory.Exists(userInput))
        {
            Console.WriteLine("Invalid directory. Please enter a valid installation directory:");
            userInput = Console.ReadLine();
        }

        extractPath = userInput;
    }

    private static void Menu()
    {
        Banner();
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
                    Output("Invalid option. Please try again.\r\n", ConsoleColor.Red);
                    break;
            }

            Menu(); // Loop back to the menu
        }
    }

    private static void Backup()
    {
        try
        {
            Output("Backing up current mods...\r\n", ConsoleColor.Green);

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string backupFileName = string.Format(BackupFileNameFormat, timestamp);
            string backupFilePath = Path.Combine(extractPath!, backupFileName);
            string pluginsFolderPath = Path.Combine(extractPath!, BackupFolderName);

            ZipFile.CreateFromDirectory(pluginsFolderPath, backupFilePath);

            Output($"Backup created successfully: {extractPath}\\{backupFileName}\r\n", ConsoleColor.Green);
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
            Output("Restoring mods from backup...\r\n", ConsoleColor.Green);

            Console.WriteLine("Enter the full path of the backup file (Right click -> Copy as path):");
            string? backupFilePath = Console.ReadLine()?.Trim('"');
            Console.WriteLine("\r\n");

            if (File.Exists(backupFilePath))
            {
                string bepInExFolderPath = Path.Combine(extractPath!, BackupFolderName);
                DeleteDirectory(bepInExFolderPath);
                Directory.CreateDirectory(bepInExFolderPath);
                ZipFile.ExtractToDirectory(backupFilePath, bepInExFolderPath);

                Output("Mods restored successfully\r\n", ConsoleColor.Green);
            }
            else
            {
                Output("Backup file not found. Please make sure the file path is correct and try again.\r\n", ConsoleColor.Red);
            }
        }
        catch (Exception ex)
        {
            HandleError($"An error occurred during restore: {ex.Message}");
        }
    }

    private static void DownloadAndExtract()
    {
        try
        {
            Output("Downloading file...\r\n", ConsoleColor.Green);

            // Create a WebClient and download ZIP file
            using (WebClient webClient = new())
            {
                webClient.DownloadFile(new Uri(DownloadUrl), "Lethal Company.zip");
            }

            Output("File downloaded...\r\n", ConsoleColor.Green);

            Output("Deleting old BepInEx folder...", ConsoleColor.Red);

            // Delete old mods
            DeleteDirectory(Path.Combine(extractPath!, "BepInEx"));
            Thread.Sleep(1000);

            Output("Deleting old winthttp file...", ConsoleColor.Red);

            // Delete old BepInEx files
            DeleteFile(Path.Combine(extractPath!, "winhttp.dll"));
            Thread.Sleep(1000);

            Output("Deleting old doorstop_config file...\r\n", ConsoleColor.Red);

            // Delete old BepInEx files
            DeleteFile(Path.Combine(extractPath!, "doorstop_config.ini"));
            Thread.Sleep(1000);

            Output("Old mods removed...\r\n", ConsoleColor.Green);

            Output("Extracting mods...\r\n", ConsoleColor.Red);

            // Extract mods
            ZipFile.ExtractToDirectory("Lethal Company.zip", extractPath!);
            Thread.Sleep(1000);

            Output("Mods extracted...\r\n", ConsoleColor.Green);

            Output("Deleting downloaded zip...\r\n", ConsoleColor.Red);

            // Delete downloaded ZIp
            File.Delete("Lethal Company.zip");
            Thread.Sleep(1000);

            Output("File deleted...\r\n", ConsoleColor.Green);

            Output("Succesfully downloaded and installed mods", ConsoleColor.Green);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            // Display an error message if an exception occurs
            HandleError($"An error occurred during Download and Extract: {ex.Message}");
        }
    }

    #endregion

    #region Helpers

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

    private static void LaunchGame()
    {
        Console.WriteLine("Launching game... \r\n");
        Thread.Sleep(1000);
        try
        {
            if (Directory.Exists(@"C:\Program Files (x86)\Steam"))
            {
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\steam.exe", "steam://rungameid/1966720");
            }
            else
            {
                Console.WriteLine("You either have 32bit Steam, or you manually installed Steam to a custom path. Why?.");
                return;
            }

        }
        catch (Exception ex)
        {
            HandleError($"An error occurred launching the game {ex.Message}");
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
        Thread.Sleep(20000);
    }

    private static void Output(string output, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(output);
        Thread.Sleep(1000);
    }

    #endregion
}
