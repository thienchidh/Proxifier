using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using Fiddler;
using Proxifier.Models;

namespace Proxifier.Core
{
    public class ProxyRouter
    {
        public ProxyRouter(ushort fiddlerPort, List<Process> processesList)
        {
            FiddlerPort = fiddlerPort;
            ProcessesList = processesList;
        }

        public ushort FiddlerPort { set; get; }

        public List<Process> ProcessesList { set; get; }


        public void Shutdown()
        {
            FiddlerApplication.BeforeRequest -= beforeRequest;
            FiddlerApplication.OnValidateServerCertificate -= onValidateServerCertificate;

            if (FiddlerApplication.oProxy != null)
            {
                FiddlerApplication.oProxy.Detach();
            }

            FiddlerApplication.Shutdown();
        }

        public void Start()
        {
            FiddlerApplication.BeforeRequest += beforeRequest;
            FiddlerApplication.OnValidateServerCertificate += onValidateServerCertificate;

            var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .ListenOnPort(FiddlerPort)
                .RegisterAsSystemProxy()
                .MonitorAllConnections()
                .CaptureFTP()
                .Build();

            FiddlerApplication.Startup(startupSettings);
        }


        private static void onValidateServerCertificate(object sender, ValidateServerCertificateEventArgs e)
        {
            if (SslPolicyErrors.None == e.CertificatePolicyErrors)
                return;

            e.ValidityState = CertificateValidity.ForceValid;
        }

        private void beforeRequest(Session oSession)
        {
            var process = ProcessesList.FirstOrDefault(p => p.Pid == oSession.LocalProcessID);
            if (process == null)
            {
                return;
            }

            var processServerInfo = process.ServerInfo;
            var serverType = processServerInfo.ServerType == ServerType.Socks ? "socks=" : "";
            oSession["X-OverrideGateway"] =
                $"{serverType}{processServerInfo.ServerIp}:{processServerInfo.ServerPort}";

            if (string.IsNullOrWhiteSpace(processServerInfo.Username) ||
                string.IsNullOrWhiteSpace(processServerInfo.Password)) return;

            var userCredentials = $"{processServerInfo.Username}:{processServerInfo.Password}";
            setBasicAuthenticationHeaders(oSession, userCredentials);
        }

        private static void setBasicAuthenticationHeaders(Session oSession, string userCredentials)
        {
            var base64UserCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userCredentials));
            oSession.RequestHeaders["Proxy-Authorization"] = $"Basic {base64UserCredentials}";
        }
    }
}