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
    // Vector3 newPos = Rail;
    // newPos.z = Player.transform.position.z - 1.5f;
    // newPos.y = Player.transform.position.y + 2.5f;
    // newPos.x = Player.transform.position.x;
    Vector3 newPos = Player.transform.position;
    newPos = newPos + (Player.transform.forward * -2.5f);
    newPos = newPos + (Player.transform.up * 2.0f);
    transform.position = newPos;

    transform.rotation = Player.transform.rotation;
  }
}
