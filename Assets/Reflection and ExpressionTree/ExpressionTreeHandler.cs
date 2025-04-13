using System;
using System.Linq.Expressions;
using UnityEngine;

public class ExpressionTreeHandler : MonoBehaviour
{
    
    void Start()
    {
        var player = new Player();
        var weapon = new Weapon();

        var inject = DiContainer.CreateInjectMethod();
        inject(player,weapon);
    }
}

public static class DiContainer
{
    public static Action<Player, Weapon> CreateInjectMethod()
    {
        var playerParam = Expression.Parameter(typeof(Player), "player");
        var weaponParam = Expression.Parameter(typeof(Weapon), "weapon");

        var method = typeof(Player).GetMethod("Construct");

        var call = Expression.Call(playerParam, method, weaponParam);
        return Expression.Lambda<Action<Player, Weapon>>(call, playerParam, weaponParam).Compile();
    }
}