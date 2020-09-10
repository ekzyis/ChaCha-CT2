using System;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    public enum ResultType
    {
        QR_INPUT_A, QR_INPUT_B, QR_INPUT_C, QR_INPUT_D,
        QR_INPUT_X1, QR_INPUT_X2, QR_INPUT_X3,
        QR_OUTPUT_X1, QR_OUTPUT_X2, QR_OUTPUT_X3,
        QR_ADD_X1_X2, QR_XOR
    }
    public class IntermediateResultsManager
    {
       
        private class InterimResultList
        {
            private List<uint> _results;

            public InterimResultList(ResultType type)
            {
                _results = new List<uint>();
                Type = type;
            }
            public ResultType Type { get; }
            public uint Get(int index)
            {
                return _results[index];
            }
            public void Add(uint result)
            {
                _results.Add(result);
            }
            public void Clear()
            {
                _results.Clear();
            }
        }
        private List<InterimResultList> _interimResultsList = new List<InterimResultList>();
        private bool TypeExists(ResultType type)
        {
            return _interimResultsList.Exists(list => list.Type == type);
        }
        private InterimResultList GetList(ResultType type)
        {
            if (!TypeExists(type))
            {
                return null;
            }
            return _interimResultsList.Find(list => list.Type == type);
        }
        public void Clear()
        {
            foreach (InterimResultList r in _interimResultsList)
            {
                r.Clear();
            }
            _interimResultsList.Clear();
        }
        public void AddResult(ResultType type, uint result)
        {
            if (!TypeExists(type))
            {
                _interimResultsList.Add(new InterimResultList(type));
            }
            GetList(type).Add(result);
        }
        public uint Get(ResultType type, int index)
        {
            InterimResultList list = GetList(type);
            if (list == null)
            {
                throw new ArgumentException(string.Format("InterimResultList of type {0}, index {1} does not exist", type.ToString(), index));
            }
            return list.Get(index);
        }
    }
}
