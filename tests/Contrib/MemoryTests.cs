// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the NOTICE file in the project root for more information.

using System;
using System.Buffers;
using NSec.Cryptography.Buffers;
using Xunit;

namespace NSec.Tests.Contrib
{
    public static class MemoryTests
    {
        [Fact]
        public static void MemoryFromMemoryManagerInt()
        {
            int[] a = { 91, 92, -93, 94 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                Memory<int> memory = manager.Memory;
                memory.Validate(91, 92, -93, 94);
                memory.Slice(0, 4).Validate(91, 92, -93, 94);
                memory.Slice(1, 0).Validate();
                memory.Slice(1, 1).Validate(92);
                memory.Slice(1, 2).Validate(92, -93);
                memory.Slice(2, 2).Validate(-93, 94);
                memory.Slice(4, 0).Validate();
            }
        }

        [Fact]
        public static void MemoryManagerMemoryCtorInvalid()
        {
            int[] a = { 91, 92, -93, 94 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                Assert.Throws<ArgumentOutOfRangeException>(() => manager.Memory.Slice(a.Length + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => manager.Memory.Slice(0, a.Length + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => manager.Memory.Slice(a.Length + 1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => manager.Memory.Slice(1, a.Length));
            }
        }

        [Fact]
        public static void ReadOnlyMemoryFromMemoryFromMemoryManagerInt()
        {
            int[] a = { 91, 92, -93, 94 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                ReadOnlyMemory<int> readOnlyMemory = manager.Memory;
                readOnlyMemory.Validate(91, 92, -93, 94);
                readOnlyMemory.Slice(0, 4).Validate(91, 92, -93, 94);
                readOnlyMemory.Slice(1, 0).Validate();
                readOnlyMemory.Slice(1, 1).Validate(92);
                readOnlyMemory.Slice(1, 2).Validate(92, -93);
                readOnlyMemory.Slice(2, 2).Validate(-93, 94);
                readOnlyMemory.Slice(4, 0).Validate();
            }
        }

        [Fact]
        public static void MemoryFromMemoryManagerLong()
        {
            long[] a = { 91, -92, 93, 94, -95 };
            using (MemoryManager<long> manager = new SecureMemoryManager<long>(a.Length))
            {
                a.CopyTo(manager.Memory);

                Memory<long> memory = manager.Memory;
                memory.Validate(91, -92, 93, 94, -95);
                memory.Slice(0, 5).Validate(91, -92, 93, 94, -95);
                memory.Slice(1, 0).Validate();
                memory.Slice(1, 1).Validate(-92);
                memory.Slice(1, 2).Validate(-92, 93);
                memory.Slice(2, 3).Validate(93, 94, -95);
                memory.Slice(5, 0).Validate();
            }
        }

        [Fact]
        public static void MemoryManagerPinEmptyArray()
        {
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(0))
            {
                MemoryHandle handle = manager.Pin();
                unsafe
                {
                    Assert.True(handle.Pointer != null);
                }
            }
        }

        [Fact]
        public static void MemoryManagerPinArray()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                using (MemoryHandle handle = manager.Pin())
                {
                    unsafe
                    {
                        int* pointer = (int*)handle.Pointer;
                        Assert.True(pointer != null);

                        for (int i = 0; i < manager.Memory.Length; i++)
                        {
                            Assert.Equal(a[i], pointer[i]);
                        }
                    }
                }
            }
        }

        [Fact]
        public static void SpanFromMemoryManagerAfterDispose()
        {
            MemoryManager<int> manager;
            using (manager = new SecureMemoryManager<int>(4))
            {
                manager.GetSpan();
            }
            Assert.Throws<ObjectDisposedException>(() => manager.GetSpan());
        }

        [Fact]
        public static void MemoryManagerPin()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                Memory<int> memory = manager.Memory;
                using (MemoryHandle handle = memory.Pin())
                {
                    unsafe
                    {
                        int* pointer = (int*)handle.Pointer;
                        Assert.True(pointer != null);

                        for (int i = 0; i < memory.Length; i++)
                        {
                            Assert.Equal(a[i], pointer[i]);
                        }
                    }
                }
            }
        }

        [Fact]
        public static void MemoryManagerPinAndSlice()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            using (MemoryManager<int> manager = new SecureMemoryManager<int>(a.Length))
            {
                a.CopyTo(manager.Memory);

                Memory<int> memory = manager.Memory.Slice(1);
                using (MemoryHandle handle = memory.Pin())
                {
                    Span<int> span = memory.Span;
                    unsafe
                    {
                        int* pointer = (int*)handle.Pointer;
                        Assert.True(pointer != null);

                        for (int i = 0; i < memory.Length; i++)
                        {
                            Assert.Equal(a[i + 1], pointer[i]);
                        }

                        for (int i = 0; i < memory.Length; i++)
                        {
                            Assert.Equal(a[i + 1], span[i]);
                        }
                    }
                }
            }
        }

        private static void Validate<T>(this Memory<T> memory, params T[] expected) where T : IEquatable<T>
        {
            Assert.True(memory.Span.SequenceEqual(expected));
        }

        private static void Validate<T>(this ReadOnlyMemory<T> memory, params T[] expected) where T : IEquatable<T>
        {
            Assert.True(memory.Span.SequenceEqual(expected));
        }
    }
}
