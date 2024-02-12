using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedPayload
{
    public class GameObjectDirectory
    {
        private object _sboxInstance;
        public GameObjectDirectory( object sboxInstance )
        {
            _sboxInstance = sboxInstance;
        }
        public void Add( object gameObject )
        {
            _sboxInstance.GetType().GetMethod("Add").Invoke(_sboxInstance, new object[] { gameObject });
        }
        public void Remove( object gameObject )
        {
            _sboxInstance.GetType().GetMethod("Remove").Invoke(_sboxInstance, new object[] { gameObject });
        }
    }
}
