using System;
using System.IO;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class SimpleReadWriter : MonoBehaviour
{
    private string filePath;
    void Start()
    {
        filePath = Path.Combine(Application.dataPath+LoadConstants.JSON_FILE_PATH, LoadConstants.JSON_FILE_NAME); 
        WeatherForecast forecast = new WeatherForecast
        {
            Date = DateTime.Parse("2020-02-29 00:00:00"),
            TemperatureC = 10,
            Summary = "Cold"
        };
        LoadAndReadFileAsync(forecast);
    }


    private async UniTask LoadAndReadFileAsync(WeatherForecast forecast)
    {
        await WriteJsonAsync(forecast);

        // Асинхронное чтение из файла
        UniTask<WeatherForecast> taskForecast = ReadJsonAsync();
        WeatherForecast loadedForecast = await taskForecast;
        if (loadedForecast != null)
        {
            Debug.Log($"Прочитали JSON: {loadedForecast.Summary}, Температура: {loadedForecast.TemperatureC}°C");
        }
    }

    private async UniTask WriteJsonAsync(WeatherForecast data)
    {
        if (File.Exists(filePath))
        {
            return;
        }

        try
        {
            await using (StreamWriter writer = new StreamWriter(filePath))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = new JsonSerializer();
                await UniTask.SwitchToThreadPool(); // Переключение на пул потоков
                serializer.Serialize(jsonWriter, data);
            }
            Debug.Log($"JSON сохранён в: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка записи JSON: {e.Message}");
        }
    }

    private async UniTask<WeatherForecast> ReadJsonAsync()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("Файл не найден!");
                return null;
            }

            using StreamReader reader = new StreamReader(filePath);
            using JsonTextReader jsonReader = new JsonTextReader(reader);
            JsonSerializer serializer = new JsonSerializer();
            await UniTask.SwitchToThreadPool(); // Переключение на пул потоков
            return serializer.Deserialize<WeatherForecast>(jsonReader);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка чтения JSON: {e.Message}");
            return null;
        }
    }
    
}


public class WeatherForecast
{
    public DateTimeOffset Date { get; set; }
    public int TemperatureC { get; set; }
    [CanBeNull] public string Summary { get; set; }
}