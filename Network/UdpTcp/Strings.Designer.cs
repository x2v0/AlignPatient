﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MSR.LST.Net {
   /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class Strings {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    var temp = new ResourceManager("P.Net.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All integer values (size, index, length) must be &gt;= 0.
        /// </summary>
        internal static string AllIntegerValuesMustBePositive {
            get {
                return ResourceManager.GetString("AllIntegerValuesMustBePositive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BufferChunk does not accept or create null objects.
        /// </summary>
        internal static string BufferChunkDoesNotAcceptNull {
            get {
                return ResourceManager.GetString("BufferChunkDoesNotAcceptNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BufferChunk does not accept or create zero-length objects.
        /// </summary>
        internal static string BufferChunkDoesNotAcceptZeroLength {
            get {
                return ResourceManager.GetString("BufferChunkDoesNotAcceptZeroLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BufferChunk does not have enough buffer or data to fulfill your request. Requested space:{0} Actual space:{1}.
        /// </summary>
        internal static string BufferChunkDoesNotHaveEnoughBuffer {
            get {
                return ResourceManager.GetString("BufferChunkDoesNotHaveEnoughBuffer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BufferChunk does not have enough data to fulfill your request. Requested data:{0} Actual data:{1}.
        /// </summary>
        internal static string BufferChunkDoesNotHaveEnoughData {
            get {
                return ResourceManager.GetString("BufferChunkDoesNotHaveEnoughData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DelayBetweenPackets must be in the range of 0 to 30.
        /// </summary>
        internal static string DelayBetweenPacketsRange {
            get {
                return ResourceManager.GetString("DelayBetweenPacketsRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DropPacketsSentPercent must be between 0 and 100.
        /// </summary>
        internal static string DropPacketsSentPercentRange {
            get {
                return ResourceManager.GetString("DropPacketsSentPercentRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index and Length do not point to valid data index:{0} + length:{1} must be &lt;= dataLength:{2}.
        /// </summary>
        internal static string IndexAndLengthInvalidData {
            get {
                return ResourceManager.GetString("IndexAndLengthInvalidData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MulticastUdpListener already disposed.
        /// </summary>
        internal static string MulticastUdpListenerAlreadyDisposed {
            get {
                return ResourceManager.GetString("MulticastUdpListenerAlreadyDisposed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Null pointers are not valid arguments..
        /// </summary>
        internal static string NullPointersAreInvalid {
            get {
                return ResourceManager.GetString("NullPointersAreInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Read .
        /// </summary>
        internal static string ReadData {
            get {
                return ResourceManager.GetString("ReadData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sock does not exist as a shared socket..
        /// </summary>
        internal static string SockDoesNotExistAsASharedSocket {
            get {
                return ResourceManager.GetString("SockDoesNotExistAsASharedSocket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UdpSender already disposed.
        /// </summary>
        internal static string UdpSenderAlreadyDisposed {
            get {
                return ResourceManager.GetString("UdpSenderAlreadyDisposed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find a local routing interface, is no network present?.
        /// </summary>
        internal static string UnableToFindLocalRoutingInterface {
            get {
                return ResourceManager.GetString("UnableToFindLocalRoutingInterface", resourceCulture);
            }
        }
    }
}