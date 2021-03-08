﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;

        public class ScreenQuadNode : SceneNode
        {
            /// <summary>
            /// Gets or sets the texture.
            /// </summary>
            /// <value>
            /// The texture.
            /// </value>
            public TextureModel Texture
            {
                set { (RenderCore as DrawScreenQuadCore).Texture = value; }
                get { return (RenderCore as DrawScreenQuadCore).Texture; }
            }
            /// <summary>
            /// Gets or sets the sampler.
            /// </summary>
            /// <value>
            /// The sampler.
            /// </value>
            public SamplerStateDescription Sampler
            {
                set { (RenderCore as DrawScreenQuadCore).SamplerDescription = value; }
                get { return (RenderCore as DrawScreenQuadCore).SamplerDescription; }
            }

            private float depth = 1f;
            public float Depth
            {
                set
                {
                    if(SetAffectsRender(ref depth, value))
                    {
                        var core = RenderCore as DrawScreenQuadCore;
                        core.ModelStruct.TopLeft.Z = core.ModelStruct.TopRight.Z = core.ModelStruct.BottomLeft.Z = core.ModelStruct.BottomRight.Z = value;
                    }
                }
                get { return depth; }
            }

            public ScreenQuadNode()
            {
                IsHitTestVisible = false;
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return new DrawScreenQuadCore();
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return EffectsManager[DefaultRenderTechniqueNames.ScreenQuad];
            }

            public sealed override bool HitTest(IRenderMatrices context, Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(IRenderMatrices context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}
