using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace ReflectionMonitoring.Demo.App
{
    
    [Serializable]
    public class PropertyBag
    {
        public string Address;
        public string Subject;
        public string Volume;
    }

    [Serializable]
    public class NetworkMonitorSettings
    {
        public string WarningService;
        public string MethodToExecute;
        public PropertyBag PropertyBag;
    }

    [Serializable]
    public class Config
    {
        public NetworkMonitorSettings NetworkMonitorSettings;
    }
    
    public static class NetworkMonitor
    {
        private static NetworkingMonitorSettings _networkingMonitorSettings = new NetworkingMonitorSettings();
        [CanBeNull] private static Type _warningServiceType;
        [CanBeNull] private static MethodInfo _warningServiceMethod;
        private static List<object> _warningServiceParameterValue = new List<object>();
        private static object _warningService;
        

        public static void LoadFromConfiguration()
        {
            string path = Application.dataPath + Constants.FILE_PATH;
            if (File.Exists(path))
            {
                StringBuilder builder = new StringBuilder();
                string str = File.ReadAllText(path);
                string[] lines = str.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].TrimStart().StartsWith("//"))
                    {
                        builder.Append(lines[i]);
                    }
                }

                string finalJsonConfigs = builder.ToString().TrimEnd();
                Config config = JsonUtility.FromJson<Config>(finalJsonConfigs);
                var service = CreateServiceInstance(config.NetworkMonitorSettings.WarningService);
                
                
                Debug.Log($"<color=green> Configs is exists {finalJsonConfigs}</color>");
            }
        }
        
        private static object CreateServiceInstance(string serviceName)
        {
            Debug.Log($"Service Name is {serviceName}Slerf");
            
            var assembly = Assembly.GetExecutingAssembly();
            string typeName = "ReflectionMonitoring.Demo.App." + serviceName;
            var type = assembly.CreateInstance(typeName, true, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, 
                null, null, null);
            Debug.Log($"<color=cyan> Type is {type}</color>");
            return type;
        }
        
        
    }
    
}