using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using TheLeftExit.Memory;
using TheLeftExit.Memory.Queries;

using System.Diagnostics;

namespace TheLeftExit.Growtopia.Classes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueriedNestedClassAttribute : Attribute { }

    // Native object model transcribed to C#. Example usage: Growtopia.App.GameLogicComponent.NetAvatar.Position. Neat, isn't it?
    // Yes, this spawns an inadequate amount of short-lived classes and kills GC if something nested is accessed directly too often.
    // I'm aware that this is not good performance-wise, and generally a terrible coding practice, but it's the compromise I'm making for ease of use.
    // Blame Jeffrey Richter for not advising CLR developers to add structure inheritance.
    public abstract class GameClass
    {
        public IntPtr Handle { get; init; }
        public Int64 Address { get; init; }

        protected T Offset<T>(Int32 offset, ref bool init, bool byValue = false) where T : GameClass, new()
        {
            T ret = new T
            {
                Handle = this.Handle,
                Address = byValue ? this.Address + offset : this.Handle.ReadInt64(this.Address + offset)
            };
            if (!init)
            {
                ret.FindMemberOffsets();
                init = true;
            }
            return ret;
        }

        protected GameList<T> OffsetList<T>(Int32 offset, ref bool init, GameStructConstructor<T> constructor, bool byValue = true)
        {
            GameList<T> ret = new GameList<T>
            {
                Handle = this.Handle,
                Address = byValue ? this.Address + offset : this.Handle.ReadInt64(this.Address + offset),
                Constructor = constructor
            };
            if (!init)
            {
                ret.FindMemberOffsets();
                init = true;
            }
            return ret;
        }

        // I could certainly feed a whole lot of stuff into Offset<T> functions instead of reflecting things.
        // Maybe even create a separate accessing/querying/caching class for each nested class member.
        // I'll look into that later, but right now it seems like something that'll make the code harder to manage.
        // Each class is only being reflected once per execution, so performance doesn't tank.
        protected void FindMemberOffsets()
        {
            Type gameClass = this.GetType();

            foreach(PropertyInfo p in gameClass.GetProperties())
            {
                QueriedNestedClassAttribute attr = p.GetCustomAttribute<QueriedNestedClassAttribute>();
                if (attr == null)
                    continue;

                FieldInfo queryField = gameClass.GetRuntimeFields().First(x => x.Name == p.Name + "Query");
                FieldInfo offsetField = gameClass.GetRuntimeFields().First(x => x.Name == p.Name + "Offset");
                if (queryField == null || offsetField == null)
                    throw new MissingFieldException($"{gameClass.Name}.{p.Name} has {typeof(QueriedNestedClassAttribute).Name}, but facilitating static fields were not found.");

                PointerQuery query = (PointerQuery)queryField.GetValue(null);
                PointerQueryResult queryResult = query.Run(Handle, Address);
                if (queryResult.Equals(PointerQueryResult.None))
                    throw new ProcessMemoryException($"Failed to query offset for {gameClass.Name}->{p.Name}.");

                offsetField.SetValue(null, queryResult.Offset);
            }
        }
    }
}
