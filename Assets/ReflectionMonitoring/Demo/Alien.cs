using UnityEngine;

namespace ReflectionMonitoring.Demo
{
    public class Alien : ITalk
    {
        public void Talk(string sentence)
        {
            Debug.Log($"Alien talking...: {sentence}");
        }
    }
}