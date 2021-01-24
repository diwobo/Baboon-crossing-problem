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
    private const  int waitTime     =  10;
    private const  int maxRopeCount =   5;
    private const  int apeCount     = 100;
    private static int apesOnRope   =   0;

    public static void Main()
    {
        rope        = new Semaphore(maxRopeCount, maxRopeCount);
        separation  = new Semaphore(0, 1);
        releaseRope = new Semaphore(1, 1);
        north       = new Semaphore(1, 1);
        south       = new Semaphore(1, 1);
        apes        = new Task[apeCount];

        ApeGenerator();
        Thread.Sleep(1000);

        Console.WriteLine(": Start :");
        Console.WriteLine("-----");

        separation.Release(1);
        Task.WaitAll(apes);

        Console.WriteLine(": End :");
        Console.ReadKey();
    }
    public static void ApeGenerator()
    {
        Random rnd = new Random();
        for (int i = 0; i < apeCount; i++)
        {
            apes[i] = Task.Run(() =>
            {
                if (rnd.Next(2) == 0)
                    GoNorth();
                else
                    GoSouth();
            });
        }
        return;
    }
    public static void GoNorth()
    {
        QueueRope(south, north);
 
        rope.WaitOne();
        try
        {
            EnterRope(south, "N");
        }
        finally
        {
            ReleaseRope(north);
        }
        return;
    }
    public static void GoSouth()
    {
        QueueRope(north, south);

        rope.WaitOne();
        try
        {
            EnterRope(north, "S");
        }
        finally
        {
            ReleaseRope(south);
        }
        return;
    }
    private static void QueueRope(Semaphore startGate, Semaphore closeGate)
    {
        separation.WaitOne();
        startGate.WaitOne();
        if (apesOnRope == 0)
        {
            closeGate.WaitOne();
        }
        apesOnRope++;
        separation.Release();
        return;
    }
    private static void EnterRope(Semaphore start, string direction)
    {
        Console.Write(direction);
        start.Release();
        Thread.Sleep(waitTime);
    }
    private static void ReleaseRope(Semaphore releaseGate)
    {
        releaseRope.WaitOne();
        rope.Release();
        apesOnRope--;
        if (apesOnRope == 0)
        {
            Console.WriteLine();
            Console.WriteLine("-----");
            releaseGate.Release();
        }
        releaseRope.Release();
    }
}