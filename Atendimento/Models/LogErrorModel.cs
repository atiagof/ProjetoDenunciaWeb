using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class LogErrorModel
    {

        #region Propriedades

        public DateTime data { get; set; }
        public string ip { get; set; }
        public string browser { get; set; }
        public string log { get; set; }

        #endregion

        #region Construtor

        public LogErrorModel(HttpRequestMessage reqMsg, string log)
        {
            string browser = GetBrowserInfo();

            this.data = DateTime.Now;
            this.ip = GetClientIp(reqMsg);
            this.browser = browser.Length > 2000 ? browser.Substring(0,2000) : browser;
            this.log = log.Length > 3000 ? log.Substring(0,3000) : log;
        }

        #endregion

        #region Methods

        public static string GetBrowserInfo()
        {
            BrowserViewModel browserInfo = new Models.BrowserViewModel();

            browserInfo.Beta = HttpContext.Current.Request.Browser.Beta;
            browserInfo.Browser = HttpContext.Current.Request.Browser.Browser;
            browserInfo.Browsers = HttpContext.Current.Request.Browser.Browsers;
            browserInfo.ClrVersion = HttpContext.Current.Request.Browser.ClrVersion;
            browserInfo.Cookies = HttpContext.Current.Request.Browser.Cookies;
            browserInfo.Crawler = HttpContext.Current.Request.Browser.Crawler;
            browserInfo.EcmaScriptVersion = HttpContext.Current.Request.Browser.EcmaScriptVersion;
            browserInfo.GatewayMajorVersion = HttpContext.Current.Request.Browser.GatewayMajorVersion;
            browserInfo.GatewayMinorVersion = HttpContext.Current.Request.Browser.GatewayMinorVersion;
            browserInfo.GatewayVersion = HttpContext.Current.Request.Browser.GatewayVersion;
            browserInfo.InputType = HttpContext.Current.Request.Browser.InputType;
            browserInfo.IsColor = HttpContext.Current.Request.Browser.IsColor;
            browserInfo.IsMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;
            browserInfo.JavaApplets = HttpContext.Current.Request.Browser.JavaApplets;
            browserInfo.JavaScript = HttpContext.Current.Request.Browser.JavaScript;
            browserInfo.JScriptVersion = HttpContext.Current.Request.Browser.JScriptVersion;
            browserInfo.MajorVersion = HttpContext.Current.Request.Browser.MajorVersion;
            browserInfo.MaximumHrefLength = HttpContext.Current.Request.Browser.MaximumHrefLength;
            browserInfo.MaximumRenderedPageSize = HttpContext.Current.Request.Browser.MaximumRenderedPageSize;
            browserInfo.MaximumSoftkeyLabelLength = HttpContext.Current.Request.Browser.MaximumSoftkeyLabelLength;
            browserInfo.MinorVersion = HttpContext.Current.Request.Browser.MinorVersion;
            browserInfo.MinorVersionString = HttpContext.Current.Request.Browser.MinorVersionString;
            browserInfo.MobileDeviceManufacturer = HttpContext.Current.Request.Browser.MobileDeviceManufacturer;
            browserInfo.MobileDeviceModel = HttpContext.Current.Request.Browser.MobileDeviceModel;
            browserInfo.MSDomVersion = HttpContext.Current.Request.Browser.MSDomVersion;
            browserInfo.NumberOfSoftkeys = HttpContext.Current.Request.Browser.NumberOfSoftkeys;
            browserInfo.Platform = HttpContext.Current.Request.Browser.Platform;
            browserInfo.UseOptimizedCacheKey = HttpContext.Current.Request.Browser.UseOptimizedCacheKey;
            browserInfo.VBScript = HttpContext.Current.Request.Browser.VBScript;
            browserInfo.Version = HttpContext.Current.Request.Browser.Version;
            browserInfo.W3CDomVersion = HttpContext.Current.Request.Browser.W3CDomVersion;
            browserInfo.Win16 = HttpContext.Current.Request.Browser.Win16;
            browserInfo.Win32 = HttpContext.Current.Request.Browser.Win32;

            return JsonConvert.SerializeObject(browserInfo);
        }

        public static string GetClientIp(HttpRequestMessage request = null)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}