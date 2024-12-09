using UnityEngine;

public class TempDoor : Door {

    protected override void Update() {
        base.Update();
        if (Player.Instance.minotaurKilled) {
            isLocked = false;
        }
    }
}
