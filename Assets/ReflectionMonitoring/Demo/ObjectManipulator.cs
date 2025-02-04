using System;
using System.Reflection;
using ReflectionMonitoring.Demo;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    public void Start()
    {
        var personType = typeof(Person);
        var personConstructors = 
            personType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var constructor in personConstructors)
        {
            Debug.Log($"Constructor: {constructor}");
        }

        var privatePersonConstructor = personType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public, null, 
            new Type[] { typeof(string), typeof(int)}, null);

        var person1 = personConstructors[0].Invoke(null);
        var person2 = personConstructors[1].Invoke(new object[] {"Kevin"});
        var person3 = personConstructors[2].Invoke(new object[] {"Kevin", 40});

        var person4 = Activator.CreateInstance(typeof(Person));

        var person5 = Activator.CreateInstance(typeof(Person), BindingFlags.Instance | BindingFlags.Public, null, new object[] {"Kevin"}, null);

        var assembly = Assembly.GetExecutingAssembly();
        var person8 = assembly.CreateInstance("ReflectionMonitoring.Demo.Person", true, 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, 
            new object[] {"Lediberd", 60}, null, null);
        
        Debug.Log("Finish!");
    }
}
