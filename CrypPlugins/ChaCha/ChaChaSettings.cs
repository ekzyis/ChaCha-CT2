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
using System;
using System.ComponentModel;
using Cryptool.PluginBase;
using Cryptool.PluginBase.Miscellaneous;

namespace Cryptool.Plugins.ChaCha
{
    // HOWTO: rename class (click name, press F2)
    public class ChaChaSettings : ISettings
    {
        private int rounds = 20;
        private int _version = 0;

        [TaskPane("RoundCaption", "RoundTooltip", null, 0, false, ControlType.ComboBox, new string[] { "8", "12", "20" })]
        public int Rounds
        {
            get { return rounds; }
            set
            {
                switch(value)
                {
                    case 0:
                    case 8:
                        rounds = 8;
                        break;
                    case 1:
                    case 12:
                        rounds = 12;
                        break;
                    case 2:
                    case 20:
                        rounds = 20;
                        break;
                }
                OnPropertyChanged("Rounds");
            }
        }

        [TaskPane("VersionCaption", "VersionTooltip", null, 0, false, ControlType.ComboBox, new string[] { "IETF", "DJB" })]
        public int Version_
        {
            get { return _version; }
            set
            {
                _version = value;
                switch(value)
                {
                    case 0:
                        Version = ChaCha.Version.IETF;
                        break;
                    case 1:
                        Version = ChaCha.Version.DJB;
                        break;

                }
                OnPropertyChanged("Version");
            }
        }

        public ChaCha.Version Version { get; private set; } = ChaCha.Version.IETF;

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            EventsHelper.PropertyChanged(PropertyChanged, this, propertyName);
        }

        #endregion

        public void Initialize()
        {

        }
    }
}
