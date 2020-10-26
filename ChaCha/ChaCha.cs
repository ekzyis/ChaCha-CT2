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
using Cryptool.PluginBase.Miscellaneous;
using Cryptool.Plugins.ChaCha.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Cryptool.Plugins.ChaCha
{
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    [PluginInfo("ChaCha", "A stream cipher based on Salsa used in TLS and developed by Daniel J. Bernstein.", "ChaCha/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    [ComponentCategory(ComponentCategory.CiphersModernSymmetric)]
    public class ChaCha : ICrypComponent, IValidatableObject
    {
        #region Private Variables

        private readonly ChaChaSettings settings = new ChaChaSettings();

        private static readonly byte[] SIGMA = Encoding.ASCII.GetBytes("expand 32-byte k");
        private static readonly byte[] TAU = Encoding.ASCII.GetBytes("expand 16-byte k");

        private CStreamWriter outputWriter;

        #endregion Private Variables

        #region ICrypComponent I/O

        /// <summary>
        /// Input text which should be en- or decrypted by ChaCha.
        /// </summary>
        [PropertyInfo(Direction.InputData, "InputStreamCaption", "InputStreamTooltip", true)]
        public ICryptoolStream InputStream
        {
            get;
            set;
        }

        /// <summary>
        /// Key chosen by the user which will be used for en- or decryption.
        /// </summary>
        [KeyValidator("Key must be 128-bit or 256-bit")]
        [PropertyInfo(Direction.InputData, "InputKeyCaption", "InputKeyTooltip", true)]
        public byte[] InputKey
        {
            get;
            set;
        }

        /// <summary>
        /// Initialization vector chosen by the user.
        /// </summary>
        [IVValidator("IV must be 64-bit in DJB version or 96-bit in IETF version")]
        [PropertyInfo(Direction.InputData, "InputIVCaption", "InputIVTooltip", true)]
        public byte[] InputIV
        {
            get;
            set;
        }

        /// <summary>
        /// Counter value for the first keystream block. Will be incremented for each keystream block.
        /// </summary>
        [InitialCounterValidator("Counter must be 64-bit in DJB Version or 32-bit in IETF version")]
        [PropertyInfo(Direction.InputData, "InputInitialCounterCaption", "InputInitialCounterTooltip", false)]
        public BigInteger InitialCounter
        {
            get;
            set;
        }

        [PropertyInfo(Direction.OutputData, "OutputStreamCaption", "OutputStreamTooltip", true)]
        public ICryptoolStream OutputStream
        {
            get
            {
                return outputWriter;
            }
        }

        #endregion ICrypComponent I/O

        #region ChaCha Cipher methods (DJB)

        /// <summary>
        /// En- or decrypt input stream with ChaCha DJB version.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatenation with itself.</param>
        /// <param name="iv">Initialization vector (64-bit)</param>
        /// <param name="initialCounter">Initial counter. Can be any value between 0 and including ulong.MaxValue.</param>
        /// <param name="rounds">ChaCha Hash Rounds per keystream block.</param>
        /// <param name="input">Input message which we want to en- or decyrpt.</param>
        /// <param name="output">Output stream</param>
        public static void XcryptDJB(byte[] key, byte[] iv, ulong initialCounter, int rounds, ICryptoolStream input, CStreamWriter output)
        {
            if (!(key.Length == 32 || key.Length == 16))
            {
                throw new ArgumentOutOfRangeException("key", key.Length, "Key must be exactly 128-bit or 256-bit.");
            }
            if (iv.Length != 8)
            {
                throw new ArgumentOutOfRangeException("iv", iv.Length, "IV must be exactly 64-bit.");
            }
            if (!(0 <= initialCounter && initialCounter <= ulong.MaxValue))
            {
                throw new ArgumentOutOfRangeException("initialCounter", initialCounter, $"Initial counter must be between 0 and {ulong.MaxValue}.");
            }

            // The first 512-bit state. Reused for counter insertion.
            uint[] firstState = StateDJB(key, iv, initialCounter);

            // Buffer to read 512-bit input block
            byte[] inputBytes = new byte[64];
            CStreamReader inputReader = input.CreateReader();

            ulong blockCounter = initialCounter;
            int read = inputReader.Read(inputBytes);
            while (read != 0)
            {
                // Will hold the state during each keystream
                uint[] state = (uint[])firstState.Clone();
                InsertCounterDJB(ref state, blockCounter);
                ChaChaHash(ref state, rounds);

                byte[] stateBytes = ByteUtil.ToByteArray(state);
                byte[] c = ByteUtil.XOR(stateBytes, inputBytes, read);
                output.Write(c);

                blockCounter++;

                // Read next input block
                read = inputReader.Read(inputBytes);
            }
            inputReader.Dispose();
            output.Flush();
            output.Close();
        }

        /// <summary>
        /// Construct the 512-bit state with the given key, iv and counter.
        ///
        /// The ChaCha State matrix is built up like this:
        ///
        ///   Constants   Constants     Constants   Constants
        ///   Key         Key           Key         Key
        ///   Key         Key           Key         Key
        ///   Counter     Counter       IV          IV
        ///
        /// The byte order of every 4 bytes of the constants, key, iv and counter will be reversed before insertion.
        /// Furthermore, the byte order of the original counter value will be reversed before insertion.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatentation with itself.</param>
        /// <param name="iv">Initialization vector (64-bit)</param>
        /// <param name="counter">Counter for this keystream block. Can be between 0 and including ulong.MaxValue.</param>
        /// <returns>Initialized 512-bit ChaCha state as input for ChaCha hash function.</returns>
        public static uint[] StateDJB(byte[] key, byte[] iv, ulong counter)
        {
            uint[] state = new uint[16];
            byte[] constants = key.Length == 16 ? TAU : SIGMA;
            for (int i = 0; i < 4; ++i)
            {
                state[i] = ByteUtil.ToUInt32LE(constants, i * 4);
            }

            ExpandKey(ref key);
            for (int i = 0; i < 8; ++i)
            {
                state[i + 4] = ByteUtil.ToUInt32LE(key, i * 4);
            }

            InsertCounterDJB(ref state, counter);

            for (int i = 0; i < 2; ++i)
            {
                state[i + 14] = ByteUtil.ToUInt32LE(iv, i * 4);
            }

            return state;
        }

        /// <summary>
        /// Set the counter into the state matrix.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="counter"></param>
        public static void InsertCounterDJB(ref uint[] state, ulong counter)
        {
            // NOTE The original value of the counter is in reversed byte order!
            // The counter 0x1 will be inserted into the state as follows:
            //   Original value:                0x 00 00 00 00  00 00 00 01
            //   Reverse byte order:            0x 01 00 00 00  00 00 00 00
            //   Reverse order of each 4-byte:  0x 00 00 00 01  00 00 00 00
            //   Final value:                   0x 00 00 00 01  00 00 00 00
            byte[] counterBytes = ByteUtil.GetBytesLE(counter);
            for (int i = 0; i < 2; ++i)
            {
                // but we still also reverse byte order of each UInt32
                state[i + 12] = ByteUtil.ToUInt32LE(counterBytes, i * 4);
            }
        }

        #endregion ChaCha Cipher methods (DJB)

        #region ChaCha Cipher methods (IETF)

        /// <summary>
        /// En- or decrypt input stream with ChaCha IETF version.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatenation with itself.</param>
        /// <param name="iv">Initialization vector (96-bit)</param>
        /// <param name="initialCounter">Initial counter. Can be any value between 0 and including uint.MaxValue.</param>
        /// <param name="rounds">ChaCha Hash Rounds per keystream block.</param>
        /// <param name="input">Input message which we want to en- or decrpyt.</param>
        /// <param name="output">Output stream</param>
        /// <returns></returns>
        public static ICryptoolStream XcryptIETF(byte[] key, byte[] iv, ulong initialCounter, int rounds, ICryptoolStream input, CStreamWriter output)
        {
            if (!(key.Length == 32 || key.Length == 16))
            {
                throw new ArgumentOutOfRangeException("key", key.Length, "Key must be exactly 128-bit or 256-bit.");
            }
            if (iv.Length != 12)
            {
                throw new ArgumentOutOfRangeException("iv", iv.Length, "IV must be exactly 96-bit.");
            }
            if (!(0 <= initialCounter && initialCounter <= uint.MaxValue))
            {
                throw new ArgumentOutOfRangeException("initialCounter", initialCounter, $"Initial counter must be between 0 and {uint.MaxValue}.");
            }

            byte[] state = StateIETF(key, iv, initialCounter);

            return null;
        }

        /// <summary>
        /// Construct the 512-bit state with the given key, iv and counter.
        ///
        /// The ChaCha State matrix is built up like this:
        ///
        ///   Constants   Constants     Constants   Constants
        ///   Key         Key           Key         Key
        ///   Key         Key           Key         Key
        ///   Counter     IV            IV          IV
        ///
        /// The byte order of every 4 bytes of the constants, key, iv and counter will be reversed before insertion.
        /// Furthermore, the byte order of the original counter value will be reversed before insertion.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatentation with itself.</param>
        /// <param name="iv">Initialization vector (96-bit)</param>
        /// <param name="counter">Counter for this keystream block. Can be between 0 and including uint.MaxValue.</param>
        /// <returns>Initialized 512-bit ChaCha state as input for ChaCha hash function.</returns>
        public static byte[] StateIETF(byte[] key, byte[] iv, ulong counter)
        {
            return null;
        }

        #endregion ChaCha Cipher methods (IETF)

        #region ChaCha Cipher general methods

        /// <summary>
        /// Expands the key to a 256-bit key.
        /// </summary>
        /// <param name="key">128-bit or 256-bit key</param>
        public static void ExpandKey(ref byte[] key)
        {
            if (key.Length == 32)
            {
                // Key already 256-bit. Nothing to do.
                return;
            }
            byte[] expand = new byte[32];
            key.CopyTo(expand, 0);
            key.CopyTo(expand, 16);
            key = expand;
        }

        /// <summary>
        /// Calculate the ChaCha Hash of the given 512-bit state.
        /// </summary>
        /// <param name="state">The state which will be hashed</param>
        /// <param name="rounds">Rounds of hash function</param>
        public static void ChaChaHash(ref uint[] state, int rounds)
        {
            if (!(rounds == 8 || rounds == 12 || rounds == 20))
            {
                throw new ArgumentException("Rounds must be 8, 12 or 20.");
            }

            // The original state before ChaCha hash function was applied. Needed for state addition afterwards.
            uint[] originalState = (uint[])state.Clone();
            for (int i = 0; i < rounds / 2; ++i)
            {
                // Column rounds
                Quarterround(ref state, 0, 4, 8, 12);
                Quarterround(ref state, 1, 5, 9, 13);
                Quarterround(ref state, 2, 6, 10, 14);
                Quarterround(ref state, 3, 7, 11, 15);
                // Diagonal rounds
                Quarterround(ref state, 0, 5, 10, 15);
                Quarterround(ref state, 1, 6, 11, 12);
                Quarterround(ref state, 2, 7, 8, 13);
                Quarterround(ref state, 3, 4, 9, 14);
            }

            // add states and reverse byte order of each UInt32
            for (int i = 0; i < 16; ++i)
            {
                state[i] += originalState[i];
            }
            // reverse byte order of each UInt32
            for (int i = 0; i < 16; ++i)
            {
                state[i] = ByteUtil.ToUInt32LE(state[i]);
            }
        }

        /// <summary>
        /// Run a quarterround(a,b,c,d) with the given indices on the state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="i">state index of first element</param>
        /// <param name="j">state index of second element</param>
        /// <param name="k">state index of third element</param>
        /// <param name="l">state index of fourth element</param>
        public static void Quarterround(ref uint[] state, int i, int j, int k, int l)
        {
            (state[i], state[j], state[k], state[l]) = Quarterround(state[i], state[j], state[k], state[l]);
        }

        public static (uint, uint, uint, uint) Quarterround(uint a, uint b, uint c, uint d)
        {
            (uint, uint, uint) QuarterroundStep(uint x1, uint x2, uint x3, int shift)
            {
                x1 += x2;
                x3 ^= x1;
                x3 = ByteUtil.RotateLeft(x3, shift);
                return (x1, x2, x3);
            }
            (a, b, d) = QuarterroundStep(a, b, d, 16);
            (c, d, b) = QuarterroundStep(c, d, b, 12);
            (a, b, d) = QuarterroundStep(a, b, d, 8);
            (c, d, b) = QuarterroundStep(c, d, b, 7);
            return (a, b, c, d);
        }

        #endregion ChaCha Cipher general methods

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
        public System.Windows.Controls.UserControl Presentation
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

            GuiLogMessage($"Executing ChaCha.", NotificationLevel.Info);
            GuiLogMessage($"Settings: {settings}", NotificationLevel.Info);
            GuiLogMessage($"Key: {InputKey.Length * 8}-bit, IV: {InputIV.Length * 8}-bit, Initial counter: {InitialCounter}", NotificationLevel.Info);

            Validate();
            if (IsValid)
            {
                outputWriter = new CStreamWriter();
                // If InitialCounter is not set by user, it defaults to zero.
                // Since maximum initial counter is 64-bit, we cast it to UInt64.
                ulong initialCounter = (ulong)InitialCounter;
                if (settings.Version == Version.DJB)
                {
                    ChaCha.XcryptDJB(InputKey, InputIV, initialCounter, settings.Rounds, InputStream, outputWriter);
                }
                else
                {
                    ChaCha.XcryptIETF(InputKey, InputIV, initialCounter, settings.Rounds, InputStream, outputWriter);
                }
                OnPropertyChanged("OutputStream");
            }

            ProgressChanged(1, 1);
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

        #endregion IPlugin Members

        #region Validation

        private bool IsValid { get; set; }

        /// <summary>
        /// Validates user input.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(InputKey, new ValidationContext(this) { MemberName = "InputKey" }, results);
            Validator.TryValidateProperty(InputIV, new ValidationContext(this) { MemberName = "InputIV" }, results);
            Validator.TryValidateProperty(InitialCounter, new ValidationContext(this) { MemberName = "InitialCounter" }, results);
            if (results.Count == 0)
            {
                GuiLogMessage("Input valid", NotificationLevel.Info);
                IsValid = true;
            }
            else
            {
                GuiLogMessage($"Input invalid: {string.Join(", ", results.Select(r => r.ErrorMessage))}", NotificationLevel.Error);
                IsValid = false;
            }
            return results;
        }

        /// <summary>
        /// Convenience method to call Validate without a validation context
        /// since ChaCha itself needs no validation context.
        /// </summary>
        public IEnumerable<ValidationResult> Validate()
        {
            return Validate(null);
        }

        #endregion Validation

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

        #endregion Event Handling
    }
}