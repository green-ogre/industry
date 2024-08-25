using UnityEngine;

public static class AxisNormalize
{
    public static float Movement(float movement)
    {
        if (movement < -0.2)
        {
            return -1;
        }
        else if (movement > 0.2)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
