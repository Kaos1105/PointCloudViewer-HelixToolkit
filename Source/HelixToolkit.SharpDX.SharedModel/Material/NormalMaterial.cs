﻿#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.ComponentModel;
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    /// <summary>
    /// Render color by triangle normal
    /// </summary>
    public sealed class NormalMaterial : Material
    {
        public NormalMaterial() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalMaterial"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        public NormalMaterial(NormalMaterialCore core) : base(core)
        {

        }
        /// <summary>
        /// Called when [create core].
        /// </summary>
        /// <returns></returns>
        protected override MaterialCore OnCreateCore()
        {
            return NormalMaterialCore.Core;
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new NormalMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
