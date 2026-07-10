namespace task14;

using System;
using System.Linq;
using System.Threading;

public class DefiniteIntegral
{
    public static double SolveInOneThread(double a, double b, Func<double, double> function, double step)
    {
        if (step <= 0)
            throw new ArgumentException("Шаг должен быть > 0", nameof(step));
        if (function == null)
            throw new ArgumentException("Null функция", nameof(function));
    
        double sign = 1.0;
        if (a > b)
        {
            (a, b) = (b, a);
            sign = -1.0;
        }
    
        double length = b - a;
        long N = (long)Math.Round(length / step);
    
        if (N <= 0)
            return sign * (b - a) * (function(a) + function(b)) / 2.0;
    
        double sum = ComputePartialSum(a, step, function, 0, N);
        return sign * sum;
    }

    public static double Solve(double a, double b, Func<double, double> function,
                               double step, int threadsnumber)
    {
        if (threadsnumber <= 0)
            throw new ArgumentException("Число потоков должно быть > 0", nameof(threadsnumber));
        if (step <= 0)
            throw new ArgumentException("Шаг должен быть > 0", nameof(step));
        if (function == null)
            throw new ArgumentException("Null функция", nameof(function));

        double sign = 1.0;
        if (a > b)
        {
            (a, b) = (b, a);
            sign = -1.0;
        }

        double length = b - a;
        long N = (long)Math.Round(length / step);

        if (N <= 0)
            return sign * (b - a) * (function(a) + function(b)) / 2.0;

        int threads = (int)Math.Min(threadsnumber, N);

        using (Barrier barrier = new Barrier(threads + 1))
        {
            double totalSum = 0.0;
            long baseCount = N / threads;
            long remainder = N % threads;

            foreach (int i in Enumerable.Range(0, threads))
            {
                new Thread(() =>
                {
                    long start = i * baseCount + Math.Min(i, remainder);
                    long count = baseCount + (i < remainder ? 1 : 0);
                    double localSum = ComputePartialSum(a, step, function, start, count);

                    double initial, computed;
                    do
                    {
                        initial = totalSum;
                        computed = initial + localSum;
                    }
                    while (Interlocked.CompareExchange(ref totalSum, computed, initial) != initial);

                    barrier.SignalAndWait();
                }).Start();
            }

            barrier.SignalAndWait();
            return sign * totalSum;
        }
    }

    private static double ComputePartialSum(double a, double step,
                                            Func<double, double> function,
                                            long start, long count)
    {
        double sum = 0.0;
        for (long k = 0; k < count; k++)
        {
            long idx = start + k;
            double x0 = a + idx * step;
            double x1 = a + (idx + 1) * step;
            sum += (step / 2.0) * (function(x0) + function(x1));
        }
        return sum;
    }
}
