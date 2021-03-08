﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SDX11 = SharpDX.Direct3D11;
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
        using Render;
    

        /// <summary>
        ///
        /// </summary>
        public interface IElementsBufferProxy : IBufferProxy
        {
            /// <summary>
            ///
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct;

            /// <summary>
            ///
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            /// <param name="offset"></param>
            /// <param name="minBufferCount">Used to initialize a buffer which size is Max(count, minBufferCount). Only used in dynamic buffer.</param>
            void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct;

            /// <summary>
            /// Uploads the data to buffer using data pointer.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="data">The data.</param>
            /// <param name="countByBytes">The count by bytes.</param>
            /// <param name="offsetByBytes">The offset by bytes.</param>
            /// <param name="minBufferCountByBytes">The minimum buffer count by bytes.</param>
            unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int countByBytes, int offsetByBytes, int minBufferCountByBytes = default(int));
            /// <summary>
            /// Creates the buffer with size = count * structure size;
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="count">The count.</param>
            void CreateBuffer(DeviceContextProxy context, int count);

            /// <summary>
            /// <see cref="DisposeObject.DisposeAndClear"/>
            /// </summary>
            void DisposeAndClear();
        }

        /// <summary>
        ///
        /// </summary>
        public sealed class ImmutableBufferProxy : BufferProxyBase, IElementsBufferProxy
        {
            /// <summary>
            ///
            /// </summary>
            public ResourceOptionFlags OptionFlags { private set; get; }
            public ResourceUsage Usage { private set; get; } = ResourceUsage.Immutable;

            public CpuAccessFlags CpuAccess { private set; get; } = CpuAccessFlags.None;
            /// <summary>
            ///
            /// </summary>
            /// <param name="structureSize"></param>
            /// <param name="bindFlags"></param>
            /// <param name="optionFlags"></param>
            /// <param name="usage"></param>
            public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Immutable)
                : base(structureSize, bindFlags)
            {
                OptionFlags = optionFlags;
                Usage = usage;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableBufferProxy"/> class.
            /// </summary>
            /// <param name="structureSize">Size of the structure.</param>
            /// <param name="bindFlags">The bind flags.</param>
            /// <param name="cpuAccess">The cpu access.</param>
            /// <param name="optionFlags">The option flags.</param>
            /// <param name="usage">The usage.</param>
            public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, 
                CpuAccessFlags cpuAccess,
                ResourceOptionFlags optionFlags = ResourceOptionFlags.None, 
                ResourceUsage usage = ResourceUsage.Immutable)
                : base(structureSize, bindFlags)
            {
                OptionFlags = optionFlags;
                Usage = usage;
                CpuAccess = cpuAccess;
            }

            /// <summary>
            /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct
            {
                UploadDataToBuffer<T>(context, data, count, 0);
            }

            /// <summary>
            /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int, int, int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            /// <param name="offset"></param>
            /// <param name="minBufferCount">This is not being used in ImmutableBuffer</param>
            public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct
            {
                RemoveAndDispose(ref buffer);
                ElementCount = count;
                if(count == 0)
                {
                    return;
                }
                var buffdesc = new BufferDescription()
                {
                    BindFlags = this.BindFlags,
                    CpuAccessFlags = CpuAccess,
                    OptionFlags = this.OptionFlags,
                    SizeInBytes = StructureSize * count,
                    StructureByteStride = StructureSize,
                    Usage = Usage
                };
                buffer = Collect(Buffer.Create(context, data.GetArrayByType(), buffdesc));
            }

            /// <summary>
            /// Uploads the data to buffer using data pointer.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="data">The data pointer.</param>
            /// <param name="countByBytes">The count by bytes.</param>
            /// <param name="offsetByBytes">The offset by bytes.</param>
            /// <param name="minBufferCountByBytes">The minimum buffer count by bytes.</param>
            public unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int countByBytes, int offsetByBytes, int minBufferCountByBytes = default(int))
            {
                RemoveAndDispose(ref buffer);
                ElementCount = countByBytes / StructureSize;
                if (countByBytes == 0)
                {
                    return;
                }
                var buffdesc = new BufferDescription()
                {
                    BindFlags = this.BindFlags,
                    CpuAccessFlags = CpuAccess,
                    OptionFlags = this.OptionFlags,
                    SizeInBytes = countByBytes,
                    StructureByteStride = StructureSize,
                    Usage = Usage
                };
                buffer = Collect(new Buffer(context, data, buffdesc));
            }
            /// <summary>
            /// Creates the buffer with size of count * structure size.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="count">The element count.</param>
            public void CreateBuffer(DeviceContextProxy context, int count)
            {
                RemoveAndDispose(ref buffer);
                ElementCount = count;
                if (count == 0)
                {
                    return;
                }
                var buffdesc = new BufferDescription()
                {
                    BindFlags = this.BindFlags,
                    CpuAccessFlags = CpuAccess,
                    OptionFlags = this.OptionFlags,
                    SizeInBytes = StructureSize * count,
                    StructureByteStride = StructureSize,
                    Usage = Usage
                };
                buffer = Collect(new Buffer(context, buffdesc));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public class DynamicBufferProxy : BufferProxyBase, IElementsBufferProxy
        {
            public readonly bool CanOverwrite = false;
            public readonly bool LazyResize = true;
            /// <summary>
            ///
            /// </summary>
            public ResourceOptionFlags OptionFlags { private set; get; }
            /// <summary>
            /// Gets the capacity in bytes.
            /// </summary>
            /// <value>
            /// The capacity.
            /// </value>
            public int Capacity { private set; get; }
            /// <summary>
            /// Gets the capacity used in bytes.
            /// </summary>
            /// <value>
            /// The capacity used.
            /// </value>
            public int CapacityUsed { private set; get; }

            public CpuAccessFlags CpuAccess { private set; get; } = CpuAccessFlags.Write;
            /// <summary>
            ///
            /// </summary>
            /// <param name="structureSize"></param>
            /// <param name="bindFlags"></param>
            /// <param name="optionFlags"></param>
            /// <param name="lazyResize">If existing data size is smaller than buffer size, reuse existing. Otherwise create a new buffer with exact same size</param>
            public DynamicBufferProxy(int structureSize, BindFlags bindFlags, 
                ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
                : base(structureSize, bindFlags)
            {
                CanOverwrite = (bindFlags & (BindFlags.VertexBuffer | BindFlags.IndexBuffer)) != 0;
                this.OptionFlags = optionFlags;
                LazyResize = lazyResize;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicBufferProxy"/> class.
            /// </summary>
            /// <param name="structureSize">Size of the structure.</param>
            /// <param name="bindFlags">The bind flags.</param>
            /// <param name="optionFlags">The option flags.</param>
            /// <param name="lazyResize">if set to <c>true</c> [lazy resize].</param>
            /// <param name="canOverWrite">if set to <c>true</c> [can over write].</param>
            public DynamicBufferProxy(int structureSize, BindFlags bindFlags, bool canOverWrite,
                ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
                : base(structureSize, bindFlags)
            {
                CanOverwrite = canOverWrite;
                this.OptionFlags = optionFlags;
                LazyResize = lazyResize;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicBufferProxy"/> class.
            /// </summary>
            /// <param name="structureSize">Size of the structure.</param>
            /// <param name="bindFlags">The bind flags.</param>
            /// <param name="canOverWrite">if set to <c>true</c> [can over write].</param>
            /// <param name="cpuAccess">The cpu access.</param>
            /// <param name="optionFlags">The option flags.</param>
            /// <param name="lazyResize">if set to <c>true</c> [lazy resize].</param>
            public DynamicBufferProxy(int structureSize, BindFlags bindFlags, bool canOverWrite,
                CpuAccessFlags cpuAccess,
                ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
                : base(structureSize, bindFlags)
            {
                CanOverwrite = canOverWrite;
                this.OptionFlags = optionFlags;
                LazyResize = lazyResize;
                CpuAccess = cpuAccess;
            }
            /// <summary>
            /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct
            {
                UploadDataToBuffer<T>(context, data, count, 0);
            }

            /// <summary>
            /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int, int, int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count">Data Count</param>
            /// <param name="offset"></param>
            /// <param name="minBufferCount">Used to create a dynamic buffer with size of Max(count, minBufferCount).</param>
            public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct
            {
                ElementCount = count;
                int newSizeInBytes = StructureSize * count;
                if (count == 0)
                {
                    return;
                }
                EnsureBufferCapacity(context, ElementCount, minBufferCount);
                if(CapacityUsed + newSizeInBytes <= Capacity && !context.IsDeferred && CanOverwrite)
                {
                    Offset = CapacityUsed;
                    context.MapSubresource(this.buffer, MapMode.WriteNoOverwrite, MapFlags.None, out DataStream stream);
                    using (stream)
                    {
                        stream.Position = Offset;
                        stream.WriteRange(data.GetArrayByType(), offset, count);                    
                    }
                    context.UnmapSubresource(this.buffer, 0);
                    CapacityUsed += newSizeInBytes;
                }
                else
                {
                    context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    using (stream)
                    {
                        stream.WriteRange(data.GetArrayByType(), offset, count);
                    }
                    context.UnmapSubresource(this.buffer, 0);
                    Offset = CapacityUsed = 0;
                }
            }

            /// <summary>
            /// Uploads the data pointer to buffer. 
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="data">The data.</param>
            /// <param name="byteCount">The count by bytes.</param>
            /// <param name="byteOffset">The offset by bytes.</param>
            /// <param name="minBufferSizeByBytes">The minimum buffer count by bytes.</param>
            public unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int byteCount, int byteOffset, int minBufferSizeByBytes = default(int))
            {
                ElementCount = byteCount / StructureSize;
                int newSizeInBytes = byteCount;
                if (byteCount == 0)
                {
                    return;
                }
                EnsureBufferCapacity(context, ElementCount, minBufferSizeByBytes / StructureSize);
                if (CapacityUsed + newSizeInBytes <= Capacity && !context.IsDeferred && CanOverwrite)
                {
                    Offset = CapacityUsed;
                    context.MapSubresource(this.buffer, MapMode.WriteNoOverwrite, MapFlags.None, out DataStream stream);
                    using (stream)
                    {
                        stream.Position = Offset;
                        stream.Write(data, byteOffset, byteCount);
                    }
                    context.UnmapSubresource(this.buffer, 0);
                    CapacityUsed += newSizeInBytes;
                }
                else
                {
                    context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    using (stream)
                    {
                        stream.Write(data, byteOffset, byteCount);
                    }
                    context.UnmapSubresource(this.buffer, 0);
                    Offset = CapacityUsed = 0;
                }
            }

            /// <summary>
            /// Ensures the buffer capacity is enough.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="count">The count.</param>
            /// <param name="minSizeCount">The minimum size count.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnsureBufferCapacity(DeviceContextProxy context, int count, int minSizeCount)
            {
                int bytes = count * StructureSize;
                if (buffer == null || Capacity < bytes || (!LazyResize && Capacity != bytes))
                {
                    Initialize(context, count, minSizeCount);
                }
            }

            /// <summary>
            /// Maps the buffer. Make sure to call <see cref="EnsureBufferCapacity(DeviceContextProxy, int, int)"/> to make sure buffer has enough space
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="action">The action.</param>
            public void MapBuffer(DeviceContextProxy context, System.Action<DataStream> action)
            {
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    action(stream);
                }
                context.UnmapSubresource(this.buffer, 0);
                Offset = CapacityUsed = 0;
            }
            /// <summary>
            /// Initializes the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="count">The count.</param>
            /// <param name="minBufferCount">The minimum buffer count.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize(Device device, int count, int minBufferCount = default(int))
            {
                RemoveAndDispose(ref buffer);
                var buffdesc = new BufferDescription()
                {
                    BindFlags = this.BindFlags,
                    CpuAccessFlags = CpuAccess,
                    OptionFlags = this.OptionFlags,
                    SizeInBytes = StructureSize * System.Math.Max(count, minBufferCount),
                    StructureByteStride = StructureSize,
                    Usage = ResourceUsage.Dynamic
                };
                Capacity = buffdesc.SizeInBytes;
                CapacityUsed = 0;
                buffer = Collect(new Buffer(device, buffdesc));
                OnBufferChanged(buffer);
            }

            /// <summary>
            /// Creates the buffer with size of count * structure size.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="count">The element count.</param>
            public void CreateBuffer(DeviceContextProxy context, int count)
            {
                Initialize(context, count);
            }

            protected virtual void OnBufferChanged(Buffer newBuffer) { }
        }

        public sealed class StructuredBufferProxy : DynamicBufferProxy
        {
            private ShaderResourceViewProxy srv;
            public ShaderResourceViewProxy SRV
            {
                get { return srv; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="StructuredBufferProxy"/> class.
            /// </summary>
            /// <param name="structureSize">Size of the structure.</param>
            /// <param name="lazyResize">If existing data size is smaller than buffer size, reuse existing. Otherwise create a new buffer with exact same size</param>
            public StructuredBufferProxy(int structureSize, bool lazyResize = true) :
                base(structureSize, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured, lazyResize)
            {

            }

            protected override void OnBufferChanged(Buffer newBuffer)
            {
                RemoveAndDispose(ref srv);
                srv = Collect(new ShaderResourceViewProxy(newBuffer.Device, newBuffer));
                srv.CreateTextureView();
            }

            public static implicit operator ShaderResourceViewProxy(StructuredBufferProxy proxy)
            {
                return proxy.srv;
            }

            public static implicit operator ShaderResourceView(StructuredBufferProxy proxy)
            {
                return proxy.srv;
            }
        }
    }

}