
using UnityEngine;

public class Enemy 
{
    [Inject] public Gun Weapon { get; set; }

    [OptionalInject] private string _id;
    
    public void Print() => Debug.Log($"Weapon: {Weapon}, ID: {_id}");
}

public class Gun { }
