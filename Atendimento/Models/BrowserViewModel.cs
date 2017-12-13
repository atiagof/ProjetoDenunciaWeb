using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class BrowserViewModel
    {

        #region Propriedades 

        //
        // Summary:
        //     Gets a value indicating whether the browser is a beta version.
        //
        // Returns:
        //     true if the browser is a beta version; otherwise, false. The default is false.
        public bool Beta { get; set; }
        //
        // Summary:
        //     Gets the browser string (if any) that was sent by the browser in the User-Agent
        //     request header.
        //
        // Returns:
        //     The contents of the User-Agent request header sent by the browser.
        public string Browser { get; set; }
        //
        // Summary:
        //     Gets an System.Collections.ArrayList of the browsers in the System.Web.Configuration.HttpCapabilitiesBase.Capabilities
        //     dictionary.
        //
        // Returns:
        //     An System.Collections.ArrayList of the browsers in the System.Web.Configuration.HttpCapabilitiesBase.Capabilities
        //     dictionary.
        public ArrayList Browsers { get; set; }
        //
        // Summary:
        //     Gets the version of the .NET Framework that is installed on the client.
        //
        // Returns:
        //     The common language runtime System.Version.
        public Version ClrVersion { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser supports cookies.
        //
        // Returns:
        //     true if the browser supports cookies; otherwise, false. The default is false.
        public bool Cookies { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser is a search engine Web crawler.
        //
        // Returns:
        //     true if the browser is a search engine; otherwise, false. The default is false.
        public bool Crawler { get; set; }
        //
        // Summary:
        //     Gets the version number of ECMAScript that the browser supports.
        //
        // Returns:
        //     The version number of ECMAScript that the browser supports.
        public Version EcmaScriptVersion { get; set; }
        //
        // Summary:
        //     Gets the major version number of the wireless gateway used to access the server,
        //     if known.
        //
        // Returns:
        //     The major version number of the wireless gateway used to access the server, if
        //     known. The default is 0.
        //
        // Exceptions:
        //   T:System.Web.HttpUnhandledException:
        //     The major version number of the wireless gateway cannot be parsed.
        public virtual int GatewayMajorVersion { get; set; }
        //
        // Summary:
        //     Gets the minor version number of the wireless gateway used to access the server,
        //     if known.
        //
        // Returns:
        //     The minor version number of the wireless gateway used to access the server, if
        //     known. The default is 0.
        //
        // Exceptions:
        //   T:System.Web.HttpUnhandledException:
        //     The minor version number of the wireless gateway cannot be parsed.
        public virtual double GatewayMinorVersion { get; set; }
        //
        // Summary:
        //     Gets the version of the wireless gateway used to access the server, if known.
        //
        // Returns:
        //     The version number of the wireless gateway used to access the server, if known.
        //     The default is None.
        public virtual string GatewayVersion { get; set; }
        //
        // Summary:
        //     Returns the type of input supported by browser.
        //
        // Returns:
        //     The type of input supported by browser. The default is telephoneKeypad.
        public virtual string InputType { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser has a color display.
        //
        // Returns:
        //     true if the browser has a color display; otherwise, false. The default is false.
        public virtual bool IsColor { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser is a recognized mobile device.
        //
        // Returns:
        //     true if the browser is a recognized mobile device; otherwise, false. The default
        //     is true.
        public virtual bool IsMobileDevice { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser supports Java.
        //
        // Returns:
        //     true if the browser supports Java; otherwise, false. The default is false.
        public bool JavaApplets { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser supports JavaScript.
        //
        // Returns:
        //     true if the browser supports JavaScript; otherwise, false. The default is false.
        [Obsolete("The recommended alternative is the EcmaScriptVersion property. A Major version value greater than or equal to 1 implies JavaScript support. http://go.microsoft.com/fwlink/?linkid=14202")]
        public bool JavaScript { get; set; }
        //
        // Summary:
        //     Gets the JScript version that the browser supports.
        //
        // Returns:
        //     The System.Version of JScript that the browser supports.
        public Version JScriptVersion { get; set; }
        //
        // Summary:
        //     Gets the major (integer) version number of the browser.
        //
        // Returns:
        //     The major version number of the browser.
        //
        // Exceptions:
        //   T:System.Exception:
        //     The major version value is not an integer.
        public int MajorVersion { get; set; }
        //
        // Summary:
        //     Gets the maximum length in characters for the href attribute of an HTML <a> (anchor)
        //     element.
        //
        // Returns:
        //     The maximum length in characters for the href attribute of an HTML <a> (anchor)
        //     element.
        public virtual int MaximumHrefLength { get; set; }
        //
        // Summary:
        //     Gets the maximum length of the page, in bytes, which the browser can display.
        //
        // Returns:
        //     The maximum length of the page, in bytes, which the browser can display. The
        //     default is 2000.
        public virtual int MaximumRenderedPageSize { get; set; }
        //
        // Summary:
        //     Returns the maximum length of the text that a soft-key label can display.
        //
        // Returns:
        //     The maximum length of the text that a soft-key label can display. The default
        //     is 5.
        public virtual int MaximumSoftkeyLabelLength { get; set; }
        //
        // Summary:
        //     Gets the minor (that is, decimal) version number of the browser.
        //
        // Returns:
        //     The minor version number of the browser.
        //
        // Exceptions:
        //   T:System.Web.HttpUnhandledException:
        //     The minor version number in the header is not valid.
        public double MinorVersion { get; set; }
        //
        // Summary:
        //     Gets the minor (decimal) version number of the browser as a string.
        //
        // Returns:
        //     The minor version number of the browser.
        public string MinorVersionString { get; set; }
        //
        // Summary:
        //     Returns the name of the manufacturer of a mobile device, if known.
        //
        // Returns:
        //     The name of the manufacturer of a mobile device, if known. The default is Unknown.
        public virtual string MobileDeviceManufacturer { get; set; }
        //
        // Summary:
        //     Gets the model name of a mobile device, if known.
        //
        // Returns:
        //     The model name of a mobile device, if known. The default is Unknown.
        public virtual string MobileDeviceModel { get; set; }
        //
        // Summary:
        //     Gets the version of Microsoft HTML (MSHTML) Document Object Model (DOM) that
        //     the browser supports.
        //
        // Returns:
        //     The number of the MSHTML DOM version that the browser supports.
        public Version MSDomVersion { get; set; }
        //
        // Summary:
        //     Returns the number of soft keys on a mobile device.
        //
        // Returns:
        //     The number of soft keys supported on a mobile device. The default is 0.
        public virtual int NumberOfSoftkeys { get; set; }
        //
        // Summary:
        //     Gets the name of the platform that the client uses, if it is known.
        //
        // Returns:
        //     The operating system that the client uses, if it is known, otherwise the value
        //     is set to Unknown.
        public string Platform { get; set; }
        //
        // Summary:
        //     Used internally to get a value indicating whether to use an optimized cache key.
        //
        // Returns:
        //     true to use an optimized cache key; otherwise, false. The default is false.
        public bool UseOptimizedCacheKey { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the browser supports Visual Basic Scripting edition
        //     (VBScript).
        //
        // Returns:
        //     true if the browser supports VBScript; otherwise, false. The default is false.
        public bool VBScript { get; set; }
        //
        // Summary:
        //     Gets the full version number (integer and decimal) of the browser as a string.
        //
        // Returns:
        //     The full version number of the browser as a string.
        public string Version { get; set; }
        //
        // Summary:
        //     Gets the version of the World Wide Web Consortium (W3C) XML Document Object Model
        //     (DOM) that the browser supports.
        //
        // Returns:
        //     The number of the W3C XML DOM version number that the browser supports.
        public Version W3CDomVersion { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the client is a Win16-based computer.
        //
        // Returns:
        //     true if the browser is running on a Win16-based computer; otherwise, false. The
        //     default is false.
        public bool Win16 { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the client is a Win32-based computer.
        //
        // Returns:
        //     true if the client is a Win32-based computer; otherwise, false. The default is
        //     false.
        public bool Win32 { get; set; }

        #endregion

    }
}