using System;
using UnityEditor;
using UnityEngine;

namespace Sim.Dispositio.Shared
{
    /// <summary>
    /// A <see cref="Guid"/> that can be serialized by Unity
    /// </summary>
    [Serializable]
    public class SerializableGuid : IEquatable<SerializableGuid>, IEquatable<Guid>
    {
        [SerializeField, HideInInspector]
        private string _guidString;

        private Maybe<Guid> _guid = Maybe.None<Guid>();

        private int _cashedHashCode = -1;

        /// <summary>
        /// The <see cref="Guid"/> to be de/serialized.
        /// </summary>
        public Guid Instance
        {
            get
            {
                if (!_guid.IsSome)
                {
                    var guid = string.IsNullOrEmpty(_guidString)
                        ? Guid.Empty
                        : Guid.Parse(_guidString);

                    _guid = Maybe.Some(guid);
                }

                return _guid.Value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SerializableGuid()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The object to copy from.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="other"/> is null.
        /// </exception>
        public SerializableGuid(SerializableGuid other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            _guidString = other._guidString;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The serialized property to copy from.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="other"/> does not represent a <see cref="SerializableGuid"/>.
        /// </exception>
        public SerializableGuid(UnityEditor.SerializedProperty other)
        {
            if (other.type != nameof(SerializableGuid))
            {
                throw new ArgumentException($"{nameof(other)} param must by of type \"{nameof(SerializableGuid)}\", " +
                                            $"but is of type \"{other.type}\"");
            }

            var otherString = other.FindPropertyRelative(nameof(_guidString));
            _guidString = otherString.stringValue;
            _guid = Maybe.None<Guid>();
        }
#endif


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="guid">A GUID this one should equal.</param>
        public SerializableGuid(Guid guid)
        {
            _guidString = guid.ToString();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="g">A string that contains a GUID in a valid format. <seealso cref="Guid"/></param>
        public SerializableGuid(string g)
        {
            _guidString = g;
        }

        public void SetGuid(Guid guid)
        {
            _guidString = guid.ToString();
            _guid = Maybe.None<Guid>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Set <paramref name="guidProp"/> to <paramref name="other"/>'s value.
        /// </summary>
        /// <param name="guidProp">The property whose value to set.</param>
        /// <param name="other">The source Guid.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="guidProp"/> does not represent a <see cref="SerializableGuid"/>.
        /// </exception>
        public static void SetPropertyGuid(SerializedProperty guidProp, SerializableGuid other)
        {
            if (guidProp.type != nameof(SerializableGuid))
            {
                throw new ArgumentException($"{nameof(guidProp)} param must by of type \"{nameof(SerializableGuid)}\", " +
                                            $"but is of type \"{guidProp.type}\"");
            }

            guidProp.stringValue = (other?.Instance ?? Guid.Empty).ToString();
        }

        public static void SetPropertyGuid<T>(SerializedProperty guidProp, SerializableGuid other)
        {
            var type = guidProp.type;
            bool isReference = false;

            if (guidProp.type.Contains("<"))
            {
                type = type.SubstrAfterFirst('<').SubstrBeforeFirst('>');
                isReference = true;
            }

            if (type != typeof(T).Name && !string.IsNullOrEmpty(type))
            {
                throw new ArgumentException($"{nameof(guidProp)} param must by of type \"{nameof(T)}\", " +
                                            $"but is of type \"{guidProp.type}\"");
            }

            if (isReference)
            {
                guidProp.managedReferenceValue = other;
            }
            else
            {
                SetPropertyGuidInternal(guidProp, other);
            }
        }

        internal static void SetPropertyGuidInternal(SerializedProperty guidProp, SerializableGuid other)
        {
            var guidStringProperty = guidProp.FindPropertyRelative(nameof(_guidString));
            guidStringProperty.stringValue = (other?.Instance ?? Guid.Empty).ToString();
        }

        public static T GetPropertyGuid<T>(SerializedProperty property) where T : SerializableGuid, new()
        {
            var type = property.type;

            if (property.type.Contains("<"))
            {
                type = type.SubstrAfterFirst('<').SubstrBeforeFirst('>');
            }

            if (type != typeof(T).Name)
            {
                throw new ArgumentException($"{nameof(property)} param must by of type \"{typeof(T).Name}\", " +
                                            $"but is of type \"{property.type}\"");
            }

            return GetPropertyGuidInternal<T>(property);
        }

        internal static T GetPropertyGuidInternal<T>(SerializedProperty property) where T : SerializableGuid, new()
        {
            var guidStringProperty = property.FindPropertyRelative(nameof(_guidString));

            var instance = new T
            {
                 _guidString = guidStringProperty.stringValue,
            };
            return instance;
        }

        public static SerializableGuid GetPropertyGuid(UnityEditor.SerializedProperty property)
        {
            return new SerializableGuid(property);
        }
#endif

        /// <inheritdoc />
        public override string ToString()
        {
            return Instance.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (_cashedHashCode != -1)
            {
                return _cashedHashCode;
            }

            unchecked // Overflow is fine, just wrap.
            {
                // Choose large primes to avoid hashing collisions.
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ Instance.GetHashCode();
                _cashedHashCode = hash;
                return hash;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is SerializableGuid)
            {
                return Equals((SerializableGuid)obj);
            }
            else if (obj is Guid)
            {
                return Equals((Guid)obj);
            }

            return false;
        }

        /// <summary>
        /// <see cref="IEquatable{T}.Equals(T)"/> implementation.
        /// </summary>
        /// <param name="other">The object to compare to this one.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(SerializableGuid other)
        {
            // Check for the object to compare to being null.
            if (ReferenceEquals(other, null))
            {
                // Only the left side is null.
                return false;
            }

            return Instance == other.Instance;
        }

        /// <summary>
        /// <see cref="IEquatable{T}.Equals(T)"/> implementation.
        /// </summary>
        /// <param name="other">The object to compare to this one.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(Guid other)
        {
            return Instance == other;
        }

        /// <summary>
        /// Override ==
        /// </summary>
        public static bool operator ==(SerializableGuid lhs, SerializableGuid rhs)
        {
            // Check for null on left side.
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Override ==
        /// </summary>
        public static bool operator ==(SerializableGuid lhs, Guid rhs)
        {
            // Check for null on left side.
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Override ==
        /// </summary>
        public static bool operator ==(Guid lhs, SerializableGuid rhs)
        {
            return rhs == lhs;
        }

        /// <summary>
        /// Override !=
        /// </summary>
        public static bool operator !=(SerializableGuid lhs, SerializableGuid rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Override !=
        /// </summary>
        public static bool operator !=(SerializableGuid lhs, Guid rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Override !=
        /// </summary>
        public static bool operator !=(Guid lhs, SerializableGuid rhs)
        {
            return !(lhs == rhs);
        }
    }
}
