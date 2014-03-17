using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public GameObject Player;
  public float Ground;
  private PlayerController playerController;

  void Start() {
    playerController = (PlayerController) Player.GetComponent("PlayerController");
  }

  // Update is called once per frame
  void Update () {
    Vector3 newPos = new Vector3();
    newPos.x = 0.0f;
    newPos.y = Ground + 3.0f;
    newPos.z = Player.transform.position.z - 4;
    transform.position = newPos;
  }
}
