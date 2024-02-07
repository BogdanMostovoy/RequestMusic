using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

public class Album
{
    public string nameAlbom { get; set; }
    public string ReleaseDate { get; set; }
    public string artworkUrl { get; set; }
}

public class ArtistAlbums
{
    public List<Album> Albums { get; set; }
}

public class MusicApp
{
    private const string CacheFilePath = "/cache.json";
    private const string ApiUrl = "https://api.musicserver.com/albums?artist=";

    public static void Main()
    {
        Console.WriteLine("Введите имя исполнителя:");
        string artistName = Console.ReadLine();

        List<Album> albums = GetAlbumsFromApi(artistName);

        if (albums.Count > 0)
        {
            SaveAlbumsToCache(albums);
            PrintAlbums(albums);
        }
        else
        {
            albums = GetAlbumsFromCache();
            PrintAlbums(albums);
        }

        Console.ReadLine();
    }

    private static List<Album> GetAlbumsFromApi(string artistName)
    {
        List<Album> albums = new List<Album>();

        try
        {
            string apiUrl = ApiUrl + artistName;
            string json = GetApiResponse(apiUrl);

            if (!string.IsNullOrEmpty(json))
            {
                ArtistAlbums artistAlbums = JsonConvert.DeserializeObject<ArtistAlbums>(json);
                albums = artistAlbums.Albums;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при получении данных из API: " + ex.Message);
        }

        return albums;
    }

    private static string GetApiResponse(string apiUrl)
    {
        using (WebClient client = new WebClient())
        {
            return client.DownloadString(apiUrl);
        }
    }

    private static void SaveAlbumsToCache(List<Album> albums)
    {
        string json = JsonConvert.SerializeObject(albums);
        File.WriteAllText(CacheFilePath, json);
    }

    private static List<Album> GetAlbumsFromCache()
    {
        if (File.Exists(CacheFilePath))
        {
            string json = File.ReadAllText(CacheFilePath);
            return JsonConvert.DeserializeObject<List<Album>>(json);
        }

        return new List<Album>();
    }

    private static void PrintAlbums(List<Album> albums)
    {
        if (albums.Count == 0)
        {
            Console.WriteLine("Нет доступных альбомов");
            return;
        }

        Console.WriteLine("Список альбомов:");
        foreach (Album album in albums)
        {
            Console.WriteLine(album.nameAlbom);
        }
    }
}
