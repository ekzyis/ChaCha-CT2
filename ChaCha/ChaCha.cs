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
        /// Is optional. Default is 0.
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
        /// <param name="initialCounter">Initial counter (64-bit). Can be any value between 0 and including ulong.MaxValue.</param>
        /// <param name="rounds">ChaCha Hash Rounds per keystream block. Can be 8, 12, or 20.</param>
        /// <param name="input">Input stream</param>
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
        /// Construct the 512-bit state with the given key, iv and counter of ChaCha DJB version.
        ///
        /// The ChaCha State matrix is built up like this:
        ///
        ///   Constants   Constants     Constants   Constants
        ///   Key         Key           Key         Key
        ///   Key         Key           Key         Key
        ///   Counter     Counter       IV          IV
        ///
        /// The constants depends on the key size. A 128-bit key has the constants "expand 16-byte k" encoded in ASCII
        /// while a 256-bit has the constants "expand 32-byte k" encoded in ASCII.
        /// A 128-bit key will be expanded into a 256-bit key by concatenation with itself.
        /// The byte order of every 4 bytes of the constants, key, iv and counter will be reversed before insertion.
        /// Furthermore, the byte order of the original counter value will be reversed before insertion.
        /// </summary>
        ///
        /// <example>
        /// Input parameters:
        ///
        ///   Key (128-bit):    01:02:03:04  05:06:07:08  09:0a:0b:0c  0d:0e:0f:10
        ///   IV:               aa:bb:cc:dd  01:02:03:04
        ///   Initial counter:  00:00:00:00  00:00:00:01
        ///
        /// Output as state matrix:
        ///
        ///                     61:70:78:65  33:20:64:6e  79:62:2d:32  6b:20:65:64
        ///                     04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                     04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                     00:00:00:01: 00:00:00:00  dd:cc:bb:aa  04:03:02:01
        /// </example>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatentation with itself.</param>
        /// <param name="iv">Initialization vector (64-bit)</param>
        /// <param name="counter">Counter for this keystream block. Can be between 0 and including ulong.MaxValue.</param>
        /// <returns>Initialized 512-bit ChaCha state as input for ChaCha hash function.</returns>
        public static uint[] StateDJB(byte[] key, byte[] iv, ulong counter)
        {
            uint[] state = new uint[16];
            byte[] constants = key.Length == 16 ? TAU : SIGMA;

            InsertUInt32LE(ref state, constants, 0);

            ExpandKey(ref key);
            InsertUInt32LE(ref state, key, 4);

            InsertCounterDJB(ref state, counter);

            InsertUInt32LE(ref state, iv, 14);

            return state;
        }

        /// <summary>
        /// Set the counter into the ChaCha state matrix of DJB version.
        /// </summary>
        public static void InsertCounterDJB(ref uint[] state, ulong counter)
        {
            // NOTE The original value of the counter is in reversed byte order!
            // The counter 0x1 will be inserted into the state as follows:
            //   Original value:                0x 00 00 00 00  00 00 00 01
            //   Reverse byte order:            0x 01 00 00 00  00 00 00 00
            //   Reverse order of each 4-byte:  0x 00 00 00 01  00 00 00 00
            //   Final value:                   0x 00 00 00 01  00 00 00 00
            byte[] counterBytes = ByteUtil.GetBytesLE(counter);
            // but we still also reverse byte order of each UInt32
            InsertUInt32LE(ref state, counterBytes, 12);
        }

        #endregion ChaCha Cipher methods (DJB)

        #region ChaCha Cipher methods (IETF)

        /// <summary>
        /// En- or decrypt input stream with ChaCha IETF version.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatenation with itself.</param>
        /// <param name="iv">Initialization vector (96-bit)</param>
        /// <param name="initialCounter">Initial counter (32-bit). Can be any value between 0 and including uint.MaxValue.</param>
        /// <param name="rounds">ChaCha Hash Rounds per keystream block. Can be 8, 12, or 20.</param>
        /// <param name="input">Input stream</param>
        /// <param name="output">Output stream</param>
        public static void XcryptIETF(byte[] key, byte[] iv, ulong initialCounter, int rounds, ICryptoolStream input, CStreamWriter output)
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
            uint initialCounter32 = (uint)initialCounter;

            // The first 512-bit state. Reused for counter insertion.
            uint[] firstState = StateIETF(key, iv, initialCounter32);

            // Buffer to read 512-bit input block
            byte[] inputBytes = new byte[64];
            CStreamReader inputReader = input.CreateReader();

            uint blockCounter = initialCounter32;
            int read = inputReader.Read(inputBytes);
            while (read != 0)
            {
                // Will hold the state during each keystream
                uint[] state = (uint[])firstState.Clone();
                InsertCounterIETF(ref state, blockCounter);
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
        /// Construct the 512-bit state with the given key, iv and counter of ChaCha IETF version.
        ///
        /// The ChaCha State matrix is built up like this:
        ///
        ///   Constants   Constants     Constants   Constants
        ///   Key         Key           Key         Key
        ///   Key         Key           Key         Key
        ///   Counter     IV            IV          IV
        ///
        /// The constants depends on the key size. A 128-bit key has the constants "expand 16-byte k" encoded in ASCII
        /// while a 256-bit has the constants "expand 32-byte k" encoded in ASCII.
        /// A 128-bit key will be expanded into a 256-bit key by concatenation with itself.
        /// The byte order of every 4 bytes of the constants, key, iv and counter will be reversed before insertion.
        /// Furthermore, the byte order of the original counter value will be reversed before insertion.
        /// </summary>
        ///
        /// <example>
        /// Input parameters:
        ///
        ///   Key (128-bit):    01:02:03:04  05:06:07:08  09:0a:0b:0c  0d:0e:0f:10
        ///   IV:               aa:bb:cc:dd  01:02:03:04  05:06:07:08
        ///   Initial counter:  00:00:00:01
        ///
        /// Output as state matrix:
        ///
        ///                     61:70:78:65  33:20:64:6e  79:62:2d:32  6b:20:65:64
        ///                     04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                     04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                     00:00:00:01  dd:cc:bb:aa  04:03:02:01  08:07:06:05
        /// </example>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatentation with itself.</param>
        /// <param name="iv">Initialization vector (96-bit)</param>
        /// <param name="counter">Counter for this keystream block. Can be between 0 and including uint.MaxValue.</param>
        /// <returns>Initialized 512-bit ChaCha state as input for ChaCha hash function.</returns>
        public static uint[] StateIETF(byte[] key, byte[] iv, uint counter)
        {
            uint[] state = new uint[16];
            byte[] constants = key.Length == 16 ? TAU : SIGMA;

            InsertUInt32LE(ref state, constants, 0);

            ExpandKey(ref key);
            InsertUInt32LE(ref state, key, 4);

            InsertCounterIETF(ref state, counter);

            InsertUInt32LE(ref state, iv, 13);

            return state;
        }

        /// <summary>
        /// Set the counter into the ChaCha state matrix of IETF version.
        /// </summary>
        public static void InsertCounterIETF(ref uint[] state, uint counter)
        {
            // NOTE In the DJB version, the original value of the counter is in reversed byte order
            // but it does not matter for the IETF version because the counter is only 32-bit.
            // and we reverse each 4 byte chunk again, anyway. See following example:
            // The counter 0x1 will be inserted into the state as follows:
            //   Original value:                0x 00 00 00 01
            //   Reverse byte order:            0x 01 00 00 00
            //   Reverse order of each 4-byte:  0x 00 00 00 01
            //   Final value:                   0x 00 00 00 01
            byte[] counterBytes = ByteUtil.GetBytesLE(counter);
            // but we still also reverse byte order of each UInt32
            InsertUInt32LE(ref state, counterBytes, 12);
        }

        #endregion ChaCha Cipher methods (IETF)

        #region ChaCha Cipher general methods

        /// <summary>
        /// Insert the input elements with reversed byte order starting at specified offset.
        /// </summary>
        /// <param name="state">512-bit ChaCha state</param>
        /// <param name="input">Elements to be inserted</param>
        /// <param name="offset">State offset</param>
        public static void InsertUInt32LE(ref uint[] state, byte[] input, int offset)
        {
            for (int i = 0; i < input.Length / 4; ++i)
            {
                state[i + offset] = ByteUtil.ToUInt32LE(input, i * 4);
            }
        }

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

        /// <summary>
        /// Calculate the quarterround of the four inputs.
        /// </summary>
        /// <param name="a">Input a</param>
        /// <param name="b">Input b</param>
        /// <param name="c">Input c</param>
        /// <param name="d">Input d</param>
        /// <returns></returns>
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
                    // We pass in the initial counter here as UInt64 as well to prevent accidental overflow.
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