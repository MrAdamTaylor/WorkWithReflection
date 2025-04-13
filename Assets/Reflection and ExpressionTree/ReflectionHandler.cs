using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionHandler : MonoBehaviour
{
    void Start()
    {
        var knight = new Knight();
        var knife = new Knife();
        
        var method = typeof(Knight).GetMethod("Construct");
        method.Invoke(knight, new object[] {knife});
    }
    
}