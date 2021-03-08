﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
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
    namespace Model
    {
        using Render;
        using Shaders;       
        using Utilities;

        public abstract class GenericMaterialVariable : MaterialVariable
        {
            public enum ResourceType
            {
                Texture, Sampler, Float, Vector2, Vector3, Vector4, Matrix
            }
            private readonly ShaderPass materialPass, shadowPass, wireframePass, depthPass;


            private readonly KeyValuePair<int, ShaderResourceViewProxy>[] shaderResources;
            private readonly KeyValuePair<int, SamplerStateProxy>[] samplerResources;

            private readonly Dictionary<string, int> resourceIdxDict = new Dictionary<string, int>();
            private readonly Dictionary<string, int> samplerIdxDict = new Dictionary<string, int>();
            private readonly GenericMaterialCore materialCore;

            public GenericMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
                GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
                string materialShaderPassName = DefaultPassNames.Default,
                string shadowShaderPassName = DefaultPassNames.ShadowPass,
                string wireframePassName = DefaultPassNames.Wireframe,
                string depthPassName = DefaultPassNames.DepthPrepass)
                : base(manager, technique, constantBufferDescription, materialCore)
            {
                this.materialCore = materialCore;
                materialPass = technique[materialShaderPassName];
                shadowPass = technique[shadowShaderPassName];
                wireframePass = technique[wireframePassName];
                depthPass = technique[depthPassName];
                shaderResources = new KeyValuePair<int, ShaderResourceViewProxy>[materialPass.PixelShader.ShaderResourceViewMapping.Count];

                for(int i=0; i < materialPass.PixelShader.ShaderResourceViewMapping.Count; ++i)
                {
                    var mapping = materialPass.PixelShader.ShaderResourceViewMapping.Mappings[i];
                    resourceIdxDict.Add(mapping.Value.Description.Name, i);
                    shaderResources[i] = new KeyValuePair<int, ShaderResourceViewProxy>(mapping.Key, null);
                }

                samplerResources = new KeyValuePair<int, SamplerStateProxy>[materialPass.PixelShader.SamplerMapping.Count];

                for(int i=0; i < materialPass.PixelShader.SamplerMapping.Count; ++i)
                {
                    var mapping = materialPass.PixelShader.SamplerMapping.Mappings[i];
                    samplerIdxDict.Add(mapping.Value.Name, i);
                    samplerResources[i] = new KeyValuePair<int, SamplerStateProxy>(mapping.Key, null);
                }


                materialCore.UpdatingResource += MaterialCore_UpdatingResource;
            }

            protected override void OnInitialPropertyBindings()
            {
                base.OnInitialPropertyBindings();
                foreach (var texture in materialCore.TextureDict)
                {
                    SetTexture(texture.Key, texture.Value);
                }
                foreach (var sampler in materialCore.SamplerDict)
                {
                    SetSampler(sampler.Key, sampler.Value);
                }

                foreach (var prop in materialCore.FloatDict)
                {
                    WriteValue(prop.Key, prop.Value);
                }
                foreach (var prop in materialCore.Vector2Dict)
                {
                    WriteValue(prop.Key, prop.Value);
                }
                foreach (var prop in materialCore.Vector3Dict)
                {
                    WriteValue(prop.Key, prop.Value);
                }
                foreach (var prop in materialCore.Vector4Dict)
                {
                    WriteValue(prop.Key, prop.Value);
                }
                foreach (var prop in materialCore.MatrixDict)
                {
                    WriteValue(prop.Key, prop.Value);
                }
            }

            private void MaterialCore_UpdatingResource(object sender, GenericMaterialCore.UpdateEvent e)
            {
                switch (e.Type)
                {
                    case ResourceType.Sampler:
                        SetSampler(e.Name, materialCore.GetSampler(e.Name));
                        break;
                    case ResourceType.Texture:
                        SetTexture(e.Name, materialCore.GetTexture(e.Name));
                        break;
                    case ResourceType.Float:
                        WriteValue(e.Name, materialCore.FloatDict[e.Name]);
                        break;
                    case ResourceType.Vector2:
                        WriteValue(e.Name, materialCore.Vector2Dict[e.Name]);
                        break;
                    case ResourceType.Vector3:
                        WriteValue(e.Name, materialCore.Vector3Dict[e.Name]);
                        break;
                    case ResourceType.Vector4:
                        WriteValue(e.Name, materialCore.Vector4Dict[e.Name]);
                        break;
                    case ResourceType.Matrix:
                        WriteValue(e.Name, materialCore.MatrixDict[e.Name]);
                        break;
                }
            }

            public bool SetTexture(string name, TextureModel texture)
            {
                if(resourceIdxDict.TryGetValue(name, out var idx))
                {
                    var exist = shaderResources[idx].Value;
                    RemoveAndDispose(ref exist);
                    if (texture == null)
                    {
                        shaderResources[idx] = new KeyValuePair<int, ShaderResourceViewProxy>(shaderResources[idx].Key, null);
                    }
                    else
                    {
                        var res = Collect(EffectsManager.MaterialTextureManager.Register(texture));
                        shaderResources[idx] = new KeyValuePair<int, ShaderResourceViewProxy>(shaderResources[idx].Key, res);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool SetSampler(string name, global::SharpDX.Direct3D11.SamplerStateDescription sampler)
            {
                if (resourceIdxDict.TryGetValue(name, out var idx))
                {
                    var exist = samplerResources[idx].Value;
                    RemoveAndDispose(ref exist);
                    var res = Collect(EffectsManager.StateManager.Register(sampler));
                    samplerResources[idx] = new KeyValuePair<int, SamplerStateProxy>(samplerResources[idx].Key, res);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
            {
                foreach(var res in shaderResources)
                {
                    deviceContext.SetShaderResource(PixelShader.Type, res.Key, res.Value);
                }
                return true;
            }

            public override ShaderPass GetPass(RenderType renderType, RenderContext context)
            {
                return materialPass;
            }

            public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
            {
                return shadowPass;
            }

            public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
            {
                return wireframePass;
            }

            public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
            {
                return depthPass;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                {
                    materialCore.UpdatingResource -= MaterialCore_UpdatingResource;
                }
                base.OnDispose(disposeManagedResources);
            }
        }

        public sealed class GenericMeshMaterialVariable : GenericMaterialVariable
        {
            public GenericMeshMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
                GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
                string materialShaderPassName = DefaultPassNames.Default,
                string shadowShaderPassName = DefaultPassNames.ShadowPass,
                string wireframePassName = DefaultPassNames.Wireframe)
                :base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, wireframePassName)
            {

            }

            public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
            {
                DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
            }
        }

        public sealed class GenericPointMaterialVariable : GenericMaterialVariable
        {
            public GenericPointMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
                GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
                string materialShaderPassName = DefaultPassNames.Default,
                string shadowShaderPassName = DefaultPassNames.ShadowPass)
                : base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, "")
            {

            }

            public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
            {
                DrawPoints(deviceContext, bufferModel.VertexBuffer[0].ElementCount, instanceCount);
            }
        }
    }

}
