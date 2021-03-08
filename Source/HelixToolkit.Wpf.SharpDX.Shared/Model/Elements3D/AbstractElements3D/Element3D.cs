﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for renderable elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using SharpDX;
using System.Windows;
using Media = System.Windows.Media;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using System;
using System.Windows.Input;
using Point = System.Windows.Point;

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
{

    using Model;
#if !COREWPF
    using Model.Scene;
#endif

    /// <summary>
    /// Base class for renderable elements.
    /// </summary>    
    public abstract class Element3D : Element3DCore, IVisible
    {
#region Dependency Properties
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3D).SceneNode.Visible = (bool)e.NewValue && (d as Element3D).Visibility == Visibility.Visible;
                }));

        /// <summary>
        /// Indicates, if this element should be rendered.
        /// Use this also to make the model visible/unvisible
        /// default is true
        /// </summary>
        public bool IsRendering
        {
            get { return (bool)GetValue(IsRenderingProperty); }
            set { SetValue(IsRenderingProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element3D), new PropertyMetadata(Visibility.Visible, (d, e) =>
            {
                (d as Element3D).SceneNode.Visible = (Visibility)e.NewValue == Visibility.Visible && (d as Element3D).IsRendering;
            }));

        /// <summary>
        /// 
        /// </summary>
        public Visibility Visibility
        {
            set
            {
                SetValue(VisibilityProperty, value);
            }
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Element3D), new PropertyMetadata(Transform3D.Identity,
                (d,e)=>
                {
                    (d as Element3D).SceneNode.ModelMatrix = e.NewValue != null ? (e.NewValue as Transform3D).Value.ToMatrix() : Matrix.Identity;
                }));
        /// <summary>
        /// 
        /// </summary>
        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        /// <summary>
        /// The is hit test visible property
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element3D),
            new PropertyMetadata(true, (d, e) => {
                (d as Element3D).SceneNode.IsHitTestVisible = (bool)e.NewValue;
            }));

        /// <summary>
        /// Indicates, if this element should be hit-tested.
        /// default is true
        /// </summary>
        public bool IsHitTestVisible
        {
            set
            {
                SetValue(IsHitTestVisibleProperty, value);
            }
            get
            {
                return (bool)GetValue(IsHitTestVisibleProperty);
            }
        }


        /// <summary>
        /// Gets or sets the manual render order.
        /// </summary>
        /// <value>
        /// The render order.
        /// </value>
        public int RenderOrder
        {
            get { return (int)GetValue(RenderOrderProperty); }
            set { SetValue(RenderOrderProperty, value); }
        }

        /// <summary>
        /// The render order property
        /// </summary>
        public static readonly DependencyProperty RenderOrderProperty =
            DependencyProperty.Register("RenderOrder", typeof(int), typeof(Element3D), new PropertyMetadata(0, (d,e) =>
            {
                (d as Element3D).SceneNode.RenderOrder = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, (int)e.NewValue));
            }));
#endregion

#region Events
        public static readonly RoutedEvent MouseDown3DEvent =
            EventManager.RegisterRoutedEvent("MouseDown3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

        public static readonly RoutedEvent MouseUp3DEvent =
            EventManager.RegisterRoutedEvent("MouseUp3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

        public static readonly RoutedEvent MouseMove3DEvent =
            EventManager.RegisterRoutedEvent("MouseMove3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseDown3D
        {
            add { AddHandler(MouseDown3DEvent, value); }
            remove { RemoveHandler(MouseDown3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseUp3D
        {
            add { AddHandler(MouseUp3DEvent, value); }
            remove { RemoveHandler(MouseUp3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseMove3D
        {
            add { AddHandler(MouseMove3DEvent, value); }
            remove { RemoveHandler(MouseMove3DEvent, value); }
        }

        protected virtual void OnMouse3DDown(object sender, RoutedEventArgs e)
        {
            Mouse3DDown?.Invoke(this, e as MouseDown3DEventArgs);
        }

        protected virtual void OnMouse3DUp(object sender, RoutedEventArgs e)
        {
            Mouse3DUp?.Invoke(this, e as MouseUp3DEventArgs);
        }

        protected virtual void OnMouse3DMove(object sender, RoutedEventArgs e)
        {
            Mouse3DMove?.Invoke(this, e as MouseMove3DEventArgs);
        }

        public event EventHandler<MouseDown3DEventArgs> Mouse3DDown;
        public event EventHandler<MouseUp3DEventArgs> Mouse3DUp;
        public event EventHandler<MouseMove3DEventArgs> Mouse3DMove;
#endregion

        public Element3D()
        {
            this.MouseDown3D += OnMouse3DDown;
            this.MouseUp3D += OnMouse3DUp;
            this.MouseMove3D += OnMouse3DMove;
            OnSceneNodeCreated += Element3D_OnSceneNodeCreated;
        }

        private void Element3D_OnSceneNodeCreated(object sender, SceneNodeCreatedEventArgs e)
        {
            e.Node.MouseDown += Node_MouseDown;
            e.Node.MouseMove += Node_MouseMove;
            e.Node.MouseUp += Node_MouseUp;
        }

        private void Node_MouseUp(object sender, SceneNodeMouseUpArgs e)
        {
            RaiseEvent(new MouseUp3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
        }

        private void Node_MouseMove(object sender, SceneNodeMouseMoveArgs e)
        {
            RaiseEvent(new MouseMove3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
        }

        private void Node_MouseDown(object sender, SceneNodeMouseDownArgs e)
        {
            RaiseEvent(new MouseDown3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
        }

        /// <summary>
        /// Looks for the first visual ancestor of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of visual ancestor.</typeparam>
        /// <param name="obj">The respective <see cref="DependencyObject"/>.</param>
        /// <returns>
        /// The first visual ancestor of type <typeparamref name="T"/> if exists, else <c>null</c>.
        /// </returns>
        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    var typed = parent as T;
                    if (typed != null)
                    {
                        return typed;
                    }

                    parent = Media.VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    var pm = e.Property.GetMetadata(this);
        //    if (pm is FrameworkPropertyMetadata fm)
        //    {
        //        if (fm.AffectsRender)
        //        {
        //            InvalidateRender();
        //        }
        //    }
        //    base.OnPropertyChanged(e);
        //}
    }

    public abstract class Mouse3DEventArgs : RoutedEventArgs
    {
        public HitTestResult HitTestResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public Point Position { get; private set; }
        /// <summary>
        /// The original mouse/touch event that generated this one.
        /// 
        /// Useful for knowing what mouse button got pressed.
        /// </summary>
        public InputEventArgs OriginalInputEventArgs { get; private set; }

        public Mouse3DEventArgs(RoutedEvent routedEvent, object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, InputEventArgs originalInputEventArgs = null)
            : base(routedEvent, source)
        {
            this.HitTestResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
            this.OriginalInputEventArgs = originalInputEventArgs;
        }
    }

    public class MouseDown3DEventArgs : Mouse3DEventArgs
    {
        public MouseDown3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, InputEventArgs originalInputEventArgs = null)
            : base(Element3D.MouseDown3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }

    public class MouseUp3DEventArgs : Mouse3DEventArgs
    {
        public MouseUp3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, InputEventArgs originalInputEventArgs = null)
            : base(Element3D.MouseUp3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }

    public class MouseMove3DEventArgs : Mouse3DEventArgs
    {
        public MouseMove3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, InputEventArgs originalInputEventArgs = null)
            : base(Element3D.MouseMove3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }
}
