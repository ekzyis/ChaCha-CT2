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
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using Cryptool.PluginBase;
using Cryptool.PluginBase.Miscellaneous;

namespace Cryptool.Plugins.ChaCha
{
    // HOWTO: Change author name, email address, organization and URL.
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    // HOWTO: Change plugin caption (title to appear in CT2) and tooltip.
    // You can (and should) provide a user documentation as XML file and an own icon.
    [PluginInfo("ChaCha", "Stream cipher based on Salsa20", "ChaCha/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    // HOWTO: Change category to one that fits to your plugin. Multiple categories are allowed.
    [ComponentCategory(ComponentCategory.ToolsMisc)]
    public class ChaCha : ICrypComponent
    {
        #region Private Variables

        private readonly ChaChaSettings settings;

        private byte[] inputData;
        private byte[] outputData;
        private byte[] inputKey;
        private byte[] inputIV;

        private int rounds;

        // ChaCha state consists of 16 32-bit integers
        private uint[] state = new uint[16]; 
        // a keystream block consists of 64 bytes (one mutated state)
        private byte[] keyStreamBlock = new byte[64]; 

        #endregion

        #region Public Variables

        public static byte[] sigma = Encoding.ASCII.GetBytes("expand 32-byte k");
        public static byte[] tau = Encoding.ASCII.GetBytes("expand 16-byte k");
        
        public ChaCha()
        {
            this.settings = new ChaChaSettings();
        }

        #endregion

        #region Data Properties

        /// <summary>
        /// HOWTO: Input interface to read the input data. 
        /// You can add more input properties of other type if needed.
        /// </summary>
        [PropertyInfo(Direction.InputData, "InputDataCaption", "InputDataTooltip", true)]
        public byte[] InputData
        {
            get { return this.inputData; }
            set { 
                this.inputData = value;
                OnPropertyChanged("InputData");
            }
        }

        [PropertyInfo(Direction.InputData, "InputKeyCaption", "InputKeyTooltip", true)]
        public byte[] InputKey
        {
            get { return this.inputKey; }
            set
            {
                this.inputKey = value;
                OnPropertyChanged("InputKey");
            }
        }

        [PropertyInfo(Direction.InputData, "InputIVCaption", "InputIVTooltip", true)]
        public byte[] InputIV
        {
            get { return this.inputIV; }
            set
            {
                this.inputIV = value;
                OnPropertyChanged("InputIV");
            }
        }

        [PropertyInfo(Direction.OutputData, "OutputDataCaption", "OutputDataTooltip", true)]
        public byte[] OutputData
        {
            get { return this.outputData; }
            set
            {
                this.outputData = value;
                OnPropertyChanged("OutputData");
            }
        }

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Provide plugin-related parameters (per instance) or return null.
        /// </summary>
        public ISettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Provide custom presentation to visualize the execution or return null.
        /// </summary>
        public UserControl Presentation
        {
            get { return null; }
        }

        /// <summary>
        /// Called once when workflow execution starts.
        /// </summary>
        public void PreExecution()
        {
        }

        /// <summary>
        /// Called every time this plugin is run in the workflow execution.
        /// </summary>
        public void Execute()
        {
            ProgressChanged(0, 1);

            initStateMatrix();

            ProgressChanged(1, 1);
        }
        /* 
         * Initialize the state of ChaCha which can be represented as a matrix.
         * 
         * The state matrix consists of 16 4-byte entries which makes the state 64 byte large in total.
         * The state is constructed as following:
         *  
         *      CONSTANT  CONSTANT  CONSTANT  CONSTANT
         *      KEY       KEY       KEY       KEY
         *      KEY       KEY       KEY       KEY
         *      INPUT     INPUT     INPUT     INPUT
         *      
         * The input is not the text but the IV and counter which comes first.
         * Everything is in little-endian except the counter.
         */
        public void initStateMatrix()
        {
            byte[] constants;
            if (inputKey.Length == 32) // 32 byte key
            {
                constants = sigma;
            }
            else if(inputKey.Length == 16) // 16 byte key
            {
                constants = tau;
            }
            else
            {
                throw new ArgumentException("Key must be 32 or 16-byte.");
            }

            int stateOffset = 0;
            // Convenience method to not abstract away state offset.
            void addToState(uint value)
            {
                state[stateOffset] = value;
                stateOffset++;
            }
            void add4ByteChunksToStateAsLittleEndian(byte[] toAdd)
            {
                for (int i = 0; 4 * i < toAdd.Length; ++i)
                {
                    addToState(To4ByteLE(toAdd, 0 + 4 * i));
                }
            }

            add4ByteChunksToStateAsLittleEndian(constants);
            add4ByteChunksToStateAsLittleEndian(inputKey);
            if(inputKey.Length == 16) add4ByteChunksToStateAsLittleEndian(inputKey);
            uint startCounter = 0;
            addToState(startCounter);
            add4ByteChunksToStateAsLittleEndian(InputIV);
        }

        /* Return an uint32 in little-endian from the given byte-array, starting at offset.*/
        public uint To4ByteLE(byte[] x, int offset)
        {
            byte b1 = x[offset];
            byte b2 = x[offset + 1];
            byte b3 = x[offset + 2];
            byte b4 = x[offset + 3];

            return (uint)(b4 << 24 | b3 << 16 | b2 << 8 | b1);
        }

        /// <summary>
        /// Called once after workflow execution has stopped.
        /// </summary>
        public void PostExecution()
        {
        }

        /// <summary>
        /// Triggered time when user clicks stop button.
        /// Shall abort long-running execution.
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// Called once when plugin is loaded into editor workspace.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Called once when plugin is removed from editor workspace.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Event Handling

        public event StatusChangedEventHandler OnPluginStatusChanged;

        public event GuiLogNotificationEventHandler OnGuiLogNotificationOccured;

        public event PluginProgressChangedEventHandler OnPluginProgressChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void GuiLogMessage(string message, NotificationLevel logLevel)
        {
            EventsHelper.GuiLogMessage(OnGuiLogNotificationOccured, this, new GuiLogEventArgs(message, this, logLevel));
        }

        private void OnPropertyChanged(string name)
        {
            EventsHelper.PropertyChanged(PropertyChanged, this, new PropertyChangedEventArgs(name));
        }

        private void ProgressChanged(double value, double max)
        {
            EventsHelper.ProgressChanged(OnPluginProgressChanged, this, new PluginProgressEventArgs(value, max));
        }

        #endregion
    }
}
