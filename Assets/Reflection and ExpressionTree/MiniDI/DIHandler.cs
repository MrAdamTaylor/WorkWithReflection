using UnityEngine;

namespace Reflection_and_ExpressionTree.MiniDI
{
    public class DIHandler : MonoBehaviour
    {
        void Start()
        {
            var container = new MiniDIContainer ();

            container.Register(new Weapon());
            container.Register(new Player());
            container.Register<string>("Enemy_001");
            container.Register(new Gun());
            container.Register(new Enemy());

            container.InjectConstruct<Player>();
            container.InjectFieldsAndProperties<Enemy>();
            

            var player = container.Resolve<Player>();
            player.Fire();

            var enemy = container.Resolve<Enemy>();
            enemy.Print();
        }

        
    }
}
