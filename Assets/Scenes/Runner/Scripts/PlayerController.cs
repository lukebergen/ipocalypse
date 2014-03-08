using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // the y value at which falling stops (changes if they jump through window or some some)
  public float Ground;

  public float RunSpeed;
  public float jumpVelocity;
  public float gravity;
  public float terminalVelocity;
  public float dragThreshold;
  public float LaneDist;

  private Vector2 initialTouch;
  private bool dragging = false;
  private bool dragComplete = true;
  private float yVelocity = 0.0f;

  private enum direction {Up, Down, Left, Right};

  // Update is called once per frame
  void Update () {
    dealWithInput();

    float newZ = transform.position.z + (RunSpeed * Time.deltaTime);
    float newY = transform.position.y + yVelocity;

    if (newY < 0) {
      newY = 0;
      yVelocity = 0.0f;
    }
    if (yVelocity < terminalVelocity) {
      yVelocity = terminalVelocity;
    } else {
      yVelocity -= gravity;
    }

    transform.position = new Vector3(transform.position.x, newY, newZ);
  }

  private void dealWithInput() {
    // if we decide that they've flicked up/down/etc...

    if (Input.touchCount > 0) {
      Touch touch = Input.touches[0];
      if (!dragging && !dragComplete) {
        initialTouch = touch.position;
        dragging = true;
        dragComplete = false;
      }
      if (Vector3.Distance(initialTouch, touch.position) > dragThreshold && !dragComplete) {
        Vector3 newPos;
        switch(dragDirection(initialTouch, touch.position)) {
          case direction.Up:
            // jumping
            yVelocity = jumpVelocity;
            break;
          case direction.Down:
            break;
          case direction.Left:
            newPos = transform.position;
            newPos.x -= LaneDist;
            transform.position = newPos;
            break;
          case direction.Right:
            newPos = transform.position;
            newPos.x += LaneDist;
            transform.position = newPos;
            break;
        }
        dragComplete = true;
      }
    }
  }

  private direction dragDirection(Vector3 initial, Vector3 terminal) {
    Vector3 vect = terminal - initial;
    if (System.Math.Abs(vect.x) > System.Math.Abs(vect.y)) {
      if (vect.x > 0) { return direction.Right; }
      else { return direction.Left; }
    } else {
      if (vect.y > 0) { return direction.Up; }
      else { return direction.Down; }
    }
  }
}
