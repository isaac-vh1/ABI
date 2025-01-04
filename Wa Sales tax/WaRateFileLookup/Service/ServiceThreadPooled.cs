/*
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using WaRateFiles.Support;
using WaRateFiles;

namespace RateLookupService
{
	/// <summary>
	/// Thread pooled rate lookup service.  Set USE_THREADPOOL to true in the app.config to use this version.
	/// </summary>
	internal class ServiceThreadPooled : IRateService
	{
		private int m_timeout;
		private int m_port;
		private RateLookup m_lookup;
		private PerformanceCounters m_counters;
		private Thread m_thread;
		private volatile bool m_running = false;
		private TcpListener m_sockListener;
		
		private DateTime m_inception = DateTime.Now;
		private long m_requestCount;

		public RateLookup Locator
		{
			get { return m_lookup; }
		}

		public ServiceThreadPooled(int timeout, int port, string addrfn, string ratefn, string zipfn, bool useShortcutEval, PerformanceCounters counters)
		{
			m_counters = counters;
			m_timeout = timeout;
			m_port = port;
			m_sockListener = new TcpListener(IPAddress.Any, m_port);
			m_lookup = new RateLookup(addrfn, ratefn, zipfn, RateLookupEngine.STANDARDIZER, useShortcutEval);
		}

		public ServiceThreadPooled(int timeout, int port, bool useShortcutEval, PerformanceCounters counters)
		{
			m_counters = counters;
			m_timeout = timeout;
			m_port = port;
			m_sockListener = new TcpListener(IPAddress.Any, m_port);
			m_lookup = new RateLookup(RateLookupEngine.STANDARDIZER, useShortcutEval);
		}

		public void Start()
		{
			if (m_running)
			{
				throw new Exception("Already running");
			}
			m_sockListener.Start(100);
			m_running = true;
			ThreadStart ts = new ThreadStart(Run);
			m_thread = new Thread(ts);
			m_thread.IsBackground = false;
			m_thread.Start();
		}

		public void Stop()
		{
			m_running = false;
			m_sockListener.Stop();
			m_thread.Join();
		}

		private void Run()
		{
			LogFile.SysWriteLog("ServiceThread.run", "Startup " + DateTime.Now.ToString());

			while (m_running)
			{
				try
				{
					TcpClient sock = m_sockListener.AcceptTcpClient();
					//LogFile.WriteLog("Run", "Accepted " + sock.Client.RemoteEndPoint.ToString());
					//sock.ReceiveTimeout = m_timeout;
					//sock.SendTimeout = m_timeout;

					// Should probably use an ObjectPool for LookupTask 
					LookupTask task = new LookupTask(sock, m_lookup, m_port, m_counters, PerformanceCounters.CurrentTick, m_inception, m_requestCount++);
					ThreadPool.QueueUserWorkItem(new WaitCallback(task.ThreadPoolCallback), task.GetHashCode());
				}
				catch (Exception ex)
				{
					LogFile.SysWriteLog("ServiceThread.Run", ex);
				}
			}
			m_running = false;
			LogFile.SysWriteLog("ServiceThread.run", "Shutting down " + DateTime.Now.ToString());
		}
	}
}
