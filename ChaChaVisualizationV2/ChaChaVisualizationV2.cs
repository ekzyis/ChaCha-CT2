/*
   Copyright CrypTool 2 Team <ct2contact@cryptool.org>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Cryptool.PluginBase;
using Cryptool.PluginBase.IO;
using Cryptool.Plugins.ChaCha;
using Cryptool.Plugins.ChaCha.Util;
using Cryptool.Plugins.ChaChaVisualizationV2.Model;
using Cryptool.Plugins.ChaChaVisualizationV2.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cryptool.Plugins.ChaChaVisualizationV2
{
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    [PluginInfo("ChaChaVisualizationV2", "Subtract one number from another", "ChaChaVisualizationV2/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    [ComponentCategory(ComponentCategory.CiphersModernSymmetric)]
    public class ChaChaVisualizationV2 : ChaCha.ChaCha, INotifyPropertyChanged
    {
        #region Private Variables

        private readonly ChaChaPresentation presentation;

        #endregion Private Variables

        public ChaChaVisualizationV2()
        {
            presentation = new ChaChaPresentation(this);
        }

        #region ChaCha Override

        public override void Execute()
        {
            ClearIntermediateResults();
            base.Execute();
        }

        protected override void Xcrypt(byte[] key, byte[] iv, ulong initialCounter, ChaChaSettings settings, ICryptoolStream input, CStreamWriter output)
        {
            // calculate total keystream blocks needed
            CStreamReader inputReader = input.CreateReader();
            // byte size of input
            long inputSize = inputReader.Length;
            // one keystream block is 64 bytes (512 bit)
            TotalKeystreamBlocks = (int)Math.Ceiling((double)(inputSize) / 64);
            inputReader.Dispose();
            base.Xcrypt(key, iv, initialCounter, settings, input, output);
        }

        protected override void ChaChaHash(ref uint[] state, int rounds)
        {
            uint[] originalState = (uint[])(state.Clone());
            OriginalState.Add(originalState);
            base.ChaChaHash(ref state, rounds);
        }

        protected override void AdditionStep(ref uint[] state, uint[] originalState)
        {
            base.AdditionStep(ref state, originalState);
            uint[] additionResultState = (uint[])(state.Clone());
            AdditionResultState.Add(additionResultState);
        }

        protected override void LittleEndianStep(ref uint[] state)
        {
            base.LittleEndianStep(ref state);
            uint[] littleEndianState = (uint[])(state.Clone());
            LittleEndianState.Add(littleEndianState);
        }

        protected override (uint, uint, uint, uint) Quarterround(uint a, uint b, uint c, uint d)
        {
            QRInput.Add((a, b, c, d));
            (uint aOut, uint bOut, uint cOut, uint dOut) = base.Quarterround(a, b, c, d);
            QROutput.Add((aOut, bOut, cOut, dOut));
            return (aOut, bOut, cOut, dOut);
        }

        /// <summary>
        /// Visualization overriden quarterround step function.
        ///
        /// Does not call base function because it must calculate intermediate values anyway and thus
        /// can return them without calling base function.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="x3"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        protected override (uint, uint, uint) QuarterroundStep(uint x1, uint x2, uint x3, int shift)
        {
            x1 += x2;
            uint add = x1;
            x3 ^= x1;
            uint xor = x3;
            x3 = ByteUtil.RotateLeft(x3, shift);
            uint shift_ = x3;
            QRStep.Add(new QRStep(add, xor, shift_));
            return (x1, x2, x3);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            IEnumerable<ValidationResult> results = base.Validate();
            if (InputStream.Length == 0)
            {
                GuiLogMessage("Input message must not be empty.", NotificationLevel.Error);
                IsValid = false;
            }
            return results;
        }

        #endregion ChaCha Override

        #region Intermediate values from cipher execution

        private void ClearIntermediateResults()
        {
            OriginalState.Clear();
            QRInput.Clear();
            QRStep.Clear();
            QROutput.Clear();
        }

        private List<uint[]> _state; public List<uint[]> OriginalState
        {
            get
            {
                if (_state == null) _state = new List<uint[]>();
                return _state;
            }
        }

        private List<uint[]> _additionResultState; public List<uint[]> AdditionResultState
        {
            get
            {
                if (_additionResultState == null) _additionResultState = new List<uint[]>();
                return _additionResultState;
            }
        }

        private List<uint[]> _littleEndianState; public List<uint[]> LittleEndianState
        {
            get
            {
                if (_littleEndianState == null) _littleEndianState = new List<uint[]>();
                return _littleEndianState;
            }
        }

        private List<(uint, uint, uint, uint)> _qrInput; public List<(uint, uint, uint, uint)> QRInput
        {
            get
            {
                if (_qrInput == null) _qrInput = new List<(uint, uint, uint, uint)>();
                return _qrInput;
            }
        }

        private List<QRStep> _qrStep; public List<QRStep> QRStep
        {
            get
            {
                if (_qrStep == null) _qrStep = new List<QRStep>();
                return _qrStep;
            }
        }

        private List<(uint, uint, uint, uint)> _qrOutput; public List<(uint, uint, uint, uint)> QROutput
        {
            get
            {
                if (_qrOutput == null) _qrOutput = new List<(uint, uint, uint, uint)>();
                return _qrOutput;
            }
        }

        #endregion Intermediate values from cipher execution

        #region Public Variables / Binding Properties

        private bool _isValid; public override bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _executionFinished; public override bool ExecutionFinished
        {
            get
            {
                return _executionFinished;
            }
            set
            {
                if (_executionFinished != value)
                {
                    _executionFinished = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalKeystreamBlocks; public int TotalKeystreamBlocks
        {
            get
            {
                return _totalKeystreamBlocks;
            }
            set
            {
                if (_totalKeystreamBlocks != value)
                {
                    _totalKeystreamBlocks = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Public Variables / Binding Properties

        #region IPlugin Members

        /// <summary>
        /// Provide plugin-related parameters (per instance) or return null.
        /// </summary>

        /// <summary>
        /// Provide custom presentation to visualize the execution or return null.
        /// </summary>
        public override System.Windows.Controls.UserControl Presentation
        {
            get { return presentation; }
        }

        #endregion IPlugin Members
    }
}