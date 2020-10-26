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
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualizationV2
{
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    [PluginInfo("ChaChaVisualizationV2", "Subtract one number from another", "ChaChaVisualizationV2/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    [ComponentCategory(ComponentCategory.CiphersModernSymmetric)]
    public class ChaChaVisualizationV2 : ChaCha.ChaCha
    {
        #region Private Variables

        private readonly ChaChaPresentation presentation = new ChaChaPresentation();

        #endregion Private Variables

        #region IPlugin Members

        /// <summary>
        /// Provide plugin-related parameters (per instance) or return null.
        /// </summary>

        /// <summary>
        /// Provide custom presentation to visualize the execution or return null.
        /// </summary>
        public override UserControl Presentation
        {
            get { return presentation; }
        }

        #endregion IPlugin Members
    }
}