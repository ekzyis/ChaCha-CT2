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
using System.Windows.Threading;
using System.Threading;

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
        private readonly ChaChaPresentation _presentation = new ChaChaPresentation();

        private byte[] _inputData;
        private byte[] _outputData;
        private byte[] _inputKey;
        private byte[] _inputIV;

        // one block has 512 bits
        private readonly static int BLOCKSIZE_BYTES = 64;
        // counter size (depends on version)
        private int COUNTERSIZE_BITS;
        // IV size (depends on version)
        private int IVSIZE_BITS;
        // initial counter value (for keystream blocks)
        private uint INITIAL_COUNTER;
        // ChaCha state consists of 16 32-bit integers
        private readonly uint[] initial_state = new uint[16];

        public sealed class Version
        {
            public static readonly Version IETF = new Version("IETF", 32, 96, 1);
            public static readonly Version DJB = new Version("DJB", 64, 64, 0);
            public string Name { get; private set; }
            public int BitsCounter { get; private set; }
            public int BitsIV { get; private set; }
            public uint InitialCounter { get; private set; }
            private Version(string name, int bitsCounter, int bitsIV, uint initialCounter)
            {
                Name = name;
                BitsCounter = bitsCounter;
                BitsIV = bitsIV;
                InitialCounter = initialCounter;
            }
        }

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
            get { return this._inputData; }
            set { 
                this._inputData = value;
                OnPropertyChanged("InputData");
            }
        }

        [PropertyInfo(Direction.InputData, "InputKeyCaption", "InputKeyTooltip", true)]
        public byte[] InputKey
        {
            get { return this._inputKey; }
            set
            {
                this._inputKey = value;
                OnPropertyChanged("InputKey");
            }
        }

        [PropertyInfo(Direction.InputData, "InputIVCaption", "InputIVTooltip", true)]
        public byte[] InputIV
        {
            get { return this._inputIV; }
            set
            {
                this._inputIV = value;
                OnPropertyChanged("InputIV");
            }
        }

        [PropertyInfo(Direction.OutputData, "OutputDataCaption", "OutputDataTooltip", true)]
        public byte[] OutputData
        {
            get { return this._outputData; }
            set
            {
                this._outputData = value;
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
            get { return _presentation; }
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

            // clear all previous results (important if user started workflow, stopped and then restarted)
            DispatchToPresentation(delegate
            {
                _presentation.clearResults();
            });

            GuiLogMessage("Executing ChaCha", NotificationLevel.Info);

            GuiLogMessage(string.Format("Rounds: {0}", settings.Rounds), NotificationLevel.Info);
            GuiLogMessage(string.Format("Version: {0}", settings.Version.Name), NotificationLevel.Info);

            COUNTERSIZE_BITS = settings.Version.BitsCounter;
            IVSIZE_BITS = settings.Version.BitsIV;
            INITIAL_COUNTER = settings.Version.InitialCounter;

            Console.WriteLine(String.Format("Rounds: {0}, Counter size: {1}", settings.Rounds, COUNTERSIZE_BITS / 8));
            Console.WriteLine(String.Format("Expected: Key size: 16 or 32, IV size: {0}", IVSIZE_BITS / 8));
            Console.WriteLine(String.Format("Actual: Key size: {0}, IV size: {1}", InputKey.Length, InputIV.Length));

            if (ValidateInput())
            {
                InitStateMatrix();

                OutputData = Xcrypt(InputData);
            }

            ProgressChanged(1, 1);

            // enable navigation since now all values needed for visualization are set.
            DispatchToPresentation(delegate { _presentation.ExecutionFinished = true; });
        }

        private void DispatchToPresentation(SendOrPostCallback callback)
        {
            Presentation.Dispatcher.Invoke(DispatcherPriority.Normal, callback, null);
        }

        /*
         * Validates the given inputs.
         */ 
        public bool ValidateInput()
        {
            string message = null;
            if (_inputKey.Length != 32 && _inputKey.Length != 16)
            {
                message = "Key must be 32 or 16-byte.";
            }
            else if (_inputIV.Length != IVSIZE_BITS / 8)
            {
                message = string.Format("IV must be {0}-byte", IVSIZE_BITS / 8);
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
         * Every entry is in little-endian.
         */
        public void InitStateMatrix()
        {
            byte[] constants;
            if (_inputKey.Length == 32) // 32 byte key
            {
                constants = sigma;
            }
            else // 16-byte key
            {
                constants = tau;
            }

            int stateOffset = 0;
            for (int i = 0; 4 * i < constants.Length; ++i)
            {
                uint le = To4ByteLE(constants, 0 + 4 * i);
                initial_state[stateOffset] = le;
                stateOffset++;
            }
            for (int i = 0; 4 * i < _inputKey.Length; ++i)
            {
                uint le = To4ByteLE(_inputKey, 0 + 4 * i);
                initial_state[stateOffset] = le;
                stateOffset++;
            }
            if (_inputKey.Length == 16)
            {
                for (int i = 0; 4 * i < _inputKey.Length; ++i)
                {
                    uint le = To4ByteLE(_inputKey, 0 + 4 * i);
                    initial_state[stateOffset] = le;
                    stateOffset++;
                }
            }
            byte[] counter = Enumerable.Repeat<byte>(0, COUNTERSIZE_BITS / 8).ToArray();
            for (int i = 0; 4 * i < counter.Length; ++i)
            {
                initial_state[stateOffset] = To4ByteLE(counter, 0 + 4 * i);
                stateOffset++;
            }
            for (int i = 0; 4 * i < InputIV.Length; ++i)
            {
                initial_state[stateOffset] = To4ByteLE(InputIV, 0 + 4 * i);
                stateOffset++;
            }

            DispatchToPresentation(delegate
            {
                // set state params
                _presentation.Constants = constants;
                _presentation.InputKey = _inputKey;
                _presentation.InputIV = _inputIV;
                _presentation.InputData = _inputData;
                _presentation.InitialCounter = counter;
            });
        }

        /* Return an uint32 in little-endian from the given byte-array, starting at offset.*/
        public static uint To4ByteLE(byte[] x, int offset)
        {
            byte b1 = x[offset];
            byte b2 = x[offset + 1];
            byte b3 = x[offset + 2];
            byte b4 = x[offset + 3];

            return (uint)(b4 << 24 | b3 << 16 | b2 << 8 | b1);
        }
        public static uint To4ByteLE(uint x)
        {
            uint b1 = x >> 24;
            uint b2 = (x >> 16) & 0xFF;
            uint b3 = (x >> 8) & 0xFF;
            uint b4 = x & 0xFF;
            return b4 << 24 | b3 << 16 | b2 << 8 | b1;
        }

        /* XOR the input with the keystream which results in en- or decryption.*/
        public byte[] Xcrypt(byte[] src)
        {
            byte[] dst = new byte[src.Length];
            int keystreamBlocksNeeded = (int) Math.Ceiling((double)(src.Length) / BLOCKSIZE_BYTES);
            byte[] keystream = new byte[keystreamBlocksNeeded * BLOCKSIZE_BYTES];
            int keystreamBlocksOffset = 0;

            for (uint i = INITIAL_COUNTER; i < keystreamBlocksNeeded + INITIAL_COUNTER; i++)
            {
                byte[] keyblock = GenerateKeystreamBlock(i);
                // add each byte of keyblock to keystream
                for (int j = 0; j < keyblock.Length; ++j)
                {
                    keystream[keystreamBlocksOffset] = keyblock[j];
                    keystreamBlocksOffset++;
                }
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
        public byte[] GenerateKeystreamBlock(ulong n)
        {
            uint[] state = (uint[])(initial_state.Clone());
            setCounterToState(state, n);
            // hash state block
            uint[] hash = ChaChaHash(state);
            // convert the hashed uint state array into an array of bytes
            byte[] keystreamBlock = new byte[BLOCKSIZE_BYTES];
            for (int i = 0; i < hash.Length; ++i)
            {
                byte[] stateEntryBytes = GetBytes(hash[i]);
                keystreamBlock[4*i] = stateEntryBytes[0];
                keystreamBlock[4*i+1] = stateEntryBytes[1];
                keystreamBlock[4*i+2] = stateEntryBytes[2];
                keystreamBlock[4*i+3] = stateEntryBytes[3];
            }
            return keystreamBlock;
        }

        public void setCounterToState(uint[] state, ulong c)
        {
            byte[] counter = GetBytes(c);
            // counter as little-endian
            Array.Reverse(counter);
            // set counter value to state
            state[12] = To4ByteLE(counter, 0);
            if (COUNTERSIZE_BITS > 32)
                state[13] = To4ByteLE(counter, 4);
        }

        /**
         * Return a bytes array of the given number with the highest significant byte always coming first (big-endian), independent of system architecture.
         * For example: getBytes(0x01020304) -> byte[4] { 1, 2, 3, 4 }
         */
        public static byte[] GetBytes(uint n)
        {
            byte[] b = BitConverter.GetBytes(n);
            // if system architecture is little-endian,
            // BitConverter.GetBytes returned bytes in little-endian order thus we reverse the order
            // to always get the highest significant byte first (big-endian)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }
            return b;
        }

        public static byte[] GetBytes(ulong n)
        {
            byte[] b = BitConverter.GetBytes(n);
            // if system architecture is little-endian,
            // BitConverter.GetBytes returned bytes in little-endian order thus we reverse the order
            // to always get the highest significant byte first (big-endian)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }
            return b;
        }

        /**
         * Return a uint of the given bytes array interpreted as big-endian, independent of system architecture.
         * For example: ToUInt32(new byte[] { 0x01, 0x02, 0x03, 0x04 }) -> 0x01020304
         */
        public static uint ToUInt32(byte[] b, int startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }
            return BitConverter.ToUInt32(b, startIndex);
        }

        /* Generates the hash out of the input state block.*/
        public uint[] ChaChaHash(uint[] state)
        {
            uint[] originalState = (uint[])(state.Clone());
            for (int i = 0; i < settings.Rounds / 2; ++i)
            {
                // column round
                state = QuarterroundState(state, 0, 4, 8, 12);
                state = QuarterroundState(state, 1, 5, 9, 13);
                state = QuarterroundState(state, 2, 6, 10, 14);
                state = QuarterroundState(state, 3, 7, 11, 15);
                // diagonal round
                state = QuarterroundState(state, 0, 5, 10, 15);
                state = QuarterroundState(state, 1, 6, 11, 12);
                state = QuarterroundState(state, 2, 7, 8, 13);
                state = QuarterroundState(state, 3, 4, 9, 14);
            }
            // add the original state
            for (int i = 0; i < state.Length; ++i)
            {
                state[i] += originalState[i];
            }
            // each 4 byte chunk as little-endian
            for (int i = 0; i < state.Length; ++i)
            {
                byte[] b = GetBytes(state[i]);
                Array.Reverse(b);
                state[i] = ToUInt32(b, 0);
            }
            return state;
        }

        /**
         * Calculate the quarterround value of the state using the given indices.
         */ 
        public  uint[] QuarterroundState(uint[] state, int i, int j, int k, int l)
        {
            uint[] qr = Quarterround(state[i], state[j], state[k], state[l]);
            state[i] = qr[0];
            state[j] = qr[1];
            state[k] = qr[2];
            state[l] = qr[3];
            return state;
        }

        public uint[] quarterroundStep(uint x1, uint x2, uint x3, int shift)
        {
            x1 += x2; // x1 = x1 + x2
            DispatchToPresentation(delegate
            {
                _presentation.AddResult(ChaChaPresentation.ResultType.ADD_X1_X2, x1);
            });
            DispatchToPresentation(delegate
            {
                _presentation.AddResult(ChaChaPresentation.ResultType.X2, x2);
            });
            x3 ^= x1; // x3 = x3 ^ x1 = x3 ^ ( x1 + x2 )
            DispatchToPresentation(delegate
            {
                _presentation.AddResult(ChaChaPresentation.ResultType.QR_XOR, x3);
            });
            x3 = RotateLeft(x3, shift); // x3 <<< shift = ( x3 ^ x1 ) <<< shift = (x3 ^ (x1 + x2)) <<< shift
            return new uint[] { x1, x2, x3 };
        }

        /**
         * Calculate the quarterround value of the inputs.
         */
        public uint[] Quarterround(uint a, uint b, uint c, uint d)
        {
            uint[] qrstep = quarterroundStep(a, b, d, 16);
            a = qrstep[0]; b = qrstep[1]; d = qrstep[2];
            qrstep = quarterroundStep(c, d, b, 12);
            c = qrstep[0]; d = qrstep[1]; b = qrstep[2];
            qrstep = quarterroundStep(a, b, d, 8);
            a = qrstep[0]; b = qrstep[1]; d = qrstep[2];
            qrstep = quarterroundStep(c, d, b, 7);
            c = qrstep[0]; d = qrstep[1]; b = qrstep[2];
            return new uint[] { a, b, c, d };
        }

        /**
         * Circular shift to the left.
         */ 
        public uint RotateLeft(uint x, int shift)
        {
            return (x << shift) | (x >> -shift);
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
