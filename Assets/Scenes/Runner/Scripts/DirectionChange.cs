using UnityEngine;
using System.Collections;

public class DirectionChange : MonoBehaviour {

  public Vector3 PlayerPosition;
  public Vector3 PlayerRotation;

  public void OnTriggerEnter(Collider other) {
    if (other.gameObject.name == "Player") {
      GameObject player = other.gameObject;
      handlePositionChange(player);
      handleRotationChange(player);
      resetPlayerState(player);
    }
  }

  private void handlePositionChange(GameObject player) {
    player.transform.position = PlayerPosition;
  }

  private void handleRotationChange(GameObject player) {
    player.transform.eulerAngles = PlayerRotation;
  }

  private void resetPlayerState(GameObject player) {
    // TODO: possibly unset juking, jukeDirection, stumbling, etc...?
    // player.GetComponent<PlayerController>().juking = false;
    // RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation;
    // Vector3 right = player.transform.right;
    // if (System.Math.Abs(right.x) >= 0.0001f) { constraints = constraints | RigidbodyConstraints.FreezePositionX; }
    // if (System.Math.Abs(right.y) >= 0.0001f) { constraints = constraints | RigidbodyConstraints.FreezePositionY; }
    // if (System.Math.Abs(right.z) >= 0.0001f) { constraints = constraints | RigidbodyConstraints.FreezePositionZ; }
    // Rigidbody rb = player.GetComponent<Rigidbody>();
    // rb.constraints = constraints;
  }
}
