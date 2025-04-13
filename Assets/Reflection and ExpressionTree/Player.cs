using UnityEngine;

public class Player
{
    private Weapon _weapon;

    public void Construct(Weapon weapon)
    {
        _weapon = weapon;
        Debug.Log($"Weapon injected via expression tree {_weapon}");
    }
}