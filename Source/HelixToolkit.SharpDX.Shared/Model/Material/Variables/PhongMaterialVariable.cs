﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.CompilerServices;
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
        using ShaderManager;
        using Shaders;
        using Utilities;

        /// <summary>
        /// Default PhongMaterial Variables
        /// </summary>
        public class PhongMaterialVariables : MaterialVariable
        {
            private const int NUMTEXTURES = 6;

            private const int DiffuseIdx = 0,
                AlphaIdx = 1,
                NormalIdx = 2,
                DisplaceIdx = 3,
                SpecularColorIdx = 4,
                EmissiveIdx = 5;

            private readonly ITextureResourceManager textureManager;
            private readonly IStatePoolManager statePoolManager;
            private readonly ShaderResourceViewProxy[] textureResources = new ShaderResourceViewProxy[NUMTEXTURES];
            private SamplerStateProxy surfaceSampler, displacementSampler, shadowSampler;

            private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot, texShadowSlot, texSpecularSlot, texEmissiveSlot;
            private int samplerDiffuseSlot, samplerDisplaceSlot, samplerShadowSlot;
            private uint textureIndex = 0;

            private bool HasTextures
            {
                get
                {
                    return textureIndex != 0;
                }
            }

            public ShaderPass MaterialPass { get; }
            public ShaderPass MaterialOITPass { get; }
            public ShaderPass ShadowPass { get; }
            public ShaderPass WireframePass { get; }
            public ShaderPass WireframeOITPass { get; }
            public ShaderPass TessellationPass { get; }
            public ShaderPass TessellationOITPass { get; }
            public ShaderPass DepthPass { get; }
            /// <summary>
            /// 
            /// </summary>
            public string ShaderAlphaTexName { get; } = DefaultBufferNames.AlphaMapTB;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderDiffuseTexName { get; } = DefaultBufferNames.DiffuseMapTB;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderNormalTexName { get; } = DefaultBufferNames.NormalMapTB;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderDisplaceTexName { get; } = DefaultBufferNames.DisplacementMapTB;
            /// <summary>
            /// Gets or sets the name of the shader shadow tex.
            /// </summary>
            /// <value>
            /// The name of the shader shadow tex.
            /// </value>
            public string ShaderShadowTexName { get; } = DefaultBufferNames.ShadowMapTB;
            /// <summary>
            /// Gets the shader specular texture.
            /// </summary>
            /// <value>
            /// The shader specular texture.
            /// </value>
            public string ShaderSpecularTexName { get; } = DefaultBufferNames.SpecularTB;
            /// <summary>
            /// Gets the name of the shader emissive tex.
            /// </summary>
            /// <value>
            /// The name of the shader emissive tex.
            /// </value>
            public string ShaderEmissiveTexName { get; } = DefaultBufferNames.EmissiveTB;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderSamplerDiffuseTexName { get; } = DefaultSamplerStateNames.SurfaceSampler;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderSamplerDisplaceTexName { get; } = DefaultSamplerStateNames.DisplacementMapSampler;
            /// <summary>
            /// 
            /// </summary>
            public string ShaderSamplerShadowMapName { get; } = DefaultSamplerStateNames.ShadowMapSampler;

            private bool enableTessellation = false;
            public bool EnableTessellation
            {
                private set
                {
                    if (Set(ref enableTessellation, value))
                    {
                        currentMaterialPass = value ? TessellationPass : MaterialPass;
                        UpdateMappings(currentMaterialPass);
                        currentOITPass = value ? TessellationOITPass : MaterialOITPass;
                        InvalidateRenderer();
                    }
                }
                get
                {
                    return enableTessellation;
                }
            }

            private readonly PhongMaterialCore material;
            private ShaderPass currentMaterialPass = ShaderPass.NullPass;
            private ShaderPass currentOITPass = ShaderPass.NullPass;

            /// <summary>
            /// Initializes a new instance of the <see cref="PhongMaterialVariables"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <param name="materialCore">The material core.</param>
            /// <param name="materialPassName">Name of the material pass.</param>
            /// <param name="wireframePassName">Name of the wireframe pass.</param>
            /// <param name="materialOITPassName">Name of the material oit pass.</param>
            /// <param name="wireframeOITPassName">Name of the wireframe oit pass.</param>
            /// <param name="shadowPassName">Name of the shadow pass.</param>
            /// <param name="tessellationPassName">Name of the tessellation pass.</param>
            /// <param name="tessellationOITPassName">Name of the tessellation oit pass.</param>
            /// <param name="depthPassName">Name of the depth pass</param>
            public PhongMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore materialCore,
                string materialPassName = DefaultPassNames.Default, string wireframePassName = DefaultPassNames.Wireframe,
                string materialOITPassName = DefaultPassNames.OITPass, string wireframeOITPassName = DefaultPassNames.WireframeOITPass,
                string shadowPassName = DefaultPassNames.ShadowPass,
                string tessellationPassName = DefaultPassNames.MeshTriTessellation,
                string tessellationOITPassName = DefaultPassNames.MeshTriTessellationOIT,
                string depthPassName = DefaultPassNames.DepthPrepass)
                : base(manager, technique, DefaultMeshConstantBufferDesc, materialCore)
            {
                this.material = materialCore;
                texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
                samplerDiffuseSlot = samplerDisplaceSlot = samplerShadowSlot = -1;
                textureManager = manager.MaterialTextureManager;
                statePoolManager = manager.StateManager;
            
                MaterialPass = technique[materialPassName];
                MaterialOITPass = technique[materialOITPassName];
                ShadowPass = technique[shadowPassName];
                WireframePass = technique[wireframePassName];
                WireframeOITPass = technique[wireframeOITPassName];
                TessellationPass = technique[tessellationPassName];
                TessellationOITPass = technique[tessellationOITPassName];
                DepthPass = technique[depthPassName];
                UpdateMappings(MaterialPass);
                EnableTessellation = materialCore.EnableTessellation;
                currentMaterialPass = EnableTessellation ? TessellationPass : MaterialPass;
                currentOITPass = EnableTessellation ? TessellationOITPass : MaterialOITPass;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
            /// </summary>
            /// <param name="passName">Name of the pass.</param>
            /// <param name="manager">The manager.</param>
            /// <param name="technique"></param>
            /// <param name="material">The material.</param>
            public PhongMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
                : this(manager, technique, material, passName)
            {
            }

            protected override void OnInitialPropertyBindings()
            {
                AddPropertyBinding(nameof(PhongMaterialCore.DiffuseColor), () => { WriteValue(PhongPBRMaterialStruct.DiffuseStr, material.DiffuseColor); });
                AddPropertyBinding(nameof(PhongMaterialCore.AmbientColor), () => { WriteValue(PhongPBRMaterialStruct.AmbientStr, material.AmbientColor); });
                AddPropertyBinding(nameof(PhongMaterialCore.EmissiveColor), () => { WriteValue(PhongPBRMaterialStruct.EmissiveStr, material.EmissiveColor); });
                AddPropertyBinding(nameof(PhongMaterialCore.ReflectiveColor), () => { WriteValue(PhongPBRMaterialStruct.ReflectStr, material.ReflectiveColor); });
                AddPropertyBinding(nameof(PhongMaterialCore.SpecularColor), () => { WriteValue(PhongPBRMaterialStruct.SpecularStr, material.SpecularColor); });
                AddPropertyBinding(nameof(PhongMaterialCore.SpecularShininess), () => { WriteValue(PhongPBRMaterialStruct.ShininessStr, material.SpecularShininess); });
                AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMapScaleMask), () => { WriteValue(PhongPBRMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderShadowMap), ()=> { WriteValue(PhongPBRMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderEnvironmentMap), () => { WriteValue(PhongPBRMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.UVTransform), () => 
                {
                    Matrix m = material.UVTransform;
                    WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, m.Column1);
                    WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, m.Column2);
                });
                AddPropertyBinding(nameof(PhongMaterialCore.EnableAutoTangent), () => { WriteValue(PhongPBRMaterialStruct.EnableAutoTangent, material.EnableAutoTangent); });
                AddPropertyBinding(nameof(PhongMaterialCore.MaxTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance); });
                AddPropertyBinding(nameof(PhongMaterialCore.MaxDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor); });
                AddPropertyBinding(nameof(PhongMaterialCore.MinTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance); });
                AddPropertyBinding(nameof(PhongMaterialCore.MinDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && textureResources[DiffuseIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseAlphaMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && textureResources[AlphaIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderNormalMap), () => { WriteValue(PhongPBRMaterialStruct.HasNormalMapStr, material.RenderNormalMap && textureResources[NormalIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderSpecularColorMap), () => { WriteValue(PhongPBRMaterialStruct.HasSpecularColorMap, material.RenderSpecularColorMap && textureResources[SpecularColorIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderDisplacementMap), () => { WriteValue(PhongPBRMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && textureResources[DisplaceIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.RenderEmissiveMap), () => { WriteValue(PhongPBRMaterialStruct.HasEmissiveMapStr, material.RenderEmissiveMap && textureResources[EmissiveIdx] != null ? 1 : 0); });
                AddPropertyBinding(nameof(PhongMaterialCore.EnableFlatShading), () => { WriteValue(PhongPBRMaterialStruct.RenderFlat, material.EnableFlatShading); });
                AddPropertyBinding(nameof(PhongMaterialCore.VertexColorBlendingFactor), () => { WriteValue(PhongPBRMaterialStruct.VertColorBlending, material.VertexColorBlendingFactor); });
                AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMap), () => 
                {
                    CreateTextureView(material.DiffuseMap, DiffuseIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderDiffuseMap));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.DiffuseAlphaMap), () =>
                {
                    CreateTextureView(material.DiffuseAlphaMap, AlphaIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderDiffuseAlphaMap));
                });

                AddPropertyBinding(nameof(PhongMaterialCore.NormalMap), () => 
                {
                    CreateTextureView(material.NormalMap, NormalIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderNormalMap));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMap), () => 
                {
                    CreateTextureView(material.DisplacementMap, DisplaceIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderDisplacementMap));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.SpecularColorMap), () =>
                {
                    CreateTextureView(material.SpecularColorMap, SpecularColorIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderSpecularColorMap));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMapSampler), () =>
                {
                    RemoveAndDispose(ref surfaceSampler);
                    surfaceSampler = Collect(statePoolManager.Register(material.DiffuseMapSampler));
                });

                AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMapSampler), () =>
                {
                    RemoveAndDispose(ref displacementSampler);
                    displacementSampler = Collect(statePoolManager.Register(material.DisplacementMapSampler));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.EmissiveMap), () =>
                {
                    CreateTextureView(material.EmissiveMap, EmissiveIdx);
                    TriggerPropertyAction(nameof(PhongMaterialCore.RenderEmissiveMap));
                });
                AddPropertyBinding(nameof(PhongMaterialCore.EnableTessellation), () => { EnableTessellation = material.EnableTessellation; });

                shadowSampler = Collect(statePoolManager.Register(DefaultSamplers.ShadowSampler));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void CreateTextureView(TextureModel textureModel, int index)
            {
                RemoveAndDispose(ref textureResources[index]);
                textureResources[index] = textureModel == null ? null : Collect(textureManager.Register(textureModel));
                if (textureResources[index] != null)
                {
                    textureIndex |= 1u << index;
                }
                else
                {
                    textureIndex &= ~(1u << index);
                }
            }

            public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
            {
                if (HasTextures)
                {
                    OnBindMaterialTextures(deviceContext, shaderPass.VertexShader);
                    OnBindMaterialTextures(deviceContext, shaderPass.DomainShader);
                    OnBindMaterialTextures(context, deviceContext, shaderPass.PixelShader);
                }
                if (material.RenderShadowMap && context.IsShadowMapEnabled)
                {
                    shaderPass.PixelShader.BindTexture(deviceContext, texShadowSlot, context.SharedResource.ShadowView);
                    shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, shadowSampler);
                }
                return true;
            }

            /// <summary>
            /// Actual bindings
            /// </summary>
            /// <param name="context"></param>
            /// <param name="shader"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnBindMaterialTextures(DeviceContextProxy context, VertexShader shader)
            {
                if (shader.IsNULL)
                {
                    return;
                }
                int idx = shader.ShaderStageIndex;
                shader.BindTexture(context, texDisplaceSlot, textureResources[DisplaceIdx]);
                shader.BindSampler(context, samplerDisplaceSlot, displacementSampler);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnBindMaterialTextures(DeviceContextProxy context, DomainShader shader)
            {
                if (shader.IsNULL)
                {
                    return;
                }
                int idx = shader.ShaderStageIndex;
                shader.BindTexture(context, texDisplaceSlot, textureResources[DisplaceIdx]);
                shader.BindSampler(context, samplerDisplaceSlot, displacementSampler);
            }
            /// <summary>
            /// Actual bindings
            /// </summary>
            /// <param name="context"></param>
            /// <param name="deviceContext"></param>
            /// <param name="shader"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, PixelShader shader)
            {
                if (shader.IsNULL)
                {
                    return;
                }
                int idx = shader.ShaderStageIndex;
                shader.BindTexture(deviceContext, texDiffuseSlot, textureResources[DiffuseIdx]);
                shader.BindTexture(deviceContext, texNormalSlot, textureResources[NormalIdx]);
                shader.BindTexture(deviceContext, texAlphaSlot, textureResources[AlphaIdx]);
                shader.BindTexture(deviceContext, texSpecularSlot, textureResources[SpecularColorIdx]);
                shader.BindTexture(deviceContext, texEmissiveSlot, textureResources[EmissiveIdx]);
                shader.BindSampler(deviceContext, samplerDiffuseSlot, surfaceSampler);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdateMappings(ShaderPass shaderPass)
            {
                texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
                texAlphaSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
                texNormalSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
                texShadowSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowTexName);
                texSpecularSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderSpecularTexName);
                texEmissiveSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderEmissiveTexName);
                samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
                samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
                if (!shaderPass.DomainShader.IsNULL && material.EnableTessellation)
                {
                    texDisplaceSlot = shaderPass.DomainShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DisplacementMapTB);
                    samplerDisplaceSlot = shaderPass.DomainShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DisplacementMapSampler);
                }
                else
                {
                    texDisplaceSlot = shaderPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DisplacementMapTB);
                    samplerDisplaceSlot = shaderPass.VertexShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DisplacementMapSampler);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="disposeManagedResources"></param>
            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                {
                    for (int i = 0; i < NUMTEXTURES; ++i)
                    {
                        textureResources[i] = null;
                    }
                    surfaceSampler = displacementSampler = shadowSampler = null;
                }

                base.OnDispose(disposeManagedResources);
            }

            public override ShaderPass GetPass(RenderType renderType, RenderContext context)
            {
                return renderType == RenderType.Transparent && context.IsOITPass ? currentOITPass : currentMaterialPass;
            }

            public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
            {
                return ShadowPass;
            }

            public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
            {
                return renderType == RenderType.Transparent && context.IsOITPass ? WireframeOITPass : WireframePass;
            }

            public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
            {
                return DepthPass;
            }

            public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
            {
                DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
            }
        }
    }

}
