﻿using System;

namespace Akka.Actor
{
    /// <summary>
    /// Marker interface used to identify an object as ActorSystem extension
    /// </summary>
    public interface IExtension { }

    /// <summary>
    /// Non-generic version of interface, mostly to avoid issues with generic casting
    /// </summary>
    public interface IExtensionId
    {
        /// <summary>
        /// Returns an instance of the extension identified by this ExtensionId instance
        /// </summary>
        object Apply(ActorSystem system);

        /// <summary>
        /// Returns an instance of the extension identified by this <see cref="IExtensionId{T}"/> instance
        /// </summary>
        object Get(ActorSystem system);

        /// <summary>
        /// Is used by Akka to instantiate the <see cref="IExtension"/> identified by this ExtensionId.
        /// Internal use only.
        /// </summary>
        object CreateExtension(ActorSystem system);

        /// <summary>
        /// Returns the underlying type for this extension
        /// </summary>
        Type ExtensionType { get; }
    }

    /// <summary>
    /// Marker interface used to distinguish a unqiue ActorSystem extensions
    /// </summary>
    public interface IExtensionId<out T> : IExtensionId where T:IExtension
    {
        /// <summary>
        /// Returns an instance of the extension identified by this ExtensionId instance
        /// </summary>
        new T Apply(ActorSystem system);

        /// <summary>
        /// Returns an instance of the extension identified by this <see cref="IExtensionId{T}"/> instance
        /// </summary>
        new T Get(ActorSystem system);

        /// <summary>
        /// Is used by Akka to instantiate the <see cref="IExtension"/> identified by this ExtensionId.
        /// Internal use only.
        /// </summary>
        new T CreateExtension(ActorSystem system);
    }

    /// <summary>
    /// Static helper class used for resolving extensions
    /// </summary>
    public static class ExtendedActorSystem
    {
        public static T WithExtension<T>(this ActorSystem system) where T : IExtension
        {
            return (T)system.GetExtension<T>();
        }
    }

    /// <summary>
    ///     Class ExtensionBase.
    /// </summary>
    public abstract class ExtensionIdProvider<T> : IExtensionId<T> where T:IExtension
    {
        public T Apply(ActorSystem system)
        {
            return (T)system.RegisterExtension(this);
        }

        object IExtensionId.Get(ActorSystem system)
        {
            return Get(system);
        }

        object IExtensionId.CreateExtension(ActorSystem system)
        {
            return CreateExtension(system);
        }

        public Type ExtensionType
        {
            get { return typeof (T); }
        }

        object IExtensionId.Apply(ActorSystem system)
        {
            return Apply(system);
        }

        public T Get(ActorSystem system)
        {
            return (T)system.GetExtension(this);
        }

        public abstract T CreateExtension(ActorSystem system);

        public override bool Equals(object obj)
        {
            return obj is T;
        }

        public override int GetHashCode()
        {
            return typeof (T).GetHashCode();
        }
    }
}
