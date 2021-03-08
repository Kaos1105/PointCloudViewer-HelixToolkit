﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

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
    namespace Core.Components
    {
        using Render;
        using Shaders;
        using System;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public sealed class ConstantBufferComponent : CoreComponent
        {
            /// <summary>
            /// Gets or sets the model constant buffer.
            /// </summary>
            /// <value>
            /// The model constant buffer.
            /// </value>
            public ConstantBufferProxy ModelConstBuffer { private set; get; }
            private readonly ConstantBufferDescription bufferDesc;
            private readonly byte[] internalByteArray;
            private bool IsValid;
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
            /// </summary>
            /// <param name="desc">The desc.</param>
            public ConstantBufferComponent(ConstantBufferDescription desc)
            {
                bufferDesc = desc;
                if (desc != null)
                {
                    internalByteArray = new byte[desc.StructSize];
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="structSize">Size of the structure.</param>
            public ConstantBufferComponent(string name, int structSize)
            {
                bufferDesc = new ConstantBufferDescription(name, structSize);
                internalByteArray = new byte[structSize];
            }

            protected override void OnAttach(IRenderTechnique technique)
            {
                if(bufferDesc != null)
                {
                    ModelConstBuffer = technique.ConstantBufferPool.Register(bufferDesc);              
                }
                IsValid = bufferDesc != null && ModelConstBuffer != null;
            }

            protected override void OnDetach()
            {
                ModelConstBuffer = null;
                IsValid = false;
            }
            /// <summary>
            /// Uploads the specified device context. This uploads struct only. Ignores internal byte buffer.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="deviceContext">The device context.</param>
            /// <param name="data">The data.</param>
            public bool Upload<T>(DeviceContextProxy deviceContext, ref T data) where T : struct
            {
                if(IsValid && IsAttached)
                {
                    ModelConstBuffer.UploadDataToBuffer(deviceContext, ref data);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Uploads the specified device context. This uploads internal byte buffer only.
            /// </summary>
            /// <param name="deviceContext">The device context.</param>
            public bool Upload(DeviceContextProxy deviceContext)
            {
                if (IsValid && IsAttached)
                {
                    ModelConstBuffer.UploadDataToBuffer(deviceContext, internalByteArray, internalByteArray.Length);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Uploads the specified device context. This function writes a external struct and writes remains byte buffer by offset = input struct size/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="deviceContext">The device context.</param>
            /// <param name="data">The data.</param>
            /// <param name="structSize">The input struct size by bytes</param>
            /// <returns></returns>
            public bool Upload<T>(DeviceContextProxy deviceContext, ref T data, int structSize) where T : struct
            {
                if (IsValid && IsAttached)
                {
                    if (structSize > internalByteArray.Length)
                    {
    #if DEBUG
                        throw new ArgumentOutOfRangeException($"Try to write value out of range. StructureSize {structSize} > Internal Buffer Size {internalByteArray.Length}");
    #else
                        return false;
    #endif
                    }
                    using (var stream = ModelConstBuffer.Map(deviceContext))
                    {
                        stream.Write(data);
                        if (stream.RemainingLength > 0 && stream.RemainingLength >= internalByteArray.Length - structSize)
                        {
                            stream.Write(internalByteArray, structSize, internalByteArray.Length - structSize);
                        }
                    }
                    ModelConstBuffer.Unmap(deviceContext);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Writes the value into internal byte buffer
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="name">The variable name.</param>
            /// <param name="value">The value.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void WriteValueByName<T>(string name, T value) where T : struct
            {
                if (IsValid && IsAttached)
                {
                    if (ModelConstBuffer.TryGetVariableByName(name, out ConstantBufferVariable variable))
                    {
                        if(global::SharpDX.Utilities.SizeOf<T>() > variable.Size)
                        {
                            int structSize = global::SharpDX.Utilities.SizeOf<T>();
                            throw new ArgumentException($"Input struct size {structSize} is larger than shader variable {variable.Name} size {variable.Size}");
                        }
                        global::SharpDX.Utilities.Pin(internalByteArray, (ptr) =>
                        {
                            var offPtr = global::SharpDX.Utilities.IntPtrAdd(ptr, variable.StartOffset);
                            global::SharpDX.Utilities.Write(offPtr, ref value);
                        });
                    }
                    else
                    {
    #if DEBUG
                        throw new ArgumentException($"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
    #else
                        Technique.EffectsManager.Logger.Log(Logger.LogLevel.Warning, $"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
    #endif
                    }
                }
            }
            /// <summary>
            /// Writes the value into internal byte buffer
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value">The value.</param>
            /// <param name="offset">The offset.</param>
            public void WriteValue<T>(T value, int offset) where T : struct
            {
                if (IsValid)
                {
                    if (global::SharpDX.Utilities.SizeOf<T>() > internalByteArray.Length - offset)
                    {
                        int structSize = global::SharpDX.Utilities.SizeOf<T>();
                        throw new ArgumentOutOfRangeException($"Try to write value out of range. StructureSize {structSize} + Offset {offset} > Internal Buffer Size {internalByteArray.Length}");
                    }
                    global::SharpDX.Utilities.Pin(internalByteArray, (ptr) =>
                    {
                        var offPtr = global::SharpDX.Utilities.IntPtrAdd(ptr, offset);
                        global::SharpDX.Utilities.Write(offPtr, ref value);
                    });
                }
            }

            public bool ReadValueByName<T>(string name, out T value) where T : struct
            {
                var v = default(T);
                if (IsValid && IsAttached)
                {
                    if (ModelConstBuffer.TryGetVariableByName(name, out ConstantBufferVariable variable))
                    {
                        if (global::SharpDX.Utilities.SizeOf<T>() > variable.Size)
                        {
                            int structSize = global::SharpDX.Utilities.SizeOf<T>();
                            throw new ArgumentException($"Input struct size {structSize} is larger than shader variable {variable.Name} size {variable.Size}");
                        }
                        global::SharpDX.Utilities.Pin(internalByteArray, (ptr) =>
                        {
                            var offPtr = global::SharpDX.Utilities.IntPtrAdd(ptr, variable.StartOffset);
                            global::SharpDX.Utilities.Read(offPtr, ref v);                     
                        });
                        value = v;
                        return true;
                    }
                    else
                    {
    #if DEBUG
                        throw new ArgumentException($"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
    #else
                        Technique.EffectsManager.Logger.Log(Logger.LogLevel.Warning, $"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
                        value = v;
                        return false;
    #endif
                    }
                }
                else
                {
                    value = v;
                    return false;
                }
            }

            public bool ReadValue<T>(int offset, out T value) where T : struct
            {
                var v = default(T);
                if (IsValid)
                {
                    if (global::SharpDX.Utilities.SizeOf<T>() > internalByteArray.Length - offset)
                    {
                        int structSize = global::SharpDX.Utilities.SizeOf<T>();
                        throw new ArgumentOutOfRangeException($"Try to read value out of range. StructureSize {structSize} + Offset {offset} > Internal Buffer Size {internalByteArray.Length}");
                    }
                    global::SharpDX.Utilities.Pin(internalByteArray, (ptr) =>
                    {
                        var offPtr = global::SharpDX.Utilities.IntPtrAdd(ptr, offset);                  
                        global::SharpDX.Utilities.Read(offPtr, ref v);                    
                    });
                    value = v;
                    return true;
                }
                else
                {
                    value = v;
                    return false;
                }
            }
        }
    }

}
