using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapBreakable : MonoBehaviour {

    private Tilemap breakableTileMap;
    private void Start() {
        breakableTileMap = GetComponent<Tilemap>();
        foreach (Vector3Int deletePos in SaveManager.Instance.selectedScene.BrokenCellPos) {
            breakableTileMap.SetTile(deletePos, null);
        }
    }
}
