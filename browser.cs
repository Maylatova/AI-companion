using System;
using System.Diagnostics;
using System.Net;

namespace AICompanion
{
    public enum PlatformForQuery
    {
        Google,
        Youtube,
        TikTok,
        Twitch,
        Rozetka,
        Instagram,
        Discord,
        Steam,
        Wikipedia
    }
    public class SearchWebQuery
    {
        public void Search(string query, PlatformForQuery platform)
        {
            string encodedQuery = WebUtility.UrlEncode(query);
            string url = "";

            switch (platform)
            {
                case PlatformForQuery.Google:
                    url = $"https://www.google.com/search?q={encodedQuery}";
                    break;
                case PlatformForQuery.Youtube:
                    url = $"https://www.youtube.com/results?search_query={encodedQuery}";
                    break;
                case PlatformForQuery.TikTok:
                    url = $"https://www.tiktok.com/search?q={encodedQuery}";
                    break;
                case PlatformForQuery.Twitch:
                    url = $"https://www.twitch.tv/search?term={encodedQuery}";
                    break;
                case PlatformForQuery.Rozetka:
                    url = $"https://rozetka.com.ua/search/?text={encodedQuery}";
                    break;
                case PlatformForQuery.Instagram:
                    url = $"https://www.instagram.com/explore/tags/{encodedQuery}/";
                    break;
                case PlatformForQuery.Discord:
                    url = "https://discord.com/";
                    break;
                case PlatformForQuery.Steam:
                    url = $"https://store.steampowered.com/search/?term={encodedQuery}";
                    break;
                case PlatformForQuery.Wikipedia:
                    url = $"https://en.wikipedia.org/wiki/Special:Search?search={encodedQuery}";
                    break;
                default:
                    Console.WriteLine("Невідома платформа.");
                    return;
            }
            OpenUrl(url);
        }
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception error)
            {
                Console.WriteLine("Помилка відкриття браузера: " + error.Message);
            }
        }
    }
}
