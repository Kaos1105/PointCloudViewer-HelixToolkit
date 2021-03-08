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
    namespace Shaders
    {
        using Helper;
        /// <summary>
        /// 
        /// </summary>
        public static class DefaultPSShaderByteCodes
        {
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshBinnPhong
            {
                get;
            } = "psMeshBlinnPhong";
            /// <summary>
            /// Gets the ps mesh binn phong order independent transparent shader.
            /// </summary>
            /// <value>
            /// The ps mesh binn phong order independent transparent shader.
            /// </value>
            public static string PSMeshBinnPhongOIT
            {
                get;
            } = "psMeshBlinnPhongOIT";
            /// <summary>
            /// Gets the ps mesh binn phong oit quad.
            /// </summary>
            /// <value>
            /// The ps mesh binn phong oit quad.
            /// </value>
            public static string PSMeshBinnPhongOITQuad
            {
                get;
            } = "psMeshBlinnPhongOITQuad";
            /// <summary>
            /// Gets the ps mesh diffuse map oit.
            /// </summary>
            /// <value>
            /// The ps mesh diffuse map oit.
            /// </value>
            public static string PSMeshDiffuseMapOIT
            {
                get;
            } = "psMeshDiffuseMapOIT";
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshVertColor
            {
                get;
            } = "psColor";
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshVertPosition
            {
                get;
            } = "psPositions";
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshNormal
            {
                get;
            } = "psNormals";

            public static string PSMeshDiffuseMap
            {
                get;
            } = "psDiffuseMap";
            public static string PSMeshColorStripe
            {
                get;
            } = "psMeshColorStripe";

            public static string PSMeshViewCube
            {
                get;
            } = "psViewCube";
            /// <summary>
            /// 
            /// </summary>
            public static string PSShadow
            {
                get;
            } = "psShadow";

            /// <summary>
            /// 
            /// </summary>
            public static string PSPoint
            {
                get;
            } = "psPoint";
            /// <summary>
            /// 
            /// </summary>
            public static string PSLine
            {
                get;
            } = "psLine";

            /// <summary>
            /// 
            /// </summary>
            public static string PSLineColor
            {
                get;
            } = "psLineColor";

            /// <summary>
            /// 
            /// </summary>
            public static string PSBillboardText
            {
                get;
            } = "psBillboardText";
            /// <summary>
            /// Gets the ps billboard text order independent transparent shader.
            /// </summary>
            /// <value>
            /// The ps billboard text order independent transparent shader.
            /// </value>
            public static string PSBillboardTextOIT
            {
                get;
            } = "psBillboardTextOIT";
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshXRay
            {
                get;
            } = "psMeshXRay";

            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshClipPlaneBackface
            {
                get;
            } = "psMeshClipPlaneBackface";
            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshClipPlaneQuad
            {
                get;
            } = "psMeshClipPlaneQuad";
            /// <summary>
            /// 
            /// </summary>
            public static string PSParticle
            {
                get;
            } = "psParticle";
            /// <summary>
            /// Gets the ps particle order independent transparent shader.
            /// </summary>
            public static string PSParticleOIT
            {
                get;
            } = "psParticleOIT";
            /// <summary>
            /// 
            /// </summary>
            public static string PSSkybox
            {
                get;
            } = "psSkybox";

            /// <summary>
            /// 
            /// </summary>
            public static string PSMeshWireframe
            {
                get;
            } = "psWireframe";
            /// <summary>
            /// Gets the ps mesh wireframe oit.
            /// </summary>
            /// <value>
            /// The ps mesh wireframe oit.
            /// </value>
            public static string PSMeshWireframeOIT
            {
                get;
            } = "psWireframeOIT";
            /// <summary>
            /// 
            /// </summary>
            public static string PSDepthStencilTestOnly
            {
                get;
            } = "psDepthStencilOnly";
            /// <summary>
            /// Gets the ps mesh outline screen quad.
            /// </summary>
            /// <value>
            /// The ps mesh outline screen quad.
            /// </value>
            public static string PSEffectOutlineScreenQuad
            {
                get;
            } = "psEffectOutlineQuad";
            /// <summary>
            /// Gets the ps effect full screen blur vertical.
            /// </summary>
            /// <value>
            /// The ps effect full screen blur vertical.
            /// </value>
            public static string PSEffectFullScreenBlurVertical
            {
                get;
            } = "psEffectGaussianBlurVertical";

            /// <summary>
            /// Gets the ps effect full screen blur horizontal.
            /// </summary>
            /// <value>
            /// The ps effect full screen blur horizontal.
            /// </value>
            public static string PSEffectFullScreenBlurHorizontal
            {
                get;
            } = "psEffectGaussianBlurHorizontal";

            /// <summary>
            /// Gets the ps mesh border highlight
            /// </summary>
            /// <value>
            /// The ps mesh mesh border highlight
            /// </value>
            public static string PSEffectMeshBorderHighlight
            {
                get;
            } = "psEffectMeshBorderHighlight";

            /// <summary>
            /// Gets the ps effect outline smooth.
            /// </summary>
            /// <value>
            /// The ps effect outline smooth.
            /// </value>
            public static string PSEffectOutlineSmooth
            {
                get;
            } = "psEffectOutlineSmooth";
            /// <summary>
            /// Gets the ps mesh outline screen quad stencil.
            /// </summary>
            /// <value>
            /// The ps mesh outline screen quad stencil.
            /// </value>
            public static string PSEffectOutlineScreenQuadStencil
            {
                get;
            } = "psEffectOutlineQuadStencil";
            /// <summary>
            /// Gets the ps mesh outline quad final.
            /// </summary>
            /// <value>
            /// The ps mesh outline quad final.
            /// </value>
            public static string PSEffectOutlineQuadFinal
            {
                get;
            } = "psEffectOutlineQualFinal";
            /// <summary>
            /// 
            /// </summary>
            /// <value>
            /// 
            /// </value>
            public static string PSEffectMeshXRay
            {
                get;
            } = "psEffectMeshXRay";
            /// <summary>
            /// Gets the ps effect bloom extract.
            /// </summary>
            /// <value>
            /// The ps effect bloom extract.
            /// </value>
            public static string PSEffectBloomExtract
            {
                get;
            } = "psEffectBloomExtract";
            /// <summary>
            /// Gets the ps effect bloom vertical blur.
            /// </summary>
            /// <value>
            /// The ps effect bloom vertical blur.
            /// </value>
            public static string PSEffectBloomVerticalBlur
            {
                get;
            } = "psEffectBloomBlurVertical";
            /// <summary>
            /// Gets the ps effect bloom horizontal blur.
            /// </summary>
            /// <value>
            /// The ps effect bloom horizontal blur.
            /// </value>
            public static string PSEffectBloomHorizontalBlur
            {
                get;
            } = "psEffectBloomBlurHorizontal";
            /// <summary>
            /// Gets the ps effect bloom combine.
            /// </summary>
            /// <value>
            /// The ps effect bloom combine.
            /// </value>
            public static string PSEffectBloomCombine
            {
                get;
            } = "psEffectBloomCombine";
            /// <summary>
            /// Gets the ps effect fxaa.
            /// </summary>
            /// <value>
            /// The ps effect fxaa.
            /// </value>
            public static string PSEffectFXAA
            {
                get;
            } = "psFXAA";
            /// <summary>
            /// Gets the ps effect luma.
            /// </summary>
            /// <value>
            /// The ps effect luma.
            /// </value>
            public static string PSEffectLUMA
            {
                get;
            } = "psLuma";
            /// <summary>
            /// Gets the ps effect x ray grid. This is based on BlinnPhong
            /// </summary>
            /// <value>
            /// The ps effect x ray grid.
            /// </value>
            public static string PSEffectXRayGrid
            {
                get;
            } = "psEffectMeshXRayGrid";
            /// <summary>
            /// Gets the ps effect diffuse x ray grid.
            /// </summary>
            /// <value>
            /// The ps effect diffuse x ray grid.
            /// </value>
            public static string PSEffectDiffuseXRayGrid
            {
                get;
            } = "psEffectMeshDiffuseXRayGrid";

            /// <summary>
            /// Gets the ps plane grid.
            /// </summary>
            /// <value>
            /// The ps plane grid.
            /// </value>
            public static string PSPlaneGrid
            {
                get;
            } = "psPlaneGrid";

            /// <summary>
            /// Gets the ps mesh PBR.
            /// </summary>
            /// <value>
            /// The ps mesh PBR.
            /// </value>
            public static string PSMeshPBR
            {
                get;
            } = "psMeshPBR";

            /// <summary>
            /// Gets the ps mesh PBR OIT.
            /// </summary>
            /// <value>
            /// The ps mesh PBR.
            /// </value>
            public static string PSMeshPBROIT
            {
                get;
            } = "psMeshPBROIT";

            /// <summary>
            /// Gets the ps sprite.
            /// </summary>
            /// <value>
            /// The ps sprite.
            /// </value>
            public static string PSSprite2D
            {
                get;
            } = "psSprite";


            /// <summary>
            /// 
            /// </summary>
            public static string PSScreenDup
            {
                get;
            } = "psScreenDup";

            public static string PSVolume3D
            {
                get;
            } = "psVolume";

            public static string PSVolumeCube
            {
                get;
            } = "psVolumeCube";

            public static string PSVolumeDiffuse
            {
                get;
            } = "psVolumeDiffuse";

            public static string PSSSAOP1
            {
                get;
            } = "psSSAOP1";

            public static string PSSSAO
            {
                get;
            } = "psSSAO";

            public static string PSSSAOBlur
            {
                get;
            } = "psSSAOBlur";
        }


        /// <summary>
        /// Default Pixel Shaders
        /// </summary>
        public static class DefaultPSShaderDescriptions
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshBlinnPhong = new ShaderDescription(nameof(PSMeshBlinnPhong), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshBinnPhong);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshBlinnPhongOIT = new ShaderDescription(nameof(PSMeshBlinnPhongOIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshBinnPhongOIT);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshBlinnPhongOITQuad = new ShaderDescription(nameof(PSMeshBlinnPhongOITQuad), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshBinnPhongOITQuad);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshVertColor = new ShaderDescription(nameof(PSMeshVertColor), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshVertColor);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshVertNormal = new ShaderDescription(nameof(PSMeshVertNormal), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshNormal);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshVertPosition = new ShaderDescription(nameof(PSMeshVertPosition), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshVertPosition);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshDiffuseMap = new ShaderDescription(nameof(PSMeshDiffuseMap), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshDiffuseMap);
            /// <summary>
            /// The ps mesh diffuse map oit
            /// </summary>
            public static readonly ShaderDescription PSMeshDiffuseMapOIT = new ShaderDescription(nameof(PSMeshDiffuseMapOIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshDiffuseMapOIT);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshColorStripe = new ShaderDescription(nameof(PSMeshColorStripe), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshColorStripe);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshViewCube = new ShaderDescription(nameof(PSMeshViewCube), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshViewCube);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSPoint = new ShaderDescription(nameof(PSPoint), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSPoint);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSLine = new ShaderDescription(nameof(PSLine), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSLine);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSLineColor = new ShaderDescription(nameof(PSLineColor), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSLineColor);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSBillboardText = new ShaderDescription(nameof(PSBillboardText), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSBillboardText);
            /// <summary>
            /// The ps billboard text oit
            /// </summary>
            public static readonly ShaderDescription PSBillboardTextOIT = new ShaderDescription(nameof(PSBillboardTextOIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSBillboardTextOIT);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshXRay = new ShaderDescription(nameof(PSMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshXRay);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSShadow = new ShaderDescription(nameof(PSShadow), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSShadow);
    #region Mesh Clipping
            /// <summary>
            /// /
            /// </summary>
            public static readonly ShaderDescription PSMeshClipBackface = new ShaderDescription(nameof(PSMeshClipBackface), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshClipPlaneBackface);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshClipScreenQuad = new ShaderDescription(nameof(PSMeshClipScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshClipPlaneQuad);
    #endregion
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSParticle = new ShaderDescription(nameof(PSParticle), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSParticle);

            /// <summary>
            /// The ps particle oit
            /// </summary>
            public static readonly ShaderDescription PSParticleOIT = new ShaderDescription(nameof(PSParticleOIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSParticleOIT);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSSkybox = new ShaderDescription(nameof(PSSkybox), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSSkybox);

            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshWireframe = new ShaderDescription(nameof(PSMeshWireframe), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshWireframe);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription PSMeshWireframeOIT = new ShaderDescription(nameof(PSMeshWireframeOIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshWireframeOIT);
            /// <summary>
            /// The ps depth stencil only
            /// </summary>
            public static readonly ShaderDescription PSDepthStencilOnly = new ShaderDescription(nameof(PSDepthStencilOnly), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSDepthStencilTestOnly);

            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSMeshOutlineScreenQuad = new ShaderDescription(nameof(PSMeshOutlineScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectOutlineScreenQuad);

            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSEffectFullScreenBlurVertical = new ShaderDescription(nameof(PSEffectFullScreenBlurVertical), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectFullScreenBlurVertical);
            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSEffectFullScreenBlurHorizontal = new ShaderDescription(nameof(PSEffectFullScreenBlurHorizontal), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectFullScreenBlurHorizontal);
            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSEffectMeshBorderHighlight = new ShaderDescription(nameof(PSEffectMeshBorderHighlight), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectMeshBorderHighlight);
            /// <summary>
            /// The ps effect mesh border highlight
            /// </summary>
            public static readonly ShaderDescription PSEffectOutlineSmooth = new ShaderDescription(nameof(PSEffectOutlineSmooth), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectOutlineSmooth);
            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSMeshOutlineQuadStencil = new ShaderDescription(nameof(PSMeshOutlineQuadStencil), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectOutlineScreenQuadStencil);

            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSMeshOutlineQuadFinal = new ShaderDescription(nameof(PSMeshOutlineQuadFinal), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectOutlineQuadFinal);

            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSEffectMeshXRay = new ShaderDescription(nameof(PSEffectMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectMeshXRay);

            /// <summary>
            ///
            /// </summary>
            public static readonly ShaderDescription PSEffectBloomExtract = new ShaderDescription(nameof(PSEffectBloomExtract), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectBloomExtract);
            /// <summary>
            /// The ps effect bloom vertical blur
            /// </summary>
            public static readonly ShaderDescription PSEffectBloomVerticalBlur = new ShaderDescription(nameof(PSEffectBloomVerticalBlur), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectBloomVerticalBlur);
            /// <summary>
            /// The ps effect bloom horizontal blur
            /// </summary>
            public static readonly ShaderDescription PSEffectBloomHorizontalBlur = new ShaderDescription(nameof(PSEffectBloomHorizontalBlur), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectBloomHorizontalBlur);

            /// <summary>
            /// The ps effect bloom combine
            /// </summary>
            public static readonly ShaderDescription PSEffectBloomCombine = new ShaderDescription(nameof(PSEffectBloomCombine), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectBloomCombine);

            /// <summary>
            /// The ps effect FXAA
            /// </summary>
            public static readonly ShaderDescription PSEffectFXAA = new ShaderDescription(nameof(PSEffectFXAA), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectFXAA);
            /// <summary>
            /// The ps effect luma
            /// </summary>
            public static readonly ShaderDescription PSEffectLUMA = new ShaderDescription(nameof(PSEffectLUMA), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectLUMA);
            /// <summary>
            /// The ps effect x ray grid, this is based on BlinnPhong
            /// </summary>
            public static readonly ShaderDescription PSEffectXRayGrid = new ShaderDescription(nameof(PSEffectXRayGrid), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectXRayGrid);
            /// <summary>
            /// The ps effect x ray grid, this is based on diffuse shading
            /// </summary>
            public static readonly ShaderDescription PSEffectDiffuseXRayGrid = new ShaderDescription(nameof(PSEffectDiffuseXRayGrid), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSEffectDiffuseXRayGrid);
            /// <summary>
            /// The ps plane grid
            /// </summary>
            public static readonly ShaderDescription PSPlaneGrid = new ShaderDescription(nameof(PSPlaneGrid), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSPlaneGrid);

            /// <summary>
            /// The ps mesh PBR
            /// </summary>
            public static readonly ShaderDescription PSMeshPBR = new ShaderDescription(nameof(PSMeshPBR), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshPBR);

            /// <summary>
            /// The ps mesh PBR
            /// </summary>
            public static readonly ShaderDescription PSMeshPBROIT = new ShaderDescription(nameof(PSMeshPBROIT), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSMeshPBROIT);

            /// <summary>
            /// The ps sprite
            /// </summary>
            public static readonly ShaderDescription PSSprite2D = new ShaderDescription(nameof(PSSprite2D), ShaderStage.Pixel, new ShaderReflector(), 
                DefaultPSShaderByteCodes.PSSprite2D);

            /// <summary>
            /// The ps screen dup
            /// </summary>
            public static readonly ShaderDescription PSScreenDup = new ShaderDescription(nameof(PSScreenDup), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSScreenDup);

            /// <summary>
            /// The ps volume3d
            /// </summary>
            public static readonly ShaderDescription PSVolume3D = new ShaderDescription(nameof(PSVolume3D), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSVolume3D);

            /// <summary>
            /// The ps volume cube
            /// </summary>
            public static readonly ShaderDescription PSVolumeCube = new ShaderDescription(nameof(PSVolumeCube), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSVolumeCube);

            /// <summary>
            /// The ps volume3d
            /// </summary>
            public static readonly ShaderDescription PSVolumeDiffuse3D = new ShaderDescription(nameof(PSVolumeDiffuse3D), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSVolumeDiffuse);
            /// <summary>
            /// The psssao p1
            /// </summary>
            public static readonly ShaderDescription PSSSAOP1 = new ShaderDescription(nameof(PSSSAOP1), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSSSAOP1);
            /// <summary>
            /// The psssao
            /// </summary>
            public static readonly ShaderDescription PSSSAO = new ShaderDescription(nameof(PSSSAO), ShaderStage.Pixel, new ShaderReflector(),
               DefaultPSShaderByteCodes.PSSSAO);

            /// <summary>
            /// The ps ssao blur
            /// </summary>
            public static readonly ShaderDescription PSSSAOBlur = new ShaderDescription(nameof(PSSSAOBlur), ShaderStage.Pixel, new ShaderReflector(),
                DefaultPSShaderByteCodes.PSSSAOBlur);
        }
    }

}
