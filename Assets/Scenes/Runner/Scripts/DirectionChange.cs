using UnityEngine;
using System.Collections;

public class DirectionChange : MonoBehaviour {

  public float Ground;
  public Vector3 PlayerRotation;
  public int LaneChange;

  private GameObject player;

  public void OnTriggerEnter(Collider other) {
    if (other.gameObject.name == "Player") {
      player = other.gameObject;
      handleLaneChange();
      handleRotationChange();
    }
  }

  private void handleLaneChange() {
    PlayerController pc = player.GetComponent<PlayerController>();
    int newLane = pc.Lane + LaneChange;
    pc.Lane = newLane;

    CameraController camCtrl = GameObject.Find("Main Camera").GetComponent<CameraController>();
    camCtrl.Rail = new Vector3(player.transform.position.x - newLane, Ground + 3.0f, 0);
  }

  private void handleRotationChange() {
    // TODO: currently busted
    // Rigidbody rb = player.GetComponent<Rigidbody>();
    // rb.MoveRotation(Quaternion.Euler(PlayerRotation));
  }
}
