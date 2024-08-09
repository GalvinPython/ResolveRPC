using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace ResolveRPC
{
    internal class Program
    {
        private static DiscordRpcClient client;
        private static bool clientIsInitialised = false;
        private static string processDetails;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting ResolveRPC...");

            while (true)
            {
                await Task.Delay(1000); // Delay for 1 second

                GetResolveProjectName();

                // Check if "DaVinci Resolve" is running
                var processes = Process.GetProcessesByName("Resolve");

                if (processes.Length > 0 && !clientIsInitialised)
                {
                    // Initialize Discord RPC
                    Initialize();
                    clientIsInitialised = true;
                }
                else if (processes.Length == 0 && clientIsInitialised)
                {
                    // If DaVinci Resolve is no longer running, clear the presence and reset
                    client.ClearPresence();
                    clientIsInitialised = false;
                    Console.WriteLine("DaVinci Resolve stopped. Resetting RPC.");
                }
                else if (clientIsInitialised)
                {
                    // Update presence with new project details
                    client.SetPresence(new RichPresence()
                    {
                        Details = processDetails != null ? "In Project": "Not in project!",
                        State = processDetails,
                        Assets = new Assets()
                        {
                            LargeImageKey = "https://cdn.discordapp.com/app-icons/1271179616136069154/707072511b8b9943f53ce066db754993.png?size=256",
                            LargeImageText = "DaVinci Resolve",
                        }
                    });
                }
            }
        }

        private static void Initialize()
        {
            client = new DiscordRpcClient("1271179616136069154");
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };
            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "Not in project!",
                State = processDetails,
                Assets = new Assets()
                {
                    LargeImageKey = "https://cdn.discordapp.com/app-icons/1271179616136069154/707072511b8b9943f53ce066db754993.png?size=256",
                    LargeImageText = "DaVinci Resolve",
                }
            });

            Console.WriteLine("Initialized Discord RPC.");
        }

        private static void GetResolveProjectName()
        {
            var windowTitles = GetAllWindowTitles();
            var projectNames = new List<string>();

            foreach (var title in windowTitles)
            {
                if (title.Contains("DaVinci Resolve -"))
                {
                    string projectName = title.Replace("DaVinci Resolve - ", "").Trim();
                    if (!string.IsNullOrEmpty(projectName))
                    {
                        projectNames.Add(projectName);
                    }
                }
            }

            processDetails = string.Join(", ", projectNames);
        }

        private static List<string> GetAllWindowTitles()
        {
            var titles = new List<string>();
            EnumWindows((hwnd, lParam) =>
            {
                var sb = new StringBuilder(256);
                if (GetWindowText(hwnd, sb, sb.Capacity) > 0)
                {
                    titles.Add(sb.ToString());
                }
                return true;
            }, IntPtr.Zero);

            return titles;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
    }
}
