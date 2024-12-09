using UnityEngine;

public class MinotaurAnimationEvents : MonoBehaviour {

    [SerializeField] private Minotaur minotaur;


    public void Dash() {
        minotaur.Dash();
    }
}
