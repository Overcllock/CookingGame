using System.Collections.Generic;
using UnityEngine;

public static class Wait
{
    private static Dictionary<float, WaitForSeconds> _intervals;

    private static WaitForEndOfFrame _endOfFrame;
    private static WaitForFixedUpdate _fixedUpdate;

    public static WaitForEndOfFrame ForEndOfFrame
    {
        get
        {
            if (_endOfFrame == null)
            {
                _endOfFrame = new WaitForEndOfFrame();
            }

            return _endOfFrame;
        }
    }
    public static WaitForFixedUpdate ForFixedUpdate
    {
        get
        {
            if (_fixedUpdate == null)
            {
                _fixedUpdate = new WaitForFixedUpdate();
            }

            return _fixedUpdate;
        }
    }

    public static WaitForSeconds ForSeconds(float seconds)
    {
        if (_intervals == null)
        {
            _intervals = new Dictionary<float, WaitForSeconds>(10);
        }

        if (!_intervals.TryGetValue(seconds, out var interval))
        {
            interval = new WaitForSeconds(seconds);
            _intervals.Add(seconds, interval);
        }

        return interval;
    }
}