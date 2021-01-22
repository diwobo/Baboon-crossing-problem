using System;
using System.Threading;
using System.Threading.Tasks;

public class Apes
{
    private static Task[] apes;
    private static Semaphore rope;
    private static Semaphore separation;
    private static Semaphore releaseRope;
    private static Semaphore north;
    private static Semaphore south;
    private const int waitTime = 5;
    private const int maxRopeCount = 5;
    private const int apeCount = 5000;
    private static int apesOnRope = 0;

    public static void Main()
    {
        rope = new Semaphore(maxRopeCount, maxRopeCount);
        separation = new Semaphore(0, 1);
        releaseRope = new Semaphore(1, 1);
        north = new Semaphore(1, 1);
        south = new Semaphore(1, 1);
        apes = new Task[apeCount];

        apeGenerator();
        Thread.Sleep(1000);

        Console.WriteLine(": Start :");
        Console.WriteLine("-----");
        separation.Release(1);
        Task.WaitAll(apes);

        Console.WriteLine(": End :");
        Console.ReadKey();
    }
    public static void apeGenerator()
    {
        Random rnd = new Random();
        for (int i = 0; i < apeCount; i++)
        {
            apes[i] = Task.Run(() =>
            {
                if (rnd.Next(0, 2) == 0)
                    goNorth();
                else
                    goSouth();
            });
        }
        return;
    }
    public static void goNorth()
    {
        QueueRope(south, north);

        rope.WaitOne();
        try
        {
            Console.Write("S");
            south.Release();
            Thread.Sleep(waitTime);
        }
        finally
        {
            releaseRope.WaitOne();
            rope.Release();
            apesOnRope--;
            if (apesOnRope == 0)
            {
                Console.WriteLine();
                Console.WriteLine("-----");
                north.Release();
            }
            releaseRope.Release();
        }
        return;
    }
    public static void goSouth()
    {
        QueueRope(north,south);

        rope.WaitOne();
        try
        {
            Console.Write("N");
            north.Release();
            Thread.Sleep(waitTime);
        }
        finally
        {
            releaseRope.WaitOne();
            rope.Release();
            apesOnRope--;
            if (apesOnRope == 0)
            {
                Console.WriteLine();
                Console.WriteLine("-----");
                south.Release();
            }
            releaseRope.Release();
        }
        return;
    }
    private static void QueueRope(Semaphore myGate, Semaphore closeGate)
    {
        separation.WaitOne();
        myGate.WaitOne();
        if (apesOnRope == 0)
        {
            closeGate.WaitOne();
        }
        apesOnRope++;
        separation.Release();
        return;
    }
    
    
}