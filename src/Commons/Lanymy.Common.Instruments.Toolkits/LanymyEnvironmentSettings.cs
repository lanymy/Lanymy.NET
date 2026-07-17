using System;
using System.Net;
using System.Threading;

namespace Lanymy.Common.Instruments
{



    public class LanymyEnvironmentSettings
    {

        /// <summary>
        /// 每核的线程数，最少 25
        /// </summary>
        private const byte MAX_THREADS_PER_PROCESSOR_COUNT = 25;

        /// <summary>
        /// 线程倍数，最少 2 倍
        /// </summary>
        private const byte MAX_THREADS_MULTIPLES = 2;



        private static int _CurrentDefaultMaxWorkerThreads = 0;
        private static int _CurrentDefaultMaxCompletionPortThreads = 0;
        private static byte _CurrentDefaultMaxPerProcessorCount = 0;

        private static void CheckDefaultMaxThreads()
        {

            if (_CurrentDefaultMaxWorkerThreads <= 0 || _CurrentDefaultMaxCompletionPortThreads <= 0)
            {

                ThreadPool.GetMaxThreads(out int currentWorkerThreads, out int currentPortThreads);
                _CurrentDefaultMaxWorkerThreads = currentWorkerThreads;
                _CurrentDefaultMaxCompletionPortThreads = currentPortThreads;

                var defaultMaxPerProcessorCount = (byte)(_CurrentDefaultMaxWorkerThreads / Environment.ProcessorCount);

                if (defaultMaxPerProcessorCount < MAX_THREADS_PER_PROCESSOR_COUNT)
                {
                    defaultMaxPerProcessorCount = MAX_THREADS_PER_PROCESSOR_COUNT;
                }

                _CurrentDefaultMaxPerProcessorCount = defaultMaxPerProcessorCount;

            }

        }

        /// <summary>
        /// 按线程倍数设置最大线程数
        /// </summary>
        /// <param name="multiples">最少 2 倍</param>
        /// <returns></returns>
        public static bool SetMaxThreadsByMultiples(byte multiples = MAX_THREADS_MULTIPLES)
        {

            if (multiples < MAX_THREADS_MULTIPLES)
            {
                multiples = MAX_THREADS_MULTIPLES;
            }

            CheckDefaultMaxThreads();

            return SetMaxThreads(_CurrentDefaultMaxWorkerThreads * multiples, _CurrentDefaultMaxCompletionPortThreads * multiples);

        }

        /// <summary>
        /// 按每个 CPU 的线程数设置最大线程数
        /// </summary>
        /// <param name="perProcessorCount">每核的线程数，最少 25</param>
        /// <returns></returns>
        public static bool SetMaxThreadsByPerProcessorCount(byte perProcessorCount = MAX_THREADS_PER_PROCESSOR_COUNT)
        {

            if (perProcessorCount < _CurrentDefaultMaxPerProcessorCount)
            {
                perProcessorCount = _CurrentDefaultMaxPerProcessorCount;
            }

            var processorCounts = Environment.ProcessorCount * perProcessorCount;

            return SetMaxThreads(processorCounts, processorCounts);

        }



        public static bool SetMaxThreads(int workerThreads, int portThreads)
        {

            CheckDefaultMaxThreads();

            if (workerThreads < _CurrentDefaultMaxWorkerThreads)
            {
                workerThreads = _CurrentDefaultMaxWorkerThreads;
            }

            if (portThreads < _CurrentDefaultMaxCompletionPortThreads)
            {
                portThreads = _CurrentDefaultMaxCompletionPortThreads;
            }

            return ThreadPool.SetMaxThreads(workerThreads, portThreads);

        }




    }

}
