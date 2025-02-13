using UnityEngine;

public class Alien : ITalk
{
    public void Talk(string sentence)
    {
        Debug.Log($"Alien talking...: {sentence}");
    }
}