using UnityEngine;
using System.Collections;

public class ExplorationController : MonoBehaviour {

  private int strafeStartFingerId = -1;
  public Vector2 strafeStart;

  private int rotateStartFingerId = -1;
  private Vector2 rotateStart;

  private float xRotSensitivity = 0.003f;
  private float yRotSensitivity = 0.001f;

  private float walkSpeed = 1.0f;

  private int yMinLimit = -80;
  private int yMaxLimit = 80;

  void Update () {
    handleInput();
  }

  private void handleInput() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Application.Quit();
    }

    for (int i = 0 ; i < Input.touchCount ; i++) {
      Touch touch = Input.touches[i];

      considerControlTouch(touch);

      if (touch.fingerId == strafeStartFingerId) {
        handleStrafe(touch);
      } else if (touch.fingerId == rotateStartFingerId) {
        handleRotate(touch);
      } else {
        handleWorldInteraction(touch);
      }
    }
  }

  private void considerControlTouch(Touch touch) {
    if (touch.phase == TouchPhase.Began) {
      if (touch.position.y < Screen.height * 0.25) {
        if (touch.position.x < (Screen.width * 0.5)) {
          strafeStartFingerId = touch.fingerId;
          strafeStart = touch.position;
        } else {
          rotateStartFingerId = touch.fingerId;
          rotateStart = touch.position;
        }
      }
    }

    if (touch.phase == TouchPhase.Ended) {
      if (touch.fingerId == strafeStartFingerId) {
        strafeStartFingerId = -1;
      }
      else if (touch.fingerId == rotateStartFingerId) {
        rotateStartFingerId = -1;
      }
    }
  }

  private void handleWorldInteraction(Touch touch) {

  }

  private void handleStrafe(Touch touch) {
    Vector2 delta = strafeStart - touch.position;
    if (delta.x < -10) {
      transform.root.Translate(Vector3.right * (Time.deltaTime * walkSpeed));
      Debug.Log("Moving right");
      // move right
    }
    if (delta.y < -10) {
      transform.root.Translate(Vector3.forward * (Time.deltaTime * walkSpeed));
      Debug.Log("Moving forward");
      // move forward
    }
    if (delta.x > 10) {
      transform.root.Translate(Vector3.left * (Time.deltaTime * walkSpeed));
      Debug.Log("Moving left");
    }
    if (delta.y > 10) {
      transform.root.Translate(Vector3.back * (Time.deltaTime * walkSpeed));
      Debug.Log("Moving Back");
    }
  }

  private void handleRotate(Touch touch) {
    // Vector2 delta1 = touch.deltaPosition;

    Vector2 delta = rotateStart - touch.position;

    Vector3 current = transform.rotation.eulerAngles;
    Vector3 rootCurrent = transform.root.rotation.eulerAngles;

    float rotX = current.x + (((delta.y * yRotSensitivity) * System.Math.Abs(delta.y)));
    float rotY = current.y - (((delta.x * xRotSensitivity) * System.Math.Abs(delta.x)));

    // rotate the camera up and down
    transform.eulerAngles = new Vector3(rotX, current.y, 0);

    // but rotate the player left and right
    transform.root.eulerAngles = new Vector3(rootCurrent.x, rotY, 0);
    // transform.root.eulerAngles = transform.root.rotation.eulerAngles;
  }

  private float clampRot(float angle, float min, float max) {
    if (angle < -360) {
      angle += 360;
    }
    if (angle > 360) {
      angle -= 360;
    }
    return Mathf.Clamp(angle, min, max);
  }
}
