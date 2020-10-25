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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;

namespace Cryptool.Plugins.ChaCha
{
    [Author("Ramdip Gill", "rgill@cryptool.org", "CrypTool 2 Team", "https://www.cryptool.org")]
    [PluginInfo("ChaCha", "A stream cipher based on Salsa used in TLS and developed by Daniel J. Bernstein.", "ChaCha/userdoc.xml", new[] { "CrypWin/images/default.png" })]
    [ComponentCategory(ComponentCategory.CiphersModernSymmetric)]
    public class ChaCha : ICrypComponent, IValidatableObject
    {
        #region Private Variables

        private readonly ChaChaSettings settings = new ChaChaSettings();

        #endregion Private Variables

        #region ICrypComponent I/O

        private byte[] inputKey;
        private byte[] inputIV;

        // user can override initial counter given by version
        private BigInteger initialCounter;

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
            get { return this.inputKey; }
            set
            {
                this.inputKey = value;
                OnPropertyChanged("InputKey");
            }
        }

        /// <summary>
        /// Initialization vector chosen by the user.
        /// </summary>
        [IVValidator("IV must be 64-bit in DJB version or 96-bit in IETF version")]
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

        /// <summary>
        /// Counter value for the first keystream block. Will be incremented for each keystream block.
        /// </summary>
        [InitialCounterValidator("Counter must be 64-bit in DJB Version or 32-bit in IETF version")]
        [PropertyInfo(Direction.InputData, "InputInitialCounterCaption", "InputInitialCounterTooltip", false)]
        public BigInteger InitialCounter
        {
            get { return this.initialCounter; }
            set
            {
                this.initialCounter = value;
                OnPropertyChanged("InitialCounter");
            }
        }

        [PropertyInfo(Direction.OutputData, "OutputStreamCaption", "OutputStreamTooltip", true)]
        public ICryptoolStream OutputStream
        {
            get;
            set;
        }

        #endregion ICrypComponent I/O

        #region ChaCha Cipher methods

        /// <summary>
        /// En- or decrypt input stream with ChaCha.
        /// </summary>
        /// <param name="key">The secret key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="initialCounter"></param>
        /// <param name="input">input message</param>
        /// <returns></returns>
        public static ICryptoolStream Xcrypt(byte[] key, byte[] iv, ulong initialCounter, ICryptoolStream input)
        {
            // Make sure that the byte at index 0 is the most significant byte
            // such that the beginning of the byte array corresponds to the beginning of a byte hex string
            // (when starting reading from the left).
            //
            //   0x 12 34 56 78
            //      ^
            //      Beginning of byte hex string
            //
            // If byte array is in big-endian, the byte 0x12 would be at the zero-th index.
            // We do this by guaranteeing that the key byte order is in big-endian, independent of system architecture.

            ByteUtil.ConvertToBigEndian(ref key);
            ByteUtil.ConvertToBigEndian(ref iv);

            return null;
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
                // If InitialCounter is not set by user, it defaults to zero.
                // Since maximum initial counter is 64-bit, we cast it to UInt64.
                OutputStream = ChaCha.Xcrypt(InputKey, InputIV, (ulong)InitialCounter, InputStream);
                System.Console.WriteLine("Input valid");
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