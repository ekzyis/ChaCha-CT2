using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    public class ResultType
    {
        public static readonly ResultType<uint> QR_INPUT_A = new ResultType<uint>("QR_INPUT_A");
        public static readonly ResultType<uint> QR_INPUT_B = new ResultType<uint>("QR_INPUT_B");
        public static readonly ResultType<uint> QR_INPUT_C = new ResultType<uint>("QR_INPUT_C");
        public static readonly ResultType<uint> QR_INPUT_D = new ResultType<uint>("QR_INPUT_D");
        public static readonly ResultType<uint> QR_INPUT_X1 = new ResultType<uint>("QR_INPUT_X1");
        public static readonly ResultType<uint> QR_INPUT_X2 = new ResultType<uint>("QR_INPUT_X2");
        public static readonly ResultType<uint> QR_INPUT_X3 = new ResultType<uint>("QR_INPUT_X3");
        public static readonly ResultType<uint> QR_OUTPUT_X1 = new ResultType<uint>("QR_OUTPUT_X1");
        public static readonly ResultType<uint> QR_OUTPUT_X2 = new ResultType<uint>("QR_OUTPUT_X2");
        public static readonly ResultType<uint> QR_OUTPUT_X3 = new ResultType<uint>("QR_OUTPUT_X3");
        public static readonly ResultType<uint> QR_ADD = new ResultType<uint>("QR_ADD");
        public static readonly ResultType<uint> QR_XOR = new ResultType<uint>("QR_XOR");
        public static readonly ResultType<uint> QR_SHIFT = new ResultType<uint>("QR_SHIFT");
        public static readonly ResultType<uint> QR_OUTPUT_A = new ResultType<uint>("QR_OUTPUT_A");
        public static readonly ResultType<uint> QR_OUTPUT_B = new ResultType<uint>("QR_OUTPUT_B");
        public static readonly ResultType<uint> QR_OUTPUT_C = new ResultType<uint>("QR_OUTPUT_C");
        public static readonly ResultType<uint> QR_OUTPUT_D = new ResultType<uint>("QR_OUTPUT_D");
        public static readonly ResultType<uint[]> CHACHA_HASH_ORIGINAL_STATE = new ResultType<uint[]>("CHACHA_HASH_ORIGINAL_STATE");
        public static readonly ResultType<uint[]> CHACHA_HASH_QUARTERROUND = new ResultType<uint[]>("CHACHA_HASH_QUARTERROUND");
        public static readonly ResultType<uint[]> CHACHA_HASH_ADD_ORIGINAL_STATE = new ResultType<uint[]>("CHACHA_HASH_ADD_ORIGINAL_STATE");
        public static readonly ResultType<uint[]> CHACHA_HASH_LITTLEENDIAN_STATE = new ResultType<uint[]>("CHACHA_HASH_LITTLEENDIAN_STATE");
    }
    public class ResultType<T> {
        public ResultType(string name)
        {
            this.Name = name;
        }

        public string Name { get;  }
    }
    public class IntermediateResultsManager<T>
    {
       
        public class IntermediateResultsList
        {
            private readonly List<T> _results;
            private ResultType<T> type;

            public IntermediateResultsList(ResultType<T> type)
            {
                _results = new List<T>();
                Type = type;
            }

            public ResultType<T> Type { get; }
            public T Get(int index)
            {
                return _results[index];
            }
            public void Add(T result)
            {
                _results.Add(result);
            }
            public void Clear()
            {
                _results.Clear();
            }
            public int Size()
            {
                return _results.Count;
            }
        }
        private readonly List<IntermediateResultsList> _intermediateResultsList = new List<IntermediateResultsList>();
        private bool TypeExists(ResultType<T> type)
        {
            return _intermediateResultsList.Exists(list => list.Type == type);
        }
        public IntermediateResultsList GetList(ResultType<T> type)
        {
            if (!TypeExists(type))
            {
                return null;
            }
            return _intermediateResultsList.Find(list => list.Type == type);
        }
        public void Clear()
        {
            foreach (IntermediateResultsList r in _intermediateResultsList)
            {
                r.Clear();
            }
        }
        public void AddResult(ResultType<T> type, T result)
        {
            if (!TypeExists(type))
            {
                _intermediateResultsList.Add(new IntermediateResultsList(type));
            }
            GetList(type).Add(result);
        }

        public T Get(ResultType<T> type, int index)
        {
            IntermediateResultsList list = GetList(type);
            if (list == null)
            {
                throw new ArgumentException(string.Format("InterimResultList of type {0}, index {1} does not exist", type.ToString(), index));
            }
            return list.Get(index);
        }
    }
}
