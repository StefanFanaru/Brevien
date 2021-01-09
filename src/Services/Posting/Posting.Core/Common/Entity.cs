using System;

namespace Posting.Core.Common
{
    public class Entity<T>
    {
        public Entity(T id)
        {
            if (Equals(id, default(T)))
            {
                throw new ArgumentException("The ID cannot be the type's default value.", "id");
            }

            Id = id;
        }

        public Entity()
        {
        }

        public T Id { get; protected set; }
    }
}
