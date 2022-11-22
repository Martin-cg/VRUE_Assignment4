using UnityEngine;
using UnityEngine.Events;

public class Lane : MonoBehaviour {
    public LaneManager Manager;
    public GameObject Spawn;
    public int LaneNumber;
    public ReadyButton ReadyButton;
    public GoalTriggerManager GoalTriggerManager;
    public GameObject RaceBarrier;
}
