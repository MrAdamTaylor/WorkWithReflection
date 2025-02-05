using UnityEngine;

namespace ReflectionMonitoring.Demo.App
{
    public class SoundHornService 
    {
        public SoundHornService()
        {
            
        }

        public void SoundHorn(string data)
        {
            Debug.Log($" Send horn data {data}");
        }
    }
}
