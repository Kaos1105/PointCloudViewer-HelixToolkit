﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Point = Windows.Foundation.Point;

namespace HelixToolkit.UWP
{
    using Model;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3DCore" />
    [TemplatePart(Name = "PART_Container", Type = typeof(ContentPresenter))]
    public abstract class Element3D : Element3DCore
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
        public static readonly DependencyProperty HxTransform3DProperty =
            DependencyProperty.Register("HxTransform3D", typeof(Matrix), typeof(Element3D), new PropertyMetadata(Matrix.Identity,
                (d, e) =>
                {
                    (d as Element3DCore).SceneNode.ModelMatrix = (Matrix)e.NewValue;
                }));

        /// <summary>
        /// 
        /// </summary>
        public Matrix HxTransform3D
        {
            get { return (Matrix)this.GetValue(HxTransform3DProperty); }
            set { this.SetValue(HxTransform3DProperty, value); }
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
            DependencyProperty.Register("RenderOrder", typeof(int), typeof(Element3D), new PropertyMetadata(0, (d, e) =>
            {
                (d as Element3D).SceneNode.RenderOrder = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, (int)e.NewValue));
            }));

        #endregion
        private static readonly Size oneSize = new Size(1, 1);

        private ContentPresenter presenter;
        private FrameworkElement child;
        /// <summary>
        /// Initializes a new instance of the <see cref="Element3D"/> class.
        /// </summary>
        public Element3D()
        {
            this.DefaultStyleKey = typeof(Element3D);
            RegisterPropertyChangedCallback(VisibilityProperty, (s, e) =>
            {
                SceneNode.Visible = (Visibility)s.GetValue(e) == Visibility.Visible && IsRendering;
            });

            RegisterPropertyChangedCallback(IsHitTestVisibleProperty, (s, e) =>
            {
                SceneNode.IsHitTestVisible = (bool)s.GetValue(e);
            });
            OnSceneNodeCreated += Element3D_OnSceneNodeCreated;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            presenter = GetTemplateChild("PART_Container") as ContentPresenter;
            if (presenter == null)
            {
                throw new Exception("Template must contain a ContentPresenter named as PART_Container.");
            }
            presenter.Content = child;
        }

        protected void AttachChild(FrameworkElement child)
        {
            this.child = child;
            if (presenter != null)
            {
                presenter.Content = child;
            }
        }

        private void Element3D_OnSceneNodeCreated(object sender, SceneNodeCreatedEventArgs e)
        {
            e.Node.MouseDown += Node_MouseDown;
            e.Node.MouseUp += Node_MouseUp;
            e.Node.MouseMove += Node_MouseMove;
        }

        private void Node_MouseMove(object sender, Model.Scene.SceneNodeMouseMoveArgs e)
        {
            RaiseMouseMoveEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
        }

        private void Node_MouseUp(object sender, Model.Scene.SceneNodeMouseUpArgs e)
        {
            RaiseMouseUpEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
        }

        private void Node_MouseDown(object sender, Model.Scene.SceneNodeMouseDownArgs e)
        {
            RaiseMouseDownEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return oneSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return oneSize;
        }

        ///// <summary>
        ///// Invoked whenever application code or internal processes(such as a rebuilding layout pass) call ApplyTemplate.In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        ///// </summary>
        //protected override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    itemsContainer = GetTemplateChild("PART_ItemsContainer") as ItemsControl;
        //    itemsContainer?.Items.Clear();
        //}

        #region Events
        public event EventHandler<MouseDown3DEventArgs> OnMouse3DDown;

        public event EventHandler<MouseUp3DEventArgs> OnMouse3DUp;

        public event EventHandler<MouseMove3DEventArgs> OnMouse3DMove;

        internal void RaiseMouseDownEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
        {
            OnMouse3DDown?.Invoke(this, new MouseDown3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
        }

        internal void RaiseMouseUpEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
        {
            OnMouse3DUp?.Invoke(this, new MouseUp3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
        }

        internal void RaiseMouseMoveEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
        {
            OnMouse3DMove?.Invoke(this, new MouseMove3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
        }
        #endregion
    }

    public abstract class Mouse3DEventArgs
    {
        public HitTestResult HitTestResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public Point Position { get; private set; }
        /// <summary>
        /// The original mouse/touch event that generated this one.
        /// 
        /// Useful for knowing what mouse button got pressed.
        /// </summary>
        public PointerRoutedEventArgs OriginalInputEventArgs { get; private set; }

        public Mouse3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
        {
            this.HitTestResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
            this.OriginalInputEventArgs = originalInputEventArgs;
        }
    }

    public sealed class MouseDown3DEventArgs : Mouse3DEventArgs
    {
        public MouseDown3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
            : base(hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }

    public sealed class MouseUp3DEventArgs : Mouse3DEventArgs
    {
        public MouseUp3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
            : base(hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }

    public sealed class MouseMove3DEventArgs : Mouse3DEventArgs
    {
        public MouseMove3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null, PointerRoutedEventArgs originalInputEventArgs = null)
            : base(hitTestResult, position, viewport, originalInputEventArgs)
        { }
    }
}
