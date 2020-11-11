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
using Cryptool.PluginBase.IO;
using Cryptool.PluginBase.Miscellaneous;
using Cryptool.Plugins.ChaCha.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        #region Public Variables

        public virtual bool ExecutionFinished { get; set; }

        #endregion Public Variables

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
        /// <remarks>
        /// The [Required] validation attribute does not work because CT2 seems to pass the inputs after `PreExecution`
        /// and only executes `Execute` if all inputs are given. This means that we have no way of validating if all required
        /// inputs are given because the component won't even execute without all.
        /// </remarks>
        // [Required]
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
        // [Required]
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

        #region ChaCha Cipher methods

        /// <summary>
        /// En- or decrypt input stream with ChaCha.
        /// </summary>
        /// <param name="key">The secret 128-bit or 256-bit key. A 128-bit key will be expanded into a 256-bit key by concatenation with itself.</param>
        /// <param name="iv">Initialization vector (DJB version: 64-bit, IETF version: 96-bit)</param>
        /// <param name="initialCounter">Initial counter (DJB version: 64-bit, IETF version: 32-bit)</param>
        /// <param name="settings">Chosen Settings in the Plugin workspace. Includes Rounds and Version property.</param>
        /// <param name="input">Input stream</param>
        /// <param name="output">Output stream</param>
        protected virtual void Xcrypt(byte[] key, byte[] iv, ulong initialCounter, ChaChaSettings settings, ICryptoolStream input, CStreamWriter output)
        {
            if (!(key.Length == 32 || key.Length == 16))
            {
                throw new ArgumentOutOfRangeException("key", key.Length, "Key must be exactly 128-bit or 256-bit.");
            }
            if (iv.Length != settings.Version.IVBits / 8)
            {
                throw new ArgumentOutOfRangeException("iv", iv.Length, $"IV must be exactly {settings.Version.IVBits}-bit.");
            }
            void AssertCounter(ulong counter, ulong max)
            {
                if (!(0 <= counter && counter <= max))
                {
                    throw new ArgumentOutOfRangeException("initialCounter", counter, $"Initial counter must be between 0 and {max}.");
                }
            }
            if (settings.Version == Version.DJB)
            {
                AssertCounter(initialCounter, ulong.MaxValue);
            }
            else if (settings.Version == Version.IETF)
            {
                AssertCounter(initialCounter, uint.MaxValue);
            }

            // The first 512-bit state. Reused for counter insertion.
            uint[] firstState = State(key, iv, initialCounter, settings.Version);

            // Buffer to read 512-bit input block
            byte[] inputBytes = new byte[64];
            CStreamReader inputReader = input.CreateReader();

            ulong blockCounter = initialCounter;
            int read = inputReader.Read(inputBytes);
            while (read != 0)
            {
                // Will hold the state during each keystream
                uint[] state = (uint[])firstState.Clone();
                InsertCounter(ref state, blockCounter, settings.Version);
                ChaChaHash(ref state, settings.Rounds);

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
            output.Dispose();
        }

        /// <summary>
        ///   Construct the 512-bit state with the given key, iv and counter.
        ///
        ///   The ChaCha State matrix is built up like this:
        ///   <para>
        ///     <code>
        ///       Constants  Constants_  Constants  Constants <br/>
        ///       Key______  Key_______  Key______  Key______ <br/>
        ///       Key______  Key_______  Key______  Key______ <br/>
        ///       Counter__  Counter|IV  IV_______  IV_______ <br/>
        ///     </code>
        ///   </para>
        ///   <br/>
        ///   <para>
        ///   (The counter and IV size depend on the version, thus the '|' in the last row)
        ///   </para>
        ///   <br/>
        ///   <para>
        ///     The constants depend on the key size.</para>
        ///   <para>
        ///     A 128-bit key has the constants "expand 16-byte k" encoded in ASCII
        ///     while a 256-bit has the constants "expand 32-byte k" encoded in ASCII.
        ///     A 128-bit key will be expanded into a 256-bit key by concatenation with itself.
        ///   </para>
        ///   <para>
        ///     The byte order of every 4 bytes of the constants, key, iv and counter will be reversed before insertion.
        ///   </para>
        ///   <para>
        ///     Furthermore, the byte order of the original counter value will be reversed before insertion.
        ///   </para>
        /// </summary>
        ///
        /// <example>
        ///   Input parameters:
        ///
        ///     Version:          IETF
        ///
        ///     Key (128-bit):    01:02:03:04  05:06:07:08  09:0a:0b:0c  0d:0e:0f:10
        ///
        ///     IV:               aa:bb:cc:dd  01:02:03:04  05:06:07:08
        ///
        ///     Initial counter:  00:00:00:01
        ///
        ///   Output as state matrix:
        ///
        ///                       61:70:78:65  31:20:64:6e  79:62:2d:36  6b:20:65:64
        ///                       04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                       04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                       00:00:00:01  dd:cc:bb:aa  04:03:02:01  08:07:06:05
        /// </example>
        /// <example>
        ///   Input parameters:
        ///
        ///     Version:          DJB
        ///
        ///     Key (256-bit):    01:02:03:04  05:06:07:08  09:0a:0b:0c  0d:0e:0f:10
        ///                       11:12:13:14  15:16:17:18  19:1a:1b:1c  1d:1e:1f:20
        ///
        ///     IV:               aa:bb:cc:dd  01:02:03:04
        ///
        ///     Initial counter:  00:00:00:00  00:00:00:01
        ///
        ///   Output as state matrix:
        ///
        ///                       61:70:78:65  33:20:64:6e  79:62:2d:32  6b:20:65:74
        ///                       04:03:02:01  08:07:06:05  0c:0b:0a:09  10:0f:0e:0d
        ///                       14:13:12:11  18:17:16:15  1c:1b:1a:19  20:1f:1e:1d
        ///                       00:00:00:01  00:00:00:00  dd:cc:bb:aa  04:03:02:01
        /// </example>
        /// <param name="key">
        ///   The secret 128-bit or 256-bit key.
        /// </param>
        /// <param name="iv">
        ///   Initialization vector
        /// </param>
        /// <param name="counter">
        ///   Counter for this keystream block.
        /// </param>
        /// <returns>
        ///   Initialized 512-bit ChaCha state as input for ChaCha hash function.
        /// </returns>
        private uint[] State(byte[] key, byte[] iv, ulong counter, Version version)
        {
            uint[] state = new uint[16];
            byte[] constants = key.Length == 16 ? TAU : SIGMA;

            InsertUInt32LE(ref state, constants, 0);

            ExpandKey(ref key);
            InsertUInt32LE(ref state, key, 4);

            InsertCounter(ref state, counter, version);

            InsertUInt32LE(ref state, iv, version == Version.DJB ? 14 : 13);

            return state;
        }

        /// <summary>
        /// Insert the counter into the 512-bit state matrix.
        /// Insertion depends on the version since counter size differs between versions.
        /// </summary>
        private void InsertCounter(ref uint[] state, ulong counter, Version version)
        {
            // NOTE The original value of the counter is in reversed byte order!
            // The counter 0x1 will be inserted into the state as follows
            // (example for 64-bit counter from DJB version):
            //   Original value:                0x 00 00 00 00  00 00 00 01
            //   Reverse byte order:            0x 01 00 00 00  00 00 00 00
            //   Reverse order of each 4-byte:  0x 00 00 00 01  00 00 00 00
            //   Final value:                   0x 00 00 00 01  00 00 00 00
            // (For the IETF version, this does not matter because the counter is only 32-bit).
            byte[] counterBytes = ByteUtil.GetBytesLE(version == Version.DJB ?
                counter :
                // There should be no overflow error when casting from ulong to uint here
                // because the counter boundaries should already have been checked
                (uint)counter);
            InsertUInt32LE(ref state, counterBytes, 12);
        }

        /// <summary>
        /// Insert the input elements with reversed byte order starting at specified offset.
        /// </summary>
        /// <param name="state">512-bit ChaCha state</param>
        /// <param name="input">Elements to be inserted</param>
        /// <param name="offset">State offset</param>
        private static void InsertUInt32LE(ref uint[] state, byte[] input, int offset)
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
        private static void ExpandKey(ref byte[] key)
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
        protected virtual void ChaChaHash(ref uint[] state, int rounds)
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

            AdditionStep(ref state, originalState);

            LittleEndianStep(ref state);
        }

        /// <summary>
        /// Add the two 512-bit states together.
        /// </summary>
        protected virtual void AdditionStep(ref uint[] state, uint[] originalState)
        {
            for (int i = 0; i < 16; ++i)
            {
                state[i] += originalState[i];
            }
        }

        /// <summary>
        /// Reverse byte order of each UInt32.
        /// </summary>
        protected virtual void LittleEndianStep(ref uint[] state)
        {
            for (int i = 0; i < 16; ++i)
            {
                state[i] = ByteUtil.ToUInt32LE(state[i]);
            }
        }

        /// <summary>
        /// Run a quarterround(a,b,c,d) with the given indices on the state.
        /// </summary>
        protected void Quarterround(ref uint[] state, int i, int j, int k, int l)
        {
            (state[i], state[j], state[k], state[l]) = Quarterround(state[i], state[j], state[k], state[l]);
        }

        /// <summary>
        /// Calculate the quarterround of the four inputs.
        /// </summary>
        protected virtual (uint, uint, uint, uint) Quarterround(uint a, uint b, uint c, uint d)
        {
            (a, b, d) = QuarterroundStep(a, b, d, 16);
            (c, d, b) = QuarterroundStep(c, d, b, 12);
            (a, b, d) = QuarterroundStep(a, b, d, 8);
            (c, d, b) = QuarterroundStep(c, d, b, 7);
            return (a, b, c, d);
        }

        /// <summary>
        /// Calculate one step in the quarterround function.
        /// </summary>
        protected virtual (uint, uint, uint) QuarterroundStep(uint x1, uint x2, uint x3, int shift)
        {
            x1 += x2;
            x3 ^= x1;
            x3 = ByteUtil.RotateLeft(x3, shift);
            return (x1, x2, x3);
        }

        #endregion ChaCha Cipher methods

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
        public virtual System.Windows.Controls.UserControl Presentation
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
        public virtual void Execute()
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
                // Since maximum initial counter by any version is 64-bit, we cast it to UInt64.
                ulong initialCounter = (ulong)InitialCounter;
                Xcrypt(InputKey, InputIV, initialCounter, settings, InputStream, outputWriter);
                OnPropertyChanged("OutputStream");
            }

            ExecutionFinished = true;

            ProgressChanged(1, 1);
        }

        /// <summary>
        /// Called once after workflow execution has stopped.
        /// </summary>
        public virtual void PostExecution()
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

        public virtual bool IsValid { get; set; }

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
        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Validate(null);
        }

        #endregion Validation

        #region Event Handling

        public event StatusChangedEventHandler OnPluginStatusChanged;

        public event GuiLogNotificationEventHandler OnGuiLogNotificationOccured;

        public event PluginProgressChangedEventHandler OnPluginProgressChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void GuiLogMessage(string message, NotificationLevel logLevel)
        {
            EventsHelper.GuiLogMessage(OnGuiLogNotificationOccured, this, new GuiLogEventArgs(message, this, logLevel));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
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