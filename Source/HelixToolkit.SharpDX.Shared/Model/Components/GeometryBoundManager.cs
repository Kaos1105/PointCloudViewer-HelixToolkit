﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
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
    namespace Model.Components
    {
        using System;
        using Scene;

        public sealed class GeometryBoundManager : IDisposable
        {
            #region Properties
            private Geometry3D geometry = null;
            /// <summary>
            /// 
            /// </summary>
            public Geometry3D Geometry
            {
                set
                {
                    if(geometry == value)
                    {
                        return;
                    }
                    var old = geometry;
                    geometry = value;
                    if (geometry != null && geometry.Bound.Maximum == Vector3.Zero && geometry.Bound.Minimum == Vector3.Zero)
                    {
                        geometry.UpdateBounds();
                    }
                    if (old != null)
                    {
                        old.PropertyChanged -= OnGeometryPropertyChangedPrivate;
                    }
                    if (geometry != null)
                    {
                        geometry.PropertyChanged += OnGeometryPropertyChangedPrivate;
                        OriginalBounds = geometry.Bound;
                        OriginalBoundsSphere = geometry.BoundingSphere;
                    }
                    else
                    {
                        OriginalBounds = DefaultBound;
                        OriginalBoundsSphere = DefaultBoundSphere;
                    }
                    UpdateBounds();
                }
                get { return geometry; }
            }

            private IList<Matrix> instances = null;
            public IList<Matrix> Instances
            {
                set
                {
                    if(instances == value)
                    {
                        return;
                    }
                    instances = value;
                    UpdateBounds();
                }
                get
                {
                    return instances;
                }
            }

            public bool HasInstances { get { return instances != null && instances.Count > 0; } }

            public bool GeometryValid { private set; get; } = false;
            #region Bounds
            public static readonly BoundingBox DefaultBound = new BoundingBox();
            public static readonly BoundingSphere DefaultBoundSphere = new BoundingSphere();
            /// <summary>
            /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
            /// </summary>
            /// <value>
            /// The original bound.
            /// </value>
            public BoundingBox OriginalBounds;
            /// <summary>
            /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
            /// </summary>
            /// <value>
            /// The original bound sphere.
            /// </value>
            public BoundingSphere OriginalBoundsSphere;

            /// <summary>
            /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
            /// </summary>
            /// <value>
            /// The bounds.
            /// </value>
            public BoundingBox Bounds = DefaultBound;

            /// <summary>
            /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
            /// </summary>
            /// <value>
            /// The bounds with transform.
            /// </value>
            public BoundingBox BoundsWithTransform = DefaultBound;

            /// <summary>
            /// Gets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
            /// </summary>
            /// <value>
            /// The bounds sphere.
            /// </value>
            public BoundingSphere BoundsSphere = DefaultBoundSphere;

            /// <summary>
            /// Gets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
            /// </summary>
            /// <value>
            /// The bounds sphere with transform.
            /// </value>
            public BoundingSphere BoundsSphereWithTransform = DefaultBoundSphere;

            public bool HasBound { set; get; } = true;
            #endregion
            #endregion
            #region Events and Delegates
            public event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;

            public event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;

            public event EventHandler<BoundChangeArgs<BoundingSphere>> OnBoundSphereChanged;

            public event EventHandler<BoundChangeArgs<BoundingSphere>> OnTransformBoundSphereChanged;

            private void RaiseOnTransformBoundChanged(BoundingBox newBound, BoundingBox oldBound)
            {
                OnTransformBoundChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingBox>(ref newBound, ref oldBound));
            }

            private void RaiseOnBoundChanged(BoundingBox newBound, BoundingBox oldBound)
            {
                OnBoundChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingBox>(ref newBound, ref oldBound));
            }


            private void RaiseOnTransformBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
            {
                OnTransformBoundSphereChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingSphere>(ref newBoundSphere, ref oldBoundSphere));
            }


            private void RaiseOnBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
            {
                OnBoundSphereChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingSphere>(ref newBoundSphere, ref oldBoundSphere));
            }
            #endregion
            public delegate bool OnCheckGeometryDelegate(Geometry3D geometry);
            public OnCheckGeometryDelegate OnCheckGeometry; 

            private GeometryNode elementCore;

            public GeometryBoundManager(GeometryNode core)
            {
                this.elementCore = core;
                core.TransformChanged += OnTransformChanged;
            }

            private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName.Equals(nameof(Geometry3D.Positions)) || e.PropertyName.Equals(Geometry3D.VertexBuffer))
                {
                    GeometryValid = OnCheckGeometry != null ? OnCheckGeometry.Invoke(this.geometry) : CheckGeometry();
                }
                else if (e.PropertyName.Equals(nameof(Geometry3D.Bound)))
                {
                    UpdateBoundingBox();
                }
                else if (e.PropertyName.Equals(nameof(Geometry3D.BoundingSphere)))
                {
                    UpdateBoundingSphere();
                }
            }

            /// <summary>
            /// <para>Check geometry validity.</para>
            /// Return false if (this.geometryInternal == null || this.geometryInternal.Positions == null || this.geometryInternal.Positions.Count == 0 || this.geometryInternal.Indices == null || this.geometryInternal.Indices.Count == 0)
            /// </summary>
            /// <returns>
            /// </returns>
            private bool CheckGeometry()
            {
                return !(this.Geometry == null || this.Geometry.Positions == null || this.Geometry.Positions.Count == 0);
            }

            private void OnTransformChanged(object sender, TransformArgs e)
            {
                var oldBound = BoundsWithTransform;
                BoundsWithTransform = Bounds.Transform(e);
                RaiseOnTransformBoundChanged(BoundsWithTransform, oldBound);
                var oldSphere = BoundsSphereWithTransform;
                BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(e);
                RaiseOnTransformBoundSphereChanged(BoundsSphereWithTransform, oldSphere);
            }

            private void UpdateBoundingBox()
            {
                if (!GeometryValid)
                {
                    Bounds = DefaultBound;
                    BoundsWithTransform = DefaultBound;
                }
                else
                {
                    BoundingBox oldBound;
                    if (!HasInstances)
                    {
                        oldBound = Bounds;
                        Bounds = Geometry.Bound;
                        RaiseOnBoundChanged(Bounds, oldBound);
                        oldBound = BoundsWithTransform;
                        BoundsWithTransform = Bounds.Transform(elementCore.TotalModelMatrixInternal);
                        RaiseOnTransformBoundChanged(BoundsWithTransform, oldBound);
                    }
                    else
                    {
                        var bound = Geometry.Bound.Transform(Instances[0]);
                        foreach (var instance in Instances)
                        {
                            var b = Geometry.Bound.Transform(instance);
                            BoundingBox.Merge(ref bound, ref b, out bound);
                        }
                        oldBound = Bounds;
                        Bounds = bound;
                        RaiseOnBoundChanged(Bounds, oldBound);
                        oldBound = BoundsWithTransform;
                        BoundsWithTransform = Bounds.Transform(elementCore.TotalModelMatrixInternal);
                        RaiseOnTransformBoundChanged(BoundsWithTransform, oldBound);
                    }
                }
            }

            private void UpdateBoundingSphere()
            {
                if (!GeometryValid)
                {
                    BoundsSphere = DefaultBoundSphere;
                    BoundsSphereWithTransform = DefaultBoundSphere;
                }
                else
                {
                    BoundingSphere oldSphere;
                    if (!HasInstances)
                    {
                        oldSphere = BoundsSphere;
                        BoundsSphere = Geometry.BoundingSphere;
                        RaiseOnBoundSphereChanged(BoundsSphere, oldSphere);
                        oldSphere = BoundsSphereWithTransform;
                        BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(elementCore.TotalModelMatrixInternal);
                        RaiseOnTransformBoundSphereChanged(BoundsSphereWithTransform, oldSphere);
                    }
                    else
                    {
                        var boundSphere = Geometry.BoundingSphere.TransformBoundingSphere(Instances[0]);
                        foreach (var instance in Instances)
                        {
                            var bs = Geometry.BoundingSphere.TransformBoundingSphere(instance);
                            BoundingSphere.Merge(ref boundSphere, ref bs, out boundSphere);
                        }
                        oldSphere = BoundsSphere;
                        BoundsSphere = boundSphere;
                        RaiseOnBoundSphereChanged(BoundsSphere, oldSphere);
                        oldSphere = BoundsSphereWithTransform;
                        BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(elementCore.TotalModelMatrixInternal);
                        RaiseOnTransformBoundSphereChanged(BoundsSphereWithTransform, oldSphere);
                    }
                }
            }

            private void UpdateBounds()
            {
                GeometryValid = OnCheckGeometry != null ? OnCheckGeometry.Invoke(this.geometry) : CheckGeometry();
                UpdateBoundingBox();
                UpdateBoundingSphere();
            }

            public void DisposeAndClear()
            {
                Geometry = null;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if (geometry != null)
                        { geometry.PropertyChanged -= OnGeometryPropertyChangedPrivate; }
                        elementCore.TransformChanged -= OnTransformChanged;
                        OnBoundChanged = null;
                        OnTransformBoundChanged = null;
                        OnBoundSphereChanged = null;
                        OnTransformBoundSphereChanged = null;
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~GeometryBoundManager() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
    }

}
