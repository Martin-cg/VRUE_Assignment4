
using UnityEngine;
using UnityEngine.Events;

public class GoalTriggerManager : MonoBehaviour {

    public UnityEvent<GameObject> OnGoalCrossed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("GTM: " + other);
        OnGoalCrossed.Invoke(other.gameObject.transform.parent.gameObject);
    }
}