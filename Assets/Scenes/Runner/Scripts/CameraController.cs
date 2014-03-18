using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public GameObject Player;
  public Vector3 Rail;
  private PlayerController playerController;

  void Start() {
    playerController = (PlayerController) Player.GetComponent("PlayerController");
  }

  // Update is called once per frame
  void Update () {
    Vector3 newPos = Rail;
    newPos.z = Player.transform.position.z - 4;
    transform.position = newPos;
  }
}
