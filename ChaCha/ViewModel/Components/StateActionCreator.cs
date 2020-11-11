using System;
using System.Diagnostics;

namespace Cryptool.Plugins.ChaCha.ViewModel.Components
{
    /// <summary>
    /// Class which implements the actions for state addition and little-endian step in ChaCha Hash.
    /// </summary>
    internal class StateActionCreator : ChaChaHashActionCreator
    {
        public StateActionCreator(ChaChaHashViewModel viewModel) : base(viewModel)
        {
        }

        public Action ClearStateMatrix
        {
            get
            {
                return () =>
                {
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.StateValues[i].Value = null;
                        VM.StateValues[i].Mark = false;
                    }
                };
            }
        }

        public Action InsertFirstOriginalState
        {
            get
            {
                return () =>
                {
                    InsertOriginalState(0)();
                };
            }
        }

        /// <summary>
        /// Reset the state matrix to the state at the start of the keystream block.
        /// </summary>
        /// <param name="keystreamBlock">Zero-based keystream block index.</param>
        public Action InsertOriginalState(int keystreamBlock)
        {
            return () =>
            {
                Debug.Assert(VM.ChaCha.OriginalState.Count == VM.ChaCha.TotalKeystreamBlocks,
                $"Count of OriginalState was not equal to TotalKeystreamBlocks. Expected: {VM.ChaCha.TotalKeystreamBlocks}. Actual: {VM.ChaCha.OriginalState.Count}");
                uint[] state = VM.ChaCha.OriginalState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.StateValues[i].Value = state[i];
                    VM.StateValues[i].Mark = false;
                }
                if (VM.DiffusionActive)
                {
                    uint[] diffusionState = VM.ChaCha.OriginalStateDiffusion[keystreamBlock];
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.DiffusionStateValues[i].Value = diffusionState[i];
                    }
                    VM.OnPropertyChanged("DiffusionStateValues");
                    VM.OnPropertyChanged("DiffusionFlippedBits");
                    VM.OnPropertyChanged("DiffusionFlippedBitsPercentage");
                }
            };
        }

        public Action HideOriginalState
        {
            get
            {
                return () =>
                {
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.OriginalState[i].Value = null;
                    }
                    if (VM.DiffusionActive)
                    {
                        for (int i = 0; i < 16; ++i)
                        {
                            VM.DiffusionOriginalState[i].Value = null;
                        }
                        VM.OnPropertyChanged("DiffusionOriginalState");
                    }
                };
            }
        }

        public Action HideAdditionResult
        {
            get
            {
                return () =>
                {
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.AdditionResultState[i].Value = null;
                    }
                    if (VM.DiffusionActive)
                    {
                        for (int i = 0; i < 16; ++i)
                        {
                            VM.DiffusionAdditionResultState[i].Value = null;
                        }
                        VM.OnPropertyChanged("DiffusionAdditionResultState");
                    }
                };
            }
        }

        public Action HideLittleEndian
        {
            get
            {
                return () =>
                {
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.LittleEndianState[i].Value = null;
                    }
                    if (VM.DiffusionActive)
                    {
                        for (int i = 0; i < 16; ++i)
                        {
                            VM.DiffusionLittleEndianState[i].Value = null;
                        }
                        VM.OnPropertyChanged("DiffusionLittleEndianState");
                    }
                };
            }
        }

        public Action ShowOriginalState(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] originalState = VM.ChaCha.OriginalState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.OriginalState[i].Value = originalState[i];
                }
                if (VM.DiffusionActive)
                {
                    uint[] diffusionState = VM.ChaCha.OriginalStateDiffusion[keystreamBlock];
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.DiffusionOriginalState[i].Value = diffusionState[i];
                    }
                    VM.OnPropertyChanged("DiffusionOriginalState");
                }
            };
        }

        public Action ShowAdditionResult(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] additionResult = VM.ChaCha.AdditionResultState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.AdditionResultState[i].Value = additionResult[i];
                }
                if (VM.DiffusionActive)
                {
                    uint[] diffusionState = VM.ChaCha.AdditionResultStateDiffusion[keystreamBlock];
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.DiffusionAdditionResultState[i].Value = diffusionState[i];
                    }
                    VM.OnPropertyChanged("DiffusionAdditionResultState");
                }
            };
        }

        public Action ShowLittleEndian(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] le = VM.ChaCha.LittleEndianState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.LittleEndianState[i].Value = le[i];
                }
                if (VM.DiffusionActive)
                {
                    uint[] diffusionState = VM.ChaCha.LittleEndianStateDiffusion[keystreamBlock];
                    for (int i = 0; i < 16; ++i)
                    {
                        VM.DiffusionLittleEndianState[i].Value = diffusionState[i];
                    }
                    VM.OnPropertyChanged("DiffusionLittleEndianState");
                }
            };
        }
    }
}