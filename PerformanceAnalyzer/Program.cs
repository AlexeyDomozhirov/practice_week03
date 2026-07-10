using System;
using task14;
using System.Diagnostics;
using System.Linq;
using ScottPlot;

class Program
{
    // Вспомогательный метод для измерения времени выполнения
    static (double timeMs, double result) Measure(Action act, int warmup = 2, int runs = 5)
    {
        for (int i = 0; i < warmup; i++) act();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++) act();
        sw.Stop();
        return (sw.Elapsed.TotalMilliseconds / runs, 0);
    }

    static void Main()
    {
        double a = -100, b = 100;
        Func<double, double> sin = Math.Sin;
        double exact = 0.0; // интеграл sin(x) на [-100,100] равен 0

        // 1. Определение оптимального шага (наибольший, дающий точность 1e-4)
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        Console.WriteLine("Шаг        | Время (мс)    | Погрешность");
        double stepOpt = steps[0];
        foreach (var step in steps)
        {
            var (time, res) = Measure(() => DefiniteIntegral.Solve(a, b, sin, step, 1), 2, 5);
            Console.WriteLine($"{res:F19}"); // Вывод: 3.14
            double error = Math.Abs(res - exact);
            Console.WriteLine($"{step,6:E1}   | {time,12:F4}   | {error,12:E2}");
            // Выбираем первый (самый крупный) шаг, обеспечивающий точность
            if (error <= 1e-4)
            {
                stepOpt = step;
                break; // прекращаем перебор, так как идём от крупного к мелкому
            }
        }
        Console.WriteLine($"\nВыбран шаг: {stepOpt} (интервалов: {(b - a) / stepOpt:F0})");

        // 2. Подбор оптимального числа потоков
        int maxThreads = Environment.ProcessorCount * 2;
        int[] threadCounts = Enumerable.Range(1, maxThreads).ToArray();
        double[] multiTimes = new double[maxThreads];

        Console.WriteLine("\nПотоков | Время (мс)");
        for (int i = 0; i < maxThreads; i++)
        {
            int threads = threadCounts[i];
            var (time, _) = Measure(() => DefiniteIntegral.Solve(a, b, sin, stepOpt, threads), 3, 7);
            multiTimes[i] = time;
            Console.WriteLine($"{threads,7} | {time,11:F4}");
        }

        // 3. Сравнение с однопоточной версией (без потоков)
        var (singleTime, _) = Measure(() => DefiniteIntegral.SolveInOneThread(a, b, sin, stepOpt), 5, 10);
        double minMultiTime = multiTimes.Min();
        int bestThreads = threadCounts[Array.IndexOf(multiTimes, minMultiTime)];
        double gain = (singleTime - minMultiTime) / singleTime * 100.0;

        Console.WriteLine($"\nОднопоточная версия (без потоков): {singleTime:F4} мс");
        Console.WriteLine($"Лучшая многопоточная: {minMultiTime:F4} мс ({bestThreads} потоков)");
        Console.WriteLine($"Ускорение: {gain:F2}%");

        // 4. Построение графика (по заданию: OX – время, OY – число потоков)
        var plt = new Plot();

        // Точки многопоточных замеров: X = время, Y = число потоков
        var xs = multiTimes.Select(t => (double)t).ToArray();
        var ys = threadCounts.Select(t => (double)t).ToArray();
        var scatter = plt.Add.Scatter(xs, ys);
        scatter.Color = Colors.Blue;
        scatter.MarkerSize = 5;
        scatter.LegendText = "Многопоточная";

        // Вертикальная линия для однопоточного времени
        var vLine = plt.Add.VerticalLine(singleTime);
        vLine.Color = Colors.Red;
        vLine.LinePattern = LinePattern.Dashed;
        vLine.LegendText = "Однопоточная";

        plt.XLabel("Среднее время (мс)");
        plt.YLabel("Число потоков");
        plt.Title("Производительность DefiniteIntegral.Solve");
        plt.ShowLegend();
        plt.Legend.Alignment = Alignment.UpperRight;

        string plotPath = "performance_plot.png";
        plt.SavePng(plotPath, 800, 600);
        Console.WriteLine($"\nГрафик сохранён в {plotPath}");

        // 5. Формирование отчёта
        string report = $@"
           Оптимальный шаг (наибольший, дающий точность 1e-4): {stepOpt}
           Число интервалов: {(b - a) / stepOpt:F0}
           Однопоточное время (без потоков): {singleTime:F4} мс
           Лучшее многопоточное время: {minMultiTime:F4} мс (потоков: {bestThreads})
           Ускорение: {gain:F2}%
           {(gain >= 15 ? "Многопоточная версия эффективна (>15%)" : "Требуется оптимизация (<15%)")}";
        string reportPath = "results.txt";
        File.WriteAllText(reportPath, report);
        Console.WriteLine($"Отчёт сохранён в {reportPath}");
    }
}
