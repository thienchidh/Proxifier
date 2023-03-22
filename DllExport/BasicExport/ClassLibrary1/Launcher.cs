using System;
using System.Collections.Generic;
using ClassLibrary1;
using Proxifier.Core;
using Proxifier.Models;

namespace Proxifier
{
    public class Launcher
    {
        private static ProxyRouter _proxyRouter;

        [DllExport]
        public static void doStart(int fiddlerPort)
        {
            Console.WriteLine("Starting proxy router on port " + fiddlerPort);
            try
            {
                _proxyRouter?.Shutdown();
                _proxyRouter = new ProxyRouter((ushort) fiddlerPort, new List<Process>());

                _proxyRouter.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport]
        public static void updateProxy(
            int processId,
            string serverIp,
            int serverPort,
            int serverType,
            string username,
            string password)
        {
            Console.WriteLine("Updating proxy for process " + processId + " to " + serverIp + ":" + serverPort);
            try
            {
                _proxyRouter.ProcessesList.RemoveAll(p => p.Pid == processId);
                _proxyRouter.ProcessesList.Add(new Process(processId, new ServerInfo
                {
                    ServerIp = serverIp,
                    ServerPort = serverPort,
                    ServerType = (ServerType) serverType,
                    Username = username,
                    Password = password
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport]
        public static void removeProxy(int processId)
        {
            Console.WriteLine("Removing proxy for process " + processId);
            try
            {
                _proxyRouter.ProcessesList.RemoveAll(p => p.Pid == processId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport]
        public static void clearProxy()
        {
            Console.WriteLine("Clearing all proxies");
            try
            {
                _proxyRouter.ProcessesList.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        [DllExport]
        public static void doStop()
        {
            try
            {
                Console.WriteLine("Stopping proxy router");
                _proxyRouter.Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}