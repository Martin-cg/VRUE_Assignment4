using System;

public static class RandomExtensions {
    public static double NextDouble(this Random random, double min, double max) {
        return random.NextDouble() * (max - min) + min;
    }
    public static float NextFloat(this Random random) {
        return (float)random.NextDouble();
    }
    public static float NextFloat(this Random random, float min, float max) {
        return (float)random.NextDouble(min, max);
    }
}
