﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    namespace Core
    {
        using Model;
        using Render;

        /// <summary>
        /// 
        /// </summary>
        public class PointLineRenderCore : GeometryRenderCore, IMaterialRenderParams
        {
            private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
            /// <summary>
            /// Used to wrap all material resources
            /// </summary>
            public MaterialVariable MaterialVariables
            {
                set
                {
                    var old = materialVariables;
                    if (SetAffectsCanRenderFlag(ref materialVariables, value))
                    {
                        if (value == null)
                        {
                            materialVariables = EmptyMaterialVariable.EmptyVariable;
                        }
                    }
                }
                get
                {
                    return materialVariables;
                }
            }

            protected PointLineModelStruct modelStruct;

            protected virtual void OnUpdatePerModelStruct()
            {
                modelStruct.World = ModelMatrix;
                modelStruct.HasInstances = InstanceBuffer.HasElements ? 1 : 0;
            }

            protected override bool OnUpdateCanRenderFlag()
            {
                return base.OnUpdateCanRenderFlag() && materialVariables != EmptyMaterialVariable.EmptyVariable;
            }

            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceContext">The device context.</param>
            protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
            {
                var shaderPass = materialVariables.GetPass(RenderType, context);
                if (shaderPass.IsNULL)
                {
                    return;
                }
                OnUpdatePerModelStruct();
                if(!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, PointLineModelStruct.SizeInBytes))
                {
                    return;
                }
                if(materialVariables.BindMaterialResources(context, deviceContext, shaderPass))
                {
                    shaderPass.BindShader(deviceContext);
                    shaderPass.BindStates(deviceContext, DefaultStateBinding);
                    materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
                }
            }

            protected sealed override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
            {
                if(!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, PointLineModelStruct.SizeInBytes))
                {
                    return;
                }
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
            }

            protected sealed override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
            {
                var pass = materialVariables.GetShadowPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                var v = new SimpleMeshStruct()
                {
                    World = ModelMatrix,
                    HasInstances = InstanceBuffer.HasElements ? 1 : 0
                };
                if (!materialVariables.UpdateNonMaterialStruct(deviceContext, ref v, SimpleMeshStruct.SizeInBytes))
                {
                    return;
                }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, ShadowStateBinding); 
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
            }

            protected sealed override void OnRenderDepth(RenderContext context, DeviceContextProxy deviceContext, 
                Shaders.ShaderPass customPass)
            {
                var pass = customPass ?? materialVariables.GetDepthPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                OnUpdatePerModelStruct();
                if (!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, PointLineModelStruct.SizeInBytes))
                {
                    return;
                }
                if (materialVariables.BindMaterialResources(context, deviceContext, pass))
                {
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, DefaultStateBinding);
                    materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
                }
            }
        }
    }

}
