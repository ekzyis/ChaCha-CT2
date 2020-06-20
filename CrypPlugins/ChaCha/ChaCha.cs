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
using System.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using Cryptool.PluginBase;
using Cryptool.PluginBase.Miscellaneous;
using System.Windows.Documents;
using System.Diagnostics;
using System.Security.Policy;
using System.IO.Ports;

namespace Cryptool.Plugins.ChaCha
{
    // HOWTO: Change author name, email address, organization and URL.
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    // HOWTO: Change plugin caption (title to appear in CT2) and tooltip.
    // You can (and should) provide a user documentation as XML file and an own icon.
    [PluginInfo("ChaCha", "Stream cipher based on Salsa20", "ChaCha/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    // HOWTO: Change category to one that fits to your plugin. Multiple categories are allowed.
    [ComponentCategory(ComponentCategory.CiphersModernSymmetric)]
    public class ChaCha : ICrypComponent
    {
        #region Private Variables

        private readonly ChaChaSettings settings;

        private byte[] inputData;
        private byte[] outputData;
        private byte[] inputKey;
        private byte[] inputIV;

        private int rounds;
        // one block has 512 bits
        private readonly static int BLOCKSIZE_BYTES = 64;
        // bits of counter
        private static int COUNTERSIZE = 32;
        // bits of IV
        private static int IVSIZE = 96;
        // ChaCha state consists of 16 32-bit integers
        private uint[] initial_state = new uint[16]; 

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

            GuiLogMessage("Executing ChaCha", NotificationLevel.Info);

            if (validateInput())
            {
                InitStateMatrix();

                OutputData = Xcrypt(InputData);
            }

            ProgressChanged(1, 1);
        }

        /*
         * Validates the given inputs.
         */ 
        public bool validateInput()
        {
            string message = null;
            if (inputKey.Length != 32 && inputKey.Length != 16)
            {
                message = "Key must be 32 or 16-byte.";
            }
            else if (inputIV.Length != IVSIZE / 8)
            {
                message = "IV must be 12-byte";
            }
            if(message != null)
            {
                GuiLogMessage(message, NotificationLevel.Error);
                return false;
            }
            return true;
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
        public void InitStateMatrix()
        {
            byte[] constants;
            if (inputKey.Length == 32) // 32 byte key
            {
                constants = sigma;
            }
            else // 16-byte key
            {
                constants = tau;
            }

            int stateOffset = 0;
            // Convenience method to abstract away state offset.
            void addToState(uint value)
            {
                initial_state[stateOffset] = value;
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
            byte[] counter = Enumerable.Repeat<byte>(0, COUNTERSIZE / 8).ToArray();
            add4ByteChunksToStateAsLittleEndian(counter);
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
        public uint To4ByteLE(uint x)
        {
            return To4ByteLE(BitConverter.GetBytes(x), 0);
        }

        /* XOR the input with the keystream which results in en- or decryption.*/
        public byte[] Xcrypt(byte[] src)
        {
            byte[] dst = new byte[src.Length];
            int keystreamBlocksNeeded = (int) Math.Ceiling((double)(src.Length) / BLOCKSIZE_BYTES);
            byte[,] keystreamBlocks = new byte[keystreamBlocksNeeded, BLOCKSIZE_BYTES];
            int keystreamBlocksOffset = 0;
            // Convenience method to abstract away keystream offset.
            void addToKeystream(byte[] block)
            {
                for (int i = 0; i < block.Length; ++i)
                    keystreamBlocks[keystreamBlocksOffset, i] = block[i];
                keystreamBlocksOffset++;
            }
            for (uint i = 0; i < keystreamBlocksNeeded; i++)
            {
                addToKeystream(generateKeyStreamBlock(i));
            }
            // flatten two dimension array of bytes into one dimensional array of bytes
            byte[] keystream = new byte[keystreamBlocksNeeded * BLOCKSIZE_BYTES];
            int keystreamOffset = 0;
            foreach (byte b in keystreamBlocks)
            {
                keystream[keystreamOffset] = b;
                keystreamOffset++;
            }
            // XOR the input with the keystream
            for (int i = 0; i < src.Length; ++i)
            {
                dst[i] = (byte)(src[i] ^ keystream[i]);
            }
            return dst;
        }

        /* Generate the n-th keystream block.
         * 
         * Uses the initial state matrix to insert the counter n and then calculate the keystream block.
         */
        public byte[] generateKeyStreamBlock(uint n)
        {
            uint[] state = (uint[])(initial_state.Clone());
            byte[] counter = BitConverter.GetBytes(n);
            // set counter value to state
            state[8] = To4ByteLE(counter, 0);

            // hash state block
            uint[] hash = chachaHash(state);
            // convert the hashed uint state array into an array of bytes
            byte[] keystreamBlock = new byte[BLOCKSIZE_BYTES];
            for (int i = 0; i < hash.Length; ++i)
            {
                byte[] stateEntryBytes = BitConverter.GetBytes(hash[i]);
                keystreamBlock[i] = stateEntryBytes[0];
                keystreamBlock[i+1] = stateEntryBytes[1];
                keystreamBlock[i+2] = stateEntryBytes[2];
                keystreamBlock[i+3] = stateEntryBytes[3];
            }
            return keystreamBlock;
        }

        /* Generates the hash out of the input state block.*/
        public uint[] chachaHash(uint[] state)
        {
            uint[] originalState = (uint[])(state.Clone());
            for (int i = 0; i < rounds / 2; ++i)
            {
                // column round
                state = quarterroundState(state, 0, 4, 8, 12);
                state = quarterroundState(state, 1, 5, 9, 13);
                state = quarterroundState(state, 2, 6, 10, 14);
                state = quarterroundState(state, 3, 7, 11, 15);
                // diagonal round
                state = quarterroundState(state, 0, 5, 10, 15);
                state = quarterroundState(state, 1, 6, 11, 12);
                state = quarterroundState(state, 2, 7, 8, 13);
                state = quarterroundState(state, 3, 4, 0, 14);
            }
            // add the original state and write each entry as little-endian
            for (int i = 0; i < state.Length; ++i)
            {
                state[i] += originalState[i];
                state[i] = To4ByteLE(state[i]);
            }
            return state;
        }

        /**
         * Calculate the quarterround value of the state using the given indices.
         */ 
        public  uint[] quarterroundState(uint[] state, int i, int j, int k, int l)
        {
            (state[i], state[j], state[k], state[l]) = quarterround(state[i], state[j], state[k], state[l]);
            return state;
        }

        /**
         * Calculate the quarterround value of the inputs.
         */
        public (uint, uint, uint, uint) quarterround(uint a, uint b, uint c, uint d)
        {
            (uint, uint, uint) quarterroundStep(uint x1, uint x2, uint x3, int shift)
            {
                x1 += x2;
                x3 ^= x1;
                x3 = rotateLeft(x3, shift);
                return (x1, x2, x3);
            }
            (a, b, d) = quarterroundStep(a, b, d, 16);
            (c, d, b) = quarterroundStep(c, d, b, 12);
            (a, b, d) = quarterroundStep(a, b, d, 8);
            (c, d, b) = quarterroundStep(c, d, b, 7);
            return (a, b, c, d);
        }

        /**
         * Circular shift to the left.
         */ 
        public uint rotateLeft(uint x, int shift)
        {
            return (x << shift) | (x >> shift);
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
