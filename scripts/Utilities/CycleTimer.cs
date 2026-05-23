using System;

namespace Reactistry.scripts.Utilities;

public class CycleTimer(float duration)
{
    private readonly float duration = duration;
    private float elapsed;

    public float Progress => Math.Min(elapsed, duration) / duration;

    public bool Completed => elapsed >= duration;

    public void Advance(float delta)
    {
        if (elapsed < duration)
        {
            elapsed += delta;
        }
    }

    public bool TryTrigger()
        => TryTrigger(() => true);

    public bool TryTrigger(Func<bool> predicate)
    {
        if (Completed && predicate())
        {
            Reset();
            return true;
        }

        return false;
    }

    public void Reset()
    {
        elapsed -= duration;
    }
}
