using System; 

public class Timer 
{
    public Action run;
    long time;
    long fireAt;
    bool running;
    bool once;

    Timer next;

    static Timer timerQueue;
    static Timer timersToAdd;

    public Timer(int timeout, bool once = false)
    {
        time = timeout * 10000;
        fireAt = DateTime.Now.Ticks + time;
        running = true;
        next = timersToAdd;
        timersToAdd = this;
        this.once = once;
    }

    public void stop()
    {
        running = false;
    }

    public static Timer delay(Action func, int duration)
    {
        Timer timer = new Timer(duration, true);
        timer.run = func;
        return timer;
    }

    public static void ProcessTimers()
    {
        Timer ptr = timersToAdd;
        timersToAdd = null;
        while (ptr != null)
        {
            var next = ptr.next;
            ptr.next = timerQueue;
            timerQueue = ptr;
            ptr = next;
        }

        ptr = timerQueue;
        Timer pre = null;
        long now = System.DateTime.Now.Ticks;

        while (ptr != null)
        {
            if (!ptr.running)
            {
                if (pre != null)
                    pre.next = ptr.next;
                else
                    timerQueue = timerQueue.next;
                ptr.run = null;
            }
            else
            {
                if (ptr.fireAt <= now)
                {
                    ptr.fireAt += ptr.time;
                    if (ptr.running && ptr.run != null)
                        ptr.run();
                    if (ptr.once || ptr.run == null) ptr.running = false;
                }
                pre = ptr;
            }
            ptr = ptr.next;
        }
    }
}