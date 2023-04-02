using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookGenerator.Domain.Primitives
{
    /// <summary>
    /// Represents the base class that all entities derive from.
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        protected Entity(Guid id)
            : this()
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The Id cannot be empty.", nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <remarks>
        /// Required by EF Core.
        /// </remarks>
        protected Entity()
        {
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public Guid Id { get; private init; }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b) => !(a == b);

        /// <inheritdoc />
        public bool Equals(Entity other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (!(obj is Entity other))
            {
                return false;
            }

            if (Id == Guid.Empty || other.Id == Guid.Empty)
            {
                return false;
            }

            return Id == other.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id.GetHashCode() * 41;
    }
}
