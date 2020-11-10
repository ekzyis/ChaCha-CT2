﻿using System;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components
{
    /// <summary>
    /// Class which implements the actions for state addition and little-endian step in ChaCha Hash.
    /// </summary>
    internal class StateActionCreator : ChaChaHashActionCreator
    {
        public StateActionCreator(ChaChaHashViewModel viewModel) : base(viewModel)
        {
        }

        public Action ShowOriginalState(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] originalState = VM.ChaChaVisualization.OriginalState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.OriginalState[i].Value = originalState[i];
                }
            };
        }

        public Action ShowAdditionResultState(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] additionResult = VM.ChaChaVisualization.AdditionResultState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.AdditionResultState[i].Value = additionResult[i];
                }
            };
        }

        public Action ShowLittleEndianState(int keystreamBlock)
        {
            return () =>
            {
                AssertKeystreamBlockInput(keystreamBlock);
                uint[] le = VM.ChaChaVisualization.LittleEndianState[keystreamBlock];
                for (int i = 0; i < 16; ++i)
                {
                    VM.LittleEndianState[i].Value = le[i];
                }
            };
        }
    }
}