﻿//using System.Runtime.CompilerServices;
using Bssom.Serializer.Internal;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssMap
{
    internal struct BssmapAnalysisStack : Iterable<byte>
    {
        private ArrayPack<ulong> _values;
        private Stack<BssMapRouteToken> _tokens;
        private byte _lastValueByteCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BssmapAnalysisStack(int size)
        {
            _values = new ArrayPack<ulong>(size);
            _tokens = new Stack<BssMapRouteToken>(size);
            _lastValueByteCount = 0;
        }

        public int TokenCount => _tokens.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BssMapRouteToken PeekToken()
        {
            return _tokens.Peek();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopValue()
        {
            _values.RollbackOne();
            if (_values.NextPos == 0)
            {
                _lastValueByteCount = 0;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopToken()
        {
            _tokens.Pop();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushValue(ulong val)
        {
            _values.Add(val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushToken(BssMapRouteToken branchToken)
        {
            _tokens.Push(branchToken);
        }

        public Iterable<byte> ToUlongs(byte lastValueByteCount)
        {
            _lastValueByteCount = lastValueByteCount;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetFirstElementReference(out bool isContiguousMemoryArea)
        {
            isContiguousMemoryArea = true;
            return ref Unsafe.As<ulong, byte>(ref _values.GetArray()[0]);
        }

        public IEnumerable<byte> Ts
        {
            get
            {
                for (int i = 0; i < Length; i++)
                {
                    yield return Unsafe.Add(ref GetFirstElementReference(out bool isContiguousMemoryArea), i);
                }
            }
        }

        public int Length => _values.NextPos == 0 ? 0 : (_values.NextPos - 1) * 8 + _lastValueByteCount;
    }
}
