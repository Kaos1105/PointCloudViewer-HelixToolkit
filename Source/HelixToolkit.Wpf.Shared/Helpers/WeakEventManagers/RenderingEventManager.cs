// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderingEventManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a weak event manager for the CompositionTarget.Rendering event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media;

    /// <summary>
    /// Represents a weak event manager for the CompositionTarget.Rendering event.
    /// </summary>
    public class RenderingEventManager : WeakEventManagerBase<RenderingEventManager>
    {
        /// <summary>
        /// Start listening to the CompositionTarget.Rendering event.
        /// </summary>
        protected override void StartListening()
        {
            CompositionTarget.Rendering += this.Handler;
        }

        /// <summary>
        /// Stop listening to the CompositionTarget.Rendering event.
        /// </summary>
        protected override void StopListening()
        {
            CompositionTarget.Rendering -= this.Handler;
        }
    }
}