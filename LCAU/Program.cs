using System;
using System.IO;
using System.Net;
using System.IO.Compression;

class Program
{
    private const string DownloadUrl = "https://thomasg.ca/uploads/Lethal Company.zip";
    private static string? extractPath; // Set this to the desired extraction path

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
        Console.WriteLine("2. Backup \r\n");
        Console.WriteLine("3. Restore \r\n");

        string userInput = Console.ReadLine();
        Console.WriteLine("\r\n");


        if (userInput != null)
        {
            string cleanedInput = userInput.Trim().ToLower();

            if (cleanedInput == "1" || cleanedInput == "install" || cleanedInput == "download" || cleanedInput == "i" || cleanedInput == "banada")
            {
                DownloadAndExtract();
            }
            else if (cleanedInput == "2" || cleanedInput == "backup" || cleanedInput == "b")
            {
                Backup();
            }
            else if (cleanedInput == "3" || cleanedInput == "restore" || cleanedInput == "r")
            {
                Restore();
            }
        }

    }

    private static void Backup()
    {
        try
        {
            Console.WriteLine("Backing up current mods...\r\n");

            // Create a timestamp for the backup file
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string backupFileName = $"Backup_{timestamp}.zip";
            string backupFilePath = Path.Combine(extractPath!, backupFileName);

            // Specify the path to the Plugins folder
            string pluginsFolderPath = Path.Combine(extractPath!, "BepInEx");

            // Create a backup ZIP file containing only the Plugins folder
            ZipFile.CreateFromDirectory(pluginsFolderPath, backupFilePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Backup created successfully: {extractPath}\\{backupFileName}\r\n");

            // Display success message
            Menu();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // Display an error message if an exception occurs
            Console.WriteLine($"An error occurred during backup: {ex.Message}");
        }
    }

    private static void Restore()
    {
        try
        {
            Console.WriteLine("Restoring mods from backup...\r\n");

            // Prompt the user to enter the backup file path
            Console.WriteLine("Enter the full path of the backup file (Right click -> Copy as path):");
            string? backupFilePath = Console.ReadLine()?.Trim('"'); // Remove surrounding quotes if present
            Console.WriteLine("\r\n");
            if (File.Exists(backupFilePath))
            {
                // Specify the path to the Plugins folder
                string bepInExFolderPath = Path.Combine(extractPath!, "BepInEx");

                // Delete existing Plugins folder
                DeleteDirectory(bepInExFolderPath);

                // Create the Plugins folder if it doesn't exist
                Directory.CreateDirectory(bepInExFolderPath);

                // Extract only the Plugins folder from the backup file
                ZipFile.ExtractToDirectory(backupFilePath, bepInExFolderPath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mods restored successfully\r\n");

                // Display success message
                Menu();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Backup file not found. Please make sure the file path is correct and try again.\r\n");
                Menu();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // Display an error message if an exception occurs
            Console.WriteLine($"An error occurred during restore: {ex.Message}");
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
        string defaultPath = @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company";
        string jasonPath = @"G:\SteamLibrary\steamapps\common\Lethal Company";
        string zachPath = @"D:\SteamLibrary\steamapps\common\Lethal Company";
        string willPath = @"F:\SteamLibrary\steamapps\common\Lethal Company";
        string vincePath = @"E:\SteamLibrary\steamapps\common\Lethal Company";

        if (Directory.Exists(defaultPath))
        { extractPath = defaultPath; }
        else if (Directory.Exists(jasonPath))
        { extractPath = jasonPath; }
        else if (Directory.Exists(zachPath))
        { extractPath = zachPath; }
        else if (Directory.Exists(willPath))
        { extractPath = willPath; }
        else if (Directory.Exists(vincePath))
        { extractPath = vincePath; }
        else
        {
            Console.WriteLine("The default path doesn't exist. Please enter the installation directory:");
            string userInput = Console.ReadLine()!;

            while (string.IsNullOrWhiteSpace(userInput) || !Directory.Exists(userInput))
            {
                Console.WriteLine("Invalid directory. Please enter a valid installation directory:");
                userInput = Console.ReadLine()!;
            }

            extractPath = userInput;
        }

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

            Console.ForegroundColor = ConsoleColor.White;
            // Delete BepInEx folder, and two other files if they exist
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
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Extracting mods...\r\n");
            Thread.Sleep(1000);
            ZipFile.ExtractToDirectory("Lethal Company.zip", extractPath!);
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mods extracted...\r\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Deleting downloaded zip...\r\n");
            Thread.Sleep(1000);
            File.Delete("Lethal Company.zip");
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("File deleted...\r\n");

            // Display success message
            Menu();
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
}
