using UnityEngine;

namespace ReflectionMonitoring.Demo.App
{
    public class MonitoringApp : MonoBehaviour
    {
        private void Start()
        {
            NetworkMonitor.LoadFromConfiguration();
        }

        private void LoadMonitoringCode()
        {
        
        }
    }
}
