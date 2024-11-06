using UnityEngine;

public class SavePoint : BaseInteractable {


    private const string SCENE_NAME = "CaveLevel1";

    private bool hasPlayer = false;


    protected override void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        if (hasPlayer) {
            Debug.Log("WORKS FROM SAVE POINT");
            SaveManager.Instance.Save(transform.position, SCENE_NAME);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("has triggered");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Debug.Log("has player");
            hasPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            hasPlayer = false;
        }
    }
}
