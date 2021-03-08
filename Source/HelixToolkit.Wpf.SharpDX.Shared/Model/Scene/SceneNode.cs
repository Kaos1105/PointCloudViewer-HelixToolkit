﻿namespace HelixToolkit.Wpf.SharpDX.Model.Scene
{
#if !COREWPF
    public partial class SceneNode
    {
        public static implicit operator Element3D(SceneNode node)
        {
            return node.WrapperSource as Element3D;
        }
    }
#endif
}
