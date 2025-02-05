using UnityEngine;

namespace ReflectionMonitoring.Demo.App
{
    public class MailService 
    {
        public MailService()
        {
            
        }

        public void SendMail(string data)
        {
            Debug.Log($" Send mail data {data}");
        }
    }
}
