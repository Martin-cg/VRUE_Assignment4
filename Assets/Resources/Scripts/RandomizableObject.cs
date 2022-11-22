using UnityEngine;

public abstract class RandomizableObject : MonoBehaviour {
    public void Randomize() => Randomize(Utils.RandomSeed());
    public void Randomize(int seed) => Randomize(new System.Random(seed));
    public abstract void Randomize(System.Random random);
}
