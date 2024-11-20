using UnityEngine;

public class SavePoint : BaseInteractable {


    private const string SCENE_NAME = "CaveLevel1";

    


    protected override void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        if (hasPlayer) {
            SaveManager.Instance.Save(transform.position, SCENE_NAME);
        }
    }

    


}
