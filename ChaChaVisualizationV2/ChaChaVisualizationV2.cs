﻿/*
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
using Cryptool.Plugins.ChaChaVisualizationV2.View;
using System.Collections.Generic;
using System.ComponentModel;

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

        protected override uint[] State(byte[] key, byte[] iv, ulong counter, ChaCha.Version version)
        {
            uint[] state = base.State(key, iv, counter, version);
            OriginalState.Add(state);
            return state;
        }

        #endregion ChaCha Override

        #region Intermediate values from cipher execution

        private List<uint[]> _state; public List<uint[]> OriginalState
        {
            get
            {
                if (_state == null) _state = new List<uint[]>();
                return _state;
            }
        }

        #endregion Intermediate values from cipher execution

        #region Public Variables

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

        #endregion Public Variables

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