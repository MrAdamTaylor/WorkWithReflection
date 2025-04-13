using UnityEngine;

namespace Reflection_and_ExpressionTree.MiniDI
{
    public class Player : MonoBehaviour
    {
        private Weapon _weapon;

        public void Construct(Weapon weapon)
        {
            _weapon = weapon;
            Debug.Log("Weapon injected!");
        }

        public void Fire() => _weapon.Attack();
    }
}
