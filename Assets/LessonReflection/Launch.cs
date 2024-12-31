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
            foreach (var method in type.GetMethods())
            {
                Debug.Log($"Method of type {type.Name} a {method}");
            }
        }

        var typePerson = currentAssembly.GetType("Person");
        Debug.Log($"Current type {typePerson}!");

        foreach (var constructor in typePerson.GetConstructors())
        {
            Debug.Log($"Is Person Constructor: {constructor}");
        }

        foreach (var info in typePerson.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
        {
            Debug.Log($"Is Person NoPublic: {info}");
        }
    }
}