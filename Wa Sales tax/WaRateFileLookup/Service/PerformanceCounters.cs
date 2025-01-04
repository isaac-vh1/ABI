using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using WaRateFiles.Support;

namespace RateLookupService
{
	public class PerformanceCounters
	{
		/// <summary>
		/// Counter for counting total number of operations
		/// </summary>
		private PerformanceCounter m_totalOperations;
		/// <summary>
		/// Counter for counting number of operations per second
		/// </summary>
		private PerformanceCounter m_operationsPerSecond;
		/// <summary>
		/// Counter for counting duration averages
		/// </summary>
		private PerformanceCounter m_averageDuration;
		/// <summary>
		/// Counter for counting duration averages base
		/// </summary>
		private PerformanceCounter m_averageDurationBase;

		/// <summary>
		/// The performance counter category name.
		/// </summary>
		private string m_category;

		/// <summary>
		/// Performance counter category description.
		/// </summary>
		private string m_categoryDesc;

		/// <summary>
		/// If true, use the lower performance thread safe counter calls. 
		/// </summary>
		private bool m_threadSafe;

		/// <summary>
		/// True if setup OK
		/// </summary>
		private bool m_initialized;

		/// <summary>
		/// This is more accurate than DateTime.Now.Ticks, but I don't think it 
		/// will work on mono.
		/// </summary>
		/// <param name="ticks">current tick count</param>
		//[DllImport("Kernel32.dll")]
		//public static extern void QueryPerformanceCounter(ref long ticks);

		public PerformanceCounters(string category, string categoryDesc, bool threadSafe)
		{
			m_category = category;
			m_categoryDesc = categoryDesc;
			m_threadSafe = threadSafe;

			SetupPerformanceCounters();

			if (!m_initialized)
			{
				return;
			}

			try
			{
				m_totalOperations = new PerformanceCounter();
				m_totalOperations.CategoryName = m_category;
				m_totalOperations.CounterName = "# requests";
				m_totalOperations.MachineName = ".";
				m_totalOperations.ReadOnly = false;
				m_totalOperations.RawValue = 0;

				m_operationsPerSecond = new PerformanceCounter();
				m_operationsPerSecond.CategoryName = m_category;
				m_operationsPerSecond.CounterName = "# requests / sec";
				m_operationsPerSecond.MachineName = ".";
				m_operationsPerSecond.ReadOnly = false;
				m_operationsPerSecond.RawValue = 0;

				m_averageDuration = new PerformanceCounter();
				m_averageDuration.CategoryName = m_category;
				m_averageDuration.CounterName = "average time per request";
				m_averageDuration.MachineName = ".";
				m_averageDuration.ReadOnly = false;
				m_averageDuration.RawValue = 0;

				m_averageDurationBase = new PerformanceCounter();
				m_averageDurationBase.CategoryName = m_category;
				m_averageDurationBase.CounterName = "average time per request base";
				m_averageDurationBase.MachineName = ".";
				m_averageDurationBase.ReadOnly = false;
				m_averageDurationBase.RawValue = 0;
			}
			catch (Exception ex)
			{
				LogFile.SysWriteLog("PerformanceCounters()", ex);
				m_initialized = false;
			}
		}

		public static long CurrentTick
		{
			get
			{
				//long ticks;
				//QueryPerformanceCounter(ref ticks);
				//return ticks;

				//return DateTime.Now.Ticks;
				
				return (long)Environment.TickCount;
			}
		}

		/// <summary>
		/// Increments counters.
		/// </summary>
		/// <param name="ticks">The number of ticks the AverageTimer32 counter must be incremented by</param>
		public void Update(long ticks)
		{
			if (!m_initialized)
			{
				return;
			}
			if (m_threadSafe)
			{
				m_totalOperations.Increment();
				m_operationsPerSecond.Increment();
				m_averageDuration.IncrementBy(ticks); // increment the timer by the time cost of the operation
				m_averageDurationBase.Increment(); // increment base counter only by 1
			}
			else
			{
				m_totalOperations.RawValue++;
				m_operationsPerSecond.RawValue++;
				m_averageDuration.RawValue += ticks; // increment the timer by the time cost of the operation
				m_averageDurationBase.RawValue++; // increment base counter only by 1
			}
		}

		private void SetupPerformanceCounters()
		{
			try
			{
				if (PerformanceCounterCategory.Exists(m_category))
				{
					m_initialized = true;
					return;
				}

				CounterCreationDataCollection counters = new CounterCreationDataCollection();

				// 1. counter for counting totals: PerformanceCounterType.NumberOfItems32
				CounterCreationData totalOps = new CounterCreationData();
				totalOps.CounterName = "# requests";
				totalOps.CounterHelp = "Total number requests";
				totalOps.CounterType = PerformanceCounterType.NumberOfItems32;
				counters.Add(totalOps);

				// 2. counter for counting operations per second:
				CounterCreationData opsPerSecond = new CounterCreationData();
				opsPerSecond.CounterName = "# requests / sec";
				opsPerSecond.CounterHelp = "Number of requests per second";
				opsPerSecond.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
				counters.Add(opsPerSecond);

				// 3. counter for counting average time per operation:
				CounterCreationData avgDuration = new CounterCreationData();
				avgDuration.CounterName = "average time per request";
				avgDuration.CounterHelp = "Average duration per request";
				avgDuration.CounterType = PerformanceCounterType.AverageTimer32;
				counters.Add(avgDuration);

				// 4. base counter for counting average time
				CounterCreationData avgDurationBase = new CounterCreationData();
				avgDurationBase.CounterName = "average time per request base";
				avgDurationBase.CounterHelp = "Average duration per request base";
				avgDurationBase.CounterType = PerformanceCounterType.AverageBase;
				counters.Add(avgDurationBase);

				// create new category with the counters above
				PerformanceCounterCategory.Create(m_category, m_categoryDesc, PerformanceCounterCategoryType.SingleInstance, counters);

				m_initialized = true;
			}
			catch (Exception ex)
			{
				LogFile.SysWriteLog("SetupPerformanceCounters", ex);
				m_initialized = false;
			}
		}
	}
}
