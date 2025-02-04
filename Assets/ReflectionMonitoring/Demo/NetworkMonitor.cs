using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace ReflectionMonitoring.Demo
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


        public static void LoadFromConfiguration()
        {
            string path = Application.dataPath + "/ReflectionMonitoring/Data/appsettings.json";
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
                CreateServiceInstance(config.NetworkMonitorSettings.WarningService);
                Debug.Log($"<color=green> Configs is exists {finalJsonConfigs}</color>");
            }
        }
        
        private static void CreateServiceInstance(string serviceName)
        {
            Debug.Log($"Service Name is {serviceName}");
            Type serviceType = Type.GetType(serviceName);
        
            if (serviceType != null)
            {
                object serviceInstance = Activator.CreateInstance(serviceType);
                Debug.Log($"<color=cyan>Создан экземпляр сервиса: {serviceInstance}</color>");
            }
            else
            {
                Debug.LogError($"Сервис '{serviceName}' не найден.");
            }
        }
    }
    
    
}