﻿/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// 
        /// </summary>
        public class InstancingMeshNode : MeshNode
        {
            #region Properties
            private IList<Guid> instanceIdentifiers;
            /// <summary>
            /// Gets or sets the instance identifiers.
            /// </summary>
            /// <value>
            /// The instance identifiers.
            /// </value>
            public IList<Guid> InstanceIdentifiers
            {
                set => Set(ref instanceIdentifiers, value);
                get => instanceIdentifiers;
            }
            /// <summary>
            /// Gets or sets the instance parameter array.
            /// </summary>
            /// <value>
            /// The instance parameter array.
            /// </value>
            public IList<InstanceParameter> InstanceParamArray
            {
                set { instanceParamBuffer.Elements = value; }
                get { return instanceParamBuffer.Elements; }
            }

            private IOctreeManager octreeManager = null;
            /// <summary>
            /// Gets or sets the octree manager.
            /// </summary>
            /// <value>
            /// The octree manager.
            /// </value>
            public IOctreeManager OctreeManager
            {
                set
                {
                    if(Set(ref octreeManager, value))
                    {
                        if (octreeManager != null)
                        {
                            octreeManager.Clear();
                            isInstanceChanged = true;
                        }
                    }
                }
                get { return octreeManager; }
            }
            #endregion

            private bool isInstanceChanged = false;

            /// <summary>
            /// The instance parameter buffer
            /// </summary>
            protected IElementsBufferModel<InstanceParameter> instanceParamBuffer = new InstanceParamsBufferModel<InstanceParameter>(InstanceParameter.SizeInBytes);
            /// <summary>
            /// Called when [create render technique].
            /// </summary>
            /// <param name="host">The host.</param>
            /// <returns></returns>
            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.InstancingMesh];
            }
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new InstancingMeshRenderCore() { ParameterBuffer = this.instanceParamBuffer };
            }
            /// <summary>
            /// Called when [attach].
            /// </summary>
            /// <param name="host">The host.</param>
            /// <returns></returns>
            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    instanceParamBuffer.Initialize();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Called when [detach].
            /// </summary>
            protected override void OnDetach()
            {
                instanceParamBuffer.DisposeAndClear();
                base.OnDetach();
            }
            /// <summary>
            /// Updates the not render.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void UpdateNotRender(RenderContext context)
            {
                base.UpdateNotRender(context);
                if (isInstanceChanged)
                {
                    BuildOctree();
                    isInstanceChanged = false;
                }
            }
            /// <summary>
            /// Instanceses the changed.
            /// </summary>
            protected override void InstancesChanged()
            {
                base.InstancesChanged();
                octreeManager?.Clear();
                isInstanceChanged = true;
            }
            /// <summary>
            /// Builds the octree.
            /// </summary>
            private void BuildOctree()
            {
                if (IsRenderable && InstanceBuffer.HasElements)
                {
                    octreeManager?.RebuildTree(Enumerable.Repeat<SceneNode>(this, 1));
                }
                else
                {
                    octreeManager?.Clear();
                }
            }
            /// <summary>
            /// Hits the test.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="rayWS">The ray ws.</param>
            /// <param name="hits">The hits.</param>
            /// <returns></returns>
            public override bool HitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
            {
                bool isHit = false;
                if (CanHitTest(context) && PreHitTestOnBounds(ref rayWS))
                {
                    if (octreeManager != null && octreeManager.Octree != null)
                    {
                        var boundHits = new List<HitTestResult>();
                        isHit = octreeManager.Octree.HitTest(context, this.WrapperSource, Geometry, TotalModelMatrixInternal, rayWS, ref boundHits);
                        if (isHit)
                        {
                            isHit = false;
                            Matrix instanceMatrix;
                            foreach (var hit in boundHits)
                            {
                                int instanceIdx = (int)hit.Tag;
                                instanceMatrix = InstanceBuffer.Elements[instanceIdx];
                                var h = base.OnHitTest(context, TotalModelMatrixInternal * instanceMatrix, ref rayWS, ref hits);
                                isHit |= h;
                                if (h && hits.Count > 0)
                                {
                                    var result = hits.Last();
                                    object tag = null;
                                    if (InstanceIdentifiers != null && InstanceIdentifiers.Count == InstanceBuffer.Elements.Count)
                                    {
                                        tag = InstanceIdentifiers[instanceIdx];
                                    }
                                    else
                                    {
                                        tag = instanceIdx;
                                    }
                                    result.Tag = tag;
                                    hits[hits.Count - 1] = result;
                                }
                            }
                        }
                    }
                    else
                    {
                        isHit = base.HitTest(context, rayWS, ref hits);
                    }
                }
                return isHit;
            }
        }
    }

}