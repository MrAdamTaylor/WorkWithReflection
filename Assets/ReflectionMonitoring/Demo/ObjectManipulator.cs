using System;
using System.Reflection;
using UnityEngine;

namespace ReflectionMonitoring.Demo
{
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

            var person5 = Activator.CreateInstance(typeof(Person), BindingFlags.Instance | BindingFlags.Public, 
                null, new object[] {"Kevin"}, null);

            var assembly = Assembly.GetExecutingAssembly();
            var person8 = assembly.CreateInstance("ReflectionMonitoring.Demo.Person", true, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, 
                new object[] {"Lediberd", 60}, null, null);

            #region Create with methods GetPropery and GetField
            Debug.Log($"Person name before Reflection - {person8}");
            var nameProperty = person8?.GetType().GetProperty("Name");
            nameProperty?.SetValue(person8,"Sergey");


            var ageField = person8.GetType().GetField("_aPrivateField", 
                BindingFlags.Instance | BindingFlags.NonPublic);
            ageField?.SetValue(person8, "Private Field");
            #endregion
        
        
            #region CreateObjectByInterface

            var actualTypeFromConfig = Type.GetType(GetTypeFromConfiguration());
            var iTalkInstance = Activator.CreateInstance(actualTypeFromConfig) as ITalk;
            iTalkInstance.Talk("Hello man!");
        
            #endregion
        
            #region InvokeMember

            person5?.GetType().InvokeMember("Name", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, 
                person5, new[] {"Emma"});

            #endregion

            #region Methods

            var talkMethod = person8?.GetType().GetMethod("Talk");
            talkMethod?.Invoke(person8, new[] { "Invoking: Talk in parallel" });

            person5?.GetType().InvokeMember("Yell",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, person5,
                new[] { "Have a constructive dialogue" });

            #endregion
        
            Debug.Log($"New Person8 a {person5}");
            Debug.Log($"New Person5 a {person5}");
        
            Debug.Log("Finish!");
        }


        static string GetTypeFromConfiguration()
        {
            return "ReflectionMonitoring.Demo.Person";
        }
    }
}
