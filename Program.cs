using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample07
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        public void Run()
        {
            ParallelLoopResult loopResult = new ParallelLoopResult();
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = cts.Token;
                Console.WriteLine("\nStart Run");
                Task t = Task.Run(() => DoCancel(cts), cts.Token);
                Console.WriteLine("\nParallel.For");
                loopResult = Parallel.For(0, 50000, po, (index, loopState) =>
                {
                    DoWork(index, loopState);
                });
            }
            catch (AggregateException aex)
            {
                Console.WriteLine("\nAggregateException in Run: " + aex.Message);
                aex.Flatten();
                foreach (Exception ex in aex.InnerExceptions)
                    Console.WriteLine("  Exception: " + ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("\nOperationCanceledException in Run:\n" + ex.Message);
            }
            finally
            {
                Console.WriteLine("\nloopResult.IsCompleted: " + loopResult.IsCompleted);
                Console.WriteLine("\nEnd Run");
                Console.ReadLine();
            }
        }
        private void DoCancel(CancellationTokenSource cts)
        {
            // cts.Cancel(); // this will invoke the OperationCanceledException in Run()
            Console.WriteLine("Cancel in 2sec");
            cts.CancelAfter(2000);
        }
        private void DoWork(int index, ParallelLoopState loopState)
        {
            double temp = 1.1;
            try
            {
                // create an exception.
                if (index == 25000)
                {
                    int a = 1;
                    a = a / --a;
                }
                // doing some work
                for (int i = 0; i < 5000; i++)
                {
                    temp = Math.Sin(index) + Math.Sqrt(index) *
                            Math.Pow(index, 3.1415) + temp;
                    if (loopState.ShouldExitCurrentIteration)
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException DoWork:\n" + ex);
                throw;
            }
        }
    }
}
