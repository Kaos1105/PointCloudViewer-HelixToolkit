﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG
using SharpDX;
using System.Collections.Generic;
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
    namespace Utilities
    {
        /// <summary>
        /// Static octree for points
        /// </summary>
        public class StaticPointGeometryOctree : StaticOctree<int>
        {
            private static readonly Vector3 BoundOffset = new Vector3(0.001f);
            protected readonly IList<Vector3> Positions;
      
            /// <summary>
            /// 
            /// </summary>
            /// <param name="positions"></param>
            /// <param name="parameter"></param>
            /// <param name="stackCache"></param>
            public StaticPointGeometryOctree(IList<Vector3> positions,
                OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
                   : base(parameter)
            {
                Positions = positions;
            }
            /// <summary>
            /// Gets the bounding box from item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <returns></returns>
            protected override BoundingBox GetBoundingBoxFromItem(ref int item)
            {
                return new BoundingBox(Positions[item] - BoundOffset, Positions[item] + BoundOffset);
            }
            /// <summary>
            /// Gets the maximum bound.
            /// </summary>
            /// <returns></returns>
            protected override BoundingBox GetMaxBound()
            {
                return BoundingBoxExtensions.FromPoints(Positions);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="target"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            protected override bool IsContains(ref BoundingBox source, BoundingBox target, ref int obj)
            {
                var point = Positions[obj];
                //Bound contains point.
                return source.Minimum.X <= point.X && source.Maximum.X >= point.X &&
                    source.Minimum.Y <= point.Y && source.Maximum.Y >= point.Y &&
                    source.Minimum.Z <= point.Z && source.Maximum.Z >= point.Z;
            }

            /// <summary>
            /// Gets the objects.
            /// </summary>
            /// <returns></returns>
            protected override int[] GetObjects()
            {
                var objects = new int[Positions.Count];
                for (int i = 0; i < Positions.Count; ++i)
                {
                    objects[i] = i;
                }
                return objects;
            }

            #region Temp Variables for hittest
            private bool needRecalculate = true;
            private Vector3 clickPoint;
            private Matrix smvpm;
            #endregion
            public override bool HitTest(IRenderMatrices context, object model, Geometry3D geometry, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness)
            {
                needRecalculate = true;
                return base.HitTest(context, model, geometry, modelMatrix, rayWS, ref hits, hitThickness);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="octant"></param>
            /// <param name="context"></param>
            /// <param name="model"></param>
            /// <param name="geometry"></param>
            /// <param name="modelMatrix"></param>
            /// <param name="rayWS"></param>
            /// <param name="rayModel"></param>
            /// <param name="returnMultiple"></param>
            /// <param name="hits"></param>
            /// <param name="isIntersect"></param>
            /// <param name="hitThickness"></param>
            /// <returns></returns>
            protected override bool HitTestCurrentNodeExcludeChild(ref Octant octant, IRenderMatrices context, object model, 
                Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, bool returnMultiple,
                ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
            {
                isIntersect = false;
                if (!octant.IsBuilt || context == null)
                {
                    return false;
                }
                var isHit = false;
                var bound = octant.Bound;
                if (rayModel.Intersects(ref bound))
                {
                    isIntersect = true;
                    if (octant.Count == 0)
                    {
                        return false;
                    }
                    var result = new HitTestResult();
                    result.Distance = double.MaxValue;
                    if (needRecalculate)
                    {
                        var svpm = context.ScreenViewProjectionMatrix;
                        smvpm = modelMatrix * svpm;
                        var clickPoint3 = rayWS.Position + rayWS.Direction;
                        Vector3.TransformCoordinate(ref clickPoint3, ref svpm, out clickPoint);
                        needRecalculate = false;
                    }
                    isIntersect = true;
                    var dist = hitThickness;
                    for (int i = octant.Start; i < octant.End; ++i)
                    {
                        var v0 = Positions[Objects[i]];
                        var p0 = Vector3.TransformCoordinate(v0, smvpm);
                        var pv = p0 - clickPoint;
                        var d = pv.Length() / context.DpiScale;
                        if (returnMultiple)
                        {
                            dist = hitThickness;
                        }
                        if (d < dist) // If d is NaN, the condition is false.
                        {
                            dist = d;
                            result.IsValid = true;
                            result.ModelHit = model;
                            var px = Vector3.TransformCoordinate(v0, modelMatrix);
                            result.PointHit = px;
                            result.Distance = (rayWS.Position - px).Length();
                            result.Tag = Objects[i];
                            result.Geometry = geometry;
                            isHit = true;
                            if (returnMultiple)
                            {
                                hits.Add(result);
                                result = new HitTestResult();
                            }
                        }
                    }

                    if (isHit && !returnMultiple)
                    {
                        isHit = false;
                        if (hits.Count > 0)
                        {
                            if (hits[0].Distance > result.Distance)
                            {
                                hits[0] = result;
                                isHit = true;
                            }
                        }
                        else
                        {
                            hits.Add(result);
                            isHit = true;
                        }
                    }
                }

                return isHit;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="octant"></param>
            /// <param name="context"></param>
            /// <param name="sphere"></param>
            /// <param name="result"></param>
            /// <param name="isIntersect"></param>
            /// <returns></returns>
            protected override bool FindNearestPointBySphereExcludeChild(ref Octant octant, IRenderMatrices context,
                ref BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
            {
                bool isHit = false;
                var resultTemp = new HitTestResult();
                resultTemp.Distance = float.MaxValue;
                if (!BoxDisjointSphere(octant.Bound, ref sphere))
                {
                    isIntersect = true;
                    for (int i = octant.Start; i < octant.End; ++i)
                    {
                        var p = Positions[Objects[i]];
                        if (sphere.Contains(ref p) != ContainmentType.Disjoint)
                        {
                            var d = (p - sphere.Center).Length();
                            if (resultTemp.Distance > d)
                            {
                                resultTemp.Distance = d;
                                resultTemp.IsValid = true;
                                resultTemp.PointHit = p;
                                resultTemp.Tag = Objects[i];
                                isHit = true;
                            }
                        }
                    }
                    if (isHit)
                    {
                        isHit = false;
                        if (result.Count > 0)
                        {
                            if (result[0].Distance > resultTemp.Distance)
                            {
                                result[0] = resultTemp;
                                isHit = true;
                            }
                        }
                        else
                        {
                            result.Add(resultTemp);
                            isHit = true;
                        }
                    }
                }
                else
                {
                    isIntersect = false;
                }
                return isHit;
            }
        }
    }

}
