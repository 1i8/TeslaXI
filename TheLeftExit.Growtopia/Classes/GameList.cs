using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheLeftExit.Memory;

namespace TheLeftExit.Growtopia.Classes
{
    // << Growtopia's linked list specification >>
    // 0x00: pointer to header node
    // Node+0x00 - pointer to next node
    // Node+0x08 - pointer to prev node
    // Node+0x10 - node data (except for header)
    // 0x08: list length

    public class GameList<T>: GameClass, IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            Int64 currentNode = Handle.ReadInt64(Address);
            for (Int32 i = 0; i < Count; i++)
            {
                currentNode = Handle.ReadInt64(currentNode);
                yield return Constructor(Handle, currentNode + 0x10);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public GameStructConstructor<T> Constructor { get; init; }

        public Int32 Count => Handle.ReadInt32(Address + 0x08);
    }


}
