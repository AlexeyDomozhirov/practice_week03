namespace task14tests;

using System;
using Xunit;
using task14;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_SingleThread_ReturnsCorrectValue()
    {
        static double f(double x) => x * x;
        double result = DefiniteIntegral.Solve(0, 1, f, 1e-5, 1);
        Assert.Equal(1.0 / 3.0, result, 1e-4);
    }

    [Fact]
    public void Solve_MultipleThreads_ReturnsCorrectValue()
    {
        static double f(double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, f, 1e-5, 8);
        Assert.Equal(0.0, result, 1e-4);
    }

    [Fact]
    public void Solve_InvertedBounds_ReturnsNegativeIntegral()
    {
        static double f(double x) => x;
        double forward = DefiniteIntegral.Solve(0, 2, f, 1e-5, 2);
        double backward = DefiniteIntegral.Solve(2, 0, f, 1e-5, 2);
        Assert.Equal(forward, -backward, 1e-6);
        Assert.Equal(2.0, forward, 1e-4);
    }

    [Fact]
    public void Solve_StepExceedsInterval_ReturnsTrapezoidApproximation()
    {
        static double f(double x) => x;
        double result = DefiniteIntegral.Solve(0, 1, f, 2.0, 1);
        Assert.Equal(0.5, result, 1e-10);
    }

    [Fact]
    public void Solve_ZeroLengthInterval_ReturnsZero()
    {
        static double f(double x) => Math.Exp(x);
        double result = DefiniteIntegral.Solve(3.0, 3.0, f, 1e-5, 4);
        Assert.Equal(0.0, result, 1e-10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Solve_InvalidThreadCount_ThrowsArgumentException(int threads)
    {
        static double f(double x) => x;
        var ex = Assert.Throws<ArgumentException>(
            () => DefiniteIntegral.Solve(0, 1, f, 1e-4, threads));
        Assert.Contains("Число потоков должно быть > 0", ex.Message);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.1)]
    public void Solve_InvalidStep_ThrowsArgumentException(double step)
    {
        static double f(double x) => x;
        var ex = Assert.Throws<ArgumentException>(
            () => DefiniteIntegral.Solve(0, 1, f, step, 2));
        Assert.Contains("Шаг должен быть > 0", ex.Message);
    }

    [Fact]
    public void Solve_LinearFunction_ExactWithinPrecision()
    {
        static double f(double x) => 2 * x + 3;
        double result = DefiniteIntegral.Solve(0, 4, f, 1e-6, 4);
        Assert.Equal(28.0, result, 1e-5);
    }
}
