using System;
using JetBrains.Annotations;
using UnityEngine;

public class SimpleReadWriter : MonoBehaviour
{

    void Start()
    {
        WeatherForecast forecast = new WeatherForecast
        {
            Date = DateTime.Parse("2020-02-29 00:00:00"),
            TemperatureC = 10,
            Summary = "Cold"
        };
    }

}


public class WeatherForecast
{
    public DateTimeOffset Date { get; set; }
    public int TemperatureC { get; set; }
    [CanBeNull] public string Summary { get; set; }
}