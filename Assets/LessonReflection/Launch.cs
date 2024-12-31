using System.Reflection;
using UnityEngine;

public class Launch : MonoBehaviour
{
    
    
    private void Start()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var typesFromCurrentAssembly = currentAssembly.GetTypes();
        foreach (var type in typesFromCurrentAssembly)
        {
            Debug.Log(type.Name);
        }

        var typePerson = currentAssembly.GetType("Person");
        Debug.Log($"Current type {typePerson}!");
    }
}