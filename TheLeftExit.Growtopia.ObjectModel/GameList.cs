using System.ComponentModel;
using System.Collections.Generic;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using System.Collections;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct GameList<T> : IEnumerable<T> where T : unmanaged {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public GameList(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public int Count => Source.ForceRead<int>(Address + 0x08);

        public IEnumerator<T> GetEnumerator() {
            int count = Count;
            ulong currentNode = Source.ForceRead<ulong>(Address);
            for(int i = 0; i < count; i++) {
                currentNode = Source.ForceRead<ulong>(currentNode);
                yield return Source.ForceRead<T>(currentNode + 0x10);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
