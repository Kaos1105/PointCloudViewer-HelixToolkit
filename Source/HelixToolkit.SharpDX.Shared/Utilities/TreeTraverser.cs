﻿using System;
using System.Collections.Generic;
using System.Text;
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
    using Core;
    using Core2D;
    using Model.Scene;
    using Model.Scene2D;

    public static class TreeTraverser
    {
        /// <summary>
        /// Traverses up to the root
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> TraverseUp(this SceneNode node)
        {
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }
        /// <summary>
        /// Forces to update transform and bounds.
        /// </summary>
        /// <param name="root">The root.</param>
        public static void ForceUpdateTransformsAndBounds(this SceneNode root)
        {
            var nodes = Enumerable.Repeat(root, 1);
            foreach(var n in nodes.Traverse())
            {
                n.ComputeTransformMatrix();
            }
        }
        /// <summary>
        /// Forces the update transform and bounds.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public static void ForceUpdateTransformsAndBounds(this IEnumerable<SceneNode> nodes)
        {
            foreach (var n in nodes.Traverse())
            {
                n.ComputeTransformMatrix();
            }
        }
        /// <summary>
        /// Traverses the specified only rendering.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="onlyRendering">if set to <c>true</c> [only rendering].</param>
        /// <param name="stackCache">The stack cache.</param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> Traverse(this SceneNode root, bool onlyRendering = false,
            Stack<IEnumerator<SceneNode>> stackCache = null)
        {
            var nodes = Enumerable.Repeat(root, 1);
            return PreorderDFT(nodes, (n) => { return onlyRendering ? n.IsRenderable : true; }, stackCache);
        }
        /// <summary>
        /// Traverses the specified condition.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="onlyRendering">Only the node is currently rendering. The nodes set to be Visible = false or IsRendering = false will not be traversed. </param>
        /// <param name="stackCache">The stack cache.</param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> Traverse(this IEnumerable<SceneNode> nodes, bool onlyRendering = false,
            Stack<IEnumerator<SceneNode>> stackCache = null)
        {
            return PreorderDFT(nodes, (n)=> { return onlyRendering ? n.IsRenderable : true; }, stackCache);
        }
        /// <summary>
        /// Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> PreorderDFT(this IEnumerable<SceneNode> nodes, Func<SceneNode, bool> condition, 
            Stack<IEnumerator<SceneNode>> stackCache = null)
        {
            var stack = stackCache ?? new Stack<IEnumerator<SceneNode>>(20);
            var e = nodes.GetEnumerator();

            while (true)
            {
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!condition(item)) { continue; }
                    yield return item;
                    var elements = item.ItemsInternal;
                    if (elements.Count == 0)
                    {
                        continue;
                    }
                    stack.Push(e);
                    e = elements.GetEnumerator();
                }
                if (stack.Count == 0) break;
                e.Dispose();
                e = stack.Pop();
            }

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }
        
        /// <summary>
        /// Preorders the DFT without using Linq.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="context">The context.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="results">The results.</param>
        /// <param name="stackCache">The stack cache.</param>
        public static void PreorderDFT(this IList<SceneNode> nodes, RenderContext context,
            Func<SceneNode, RenderContext, bool> condition, IList<KeyValuePair<int, SceneNode>> results,
            Stack<KeyValuePair<int, IList<SceneNode>>> stackCache = null)
        {
            var stack = stackCache ?? new Stack<KeyValuePair<int, IList<SceneNode>>>(20);
            int i = -1;
            int level = 0;
            IList<SceneNode> currNodes = nodes;
            while (true)
            {
                var length = currNodes.Count;
                while(++i < length)
                {
                    var item = currNodes[i];              
                    if (!condition(item, context))
                    { continue; }
                    results.Add(new KeyValuePair<int, SceneNode>(level, item));
                    var elements = item.ItemsInternal;
                    if(elements.Count == 0)
                    { continue; }
                    stack.Push(new KeyValuePair<int, IList<SceneNode>>(i, currNodes));
                    i = -1;
                    ++level;
                    currNodes = elements;
                    length = currNodes.Count;
                }
                if (stack.Count == 0)
                { break; }
                var prev = stack.Pop();
                i = prev.Key;
                --level;
                currNodes = prev.Value;
            }
        }

        /// <summary>
        /// Get render cores by Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<RenderCore> PreorderDFTGetCores(this IEnumerable<SceneNode> nodes, Func<SceneNode, bool> condition,
            Stack<IEnumerator<SceneNode>> stackCache = null)
        {
            var stack = stackCache ?? new Stack<IEnumerator<SceneNode>>(20);
            var e = nodes.GetEnumerator();

            while (true)
            {
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!condition(item)) { continue; }
                    yield return item.RenderCore;
                    var elements = item.ItemsInternal;
                    if (elements.Count == 0)
                    { continue; }
                    stack.Push(e);
                    e = elements.GetEnumerator();
                }
                if (stack.Count == 0) break;
                e.Dispose();
                e = stack.Pop();
            }

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }

        /// <summary>
        /// Preorders the DFT without Linq;
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="stackCache">The stack cache.</param>
        public static void PreorderDFTRun(this IList<SceneNode2D> nodes, Func<SceneNode2D, bool> condition, 
            Stack<KeyValuePair<int, IList<SceneNode2D>>> stackCache = null)
        {
            var stack = stackCache ?? new Stack<KeyValuePair<int, IList<SceneNode2D>>>(20);
            int i = -1;
            while (true)
            {
                while (++i < nodes.Count)
                {
                    var item = nodes[i];
                    if (!condition(item))
                    { continue; }
                    var elements = item.ItemsInternal;
                    if (elements.Count == 0)
                    { continue; }
                    stack.Push(new KeyValuePair<int, IList<SceneNode2D>>(i, nodes));
                    i = -1;
                    nodes = elements;
                }
                if (stack.Count == 0)
                { break; }
                var prev = stack.Pop();
                i = prev.Key;
                nodes = prev.Value;
            }
        }

        /// <summary>
        /// Get render cores by Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<RenderCore2D> PreorderDFTGetCores(this IEnumerable<SceneNode2D> nodes, Func<SceneNode2D, bool> condition,
            Stack<IEnumerator<SceneNode2D>> stackCache = null)
        {
            var stack = stackCache ?? new Stack<IEnumerator<SceneNode2D>>(20);
            var e = nodes.GetEnumerator();

            while (true)
            {
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!condition(item)) { continue; }
                    yield return item.RenderCore;
                    var elements = item.ItemsInternal;
                    if (elements.Count == 0)
                    { continue; }
                    stack.Push(e);
                    e = elements.GetEnumerator();
                }
                if (stack.Count == 0) break;
                e.Dispose();
                e = stack.Pop();
            }

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }
    }
}
