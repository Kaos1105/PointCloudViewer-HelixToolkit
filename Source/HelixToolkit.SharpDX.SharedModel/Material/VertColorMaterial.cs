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
    /// Render color by mesh vertex color
    /// </summary>
    public sealed class VertColorMaterial : Material
    {
        protected override MaterialCore OnCreateCore()
        {
            return ColorMaterialCore.Core;
        }

        public VertColorMaterial() { }

        public VertColorMaterial(ColorMaterialCore core) : base(core) { }
#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new VertColorMaterial()
            {
                Name = Name
            };
        }
#endif
    }
}
