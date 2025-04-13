using UnityEngine;

public class Knight
{
    private Knife _knife;

    public void Construct(Knife knife)
    {
        _knife = knife;
        Debug.Log($"Knife injected via reflection {_knife}");
    }
}