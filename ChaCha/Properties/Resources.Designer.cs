﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cryptool.Plugins.ChaCha.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cryptool.Plugins.ChaCha.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ChaCha.
        /// </summary>
        internal static string ChaChaCaption {
            get {
                return ResourceManager.GetString("ChaChaCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A stream cipher based on Salsa20 and used in TLS. Developed by Daniel J. Bernstein..
        /// </summary>
        internal static string ChaChaTooltip {
            get {
                return ResourceManager.GetString("ChaChaTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initial counter.
        /// </summary>
        internal static string InputInitialCounterCaption {
            get {
                return ResourceManager.GetString("InputInitialCounterCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initial keystream block counter. Will be incremented for each keystream block. 64-bit for DJB version. 32-bit for IETF version..
        /// </summary>
        internal static string InputInitialCounterTooltip {
            get {
                return ResourceManager.GetString("InputInitialCounterTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initialization vector.
        /// </summary>
        internal static string InputIVCaption {
            get {
                return ResourceManager.GetString("InputIVCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initialization vector. 64-bit for DJB version. 96-bit for IETF version..
        /// </summary>
        internal static string InputIVTooltip {
            get {
                return ResourceManager.GetString("InputIVTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Key.
        /// </summary>
        internal static string InputKeyCaption {
            get {
                return ResourceManager.GetString("InputKeyCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Key. Can be 128-bit or 256-bit. A 128-bit key will be expanded into a 256-bit key via concatenation with itself..
        /// </summary>
        internal static string InputKeyTooltip {
            get {
                return ResourceManager.GetString("InputKeyTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Plain text.
        /// </summary>
        internal static string InputStreamCaption {
            get {
                return ResourceManager.GetString("InputStreamCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input text which should be encrypted or decrypted with ChaCha..
        /// </summary>
        internal static string InputStreamTooltip {
            get {
                return ResourceManager.GetString("InputStreamTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cipher text.
        /// </summary>
        internal static string OutputStreamCaption {
            get {
                return ResourceManager.GetString("OutputStreamCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output text encrypted or decrypted with ChaCha..
        /// </summary>
        internal static string OutputStreamTooltip {
            get {
                return ResourceManager.GetString("OutputStreamTooltip", resourceCulture);
            }
        }
    }
}
