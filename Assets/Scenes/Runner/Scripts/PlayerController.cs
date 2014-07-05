using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  public enum Direction {Up, Down, Left, Right};

  public float runSpeed;
  public float jumpForce;
  public float laneWidth;
  public float jukeSpeed;
  public Vector3 gravity;

  public float DragThreshold;

  public Vector3 nextMovement;

  private CharacterController controller;
  private bool dragging;
  private bool dragComplete;
  private Vector3 initialTouch;
  private Vector3 velocity;
  private Vector3 jukeToPos;

  void Start() {
    controller = GetComponent<CharacterController>();
    dragging = false;
    dragComplete = false;
    initialTouch = Vector3.zero;
    velocity = Vector3.zero;
  }

  void Update() {
    // dealWithTouchInput()
    dealWithKeyboardInput();
    applyPhysics();
    doMovement();
    postMovementCleanup();
  }

  private void applyPhysics() {
    // gravity
    velocity += gravity * Time.deltaTime;

    // constant movement
    Vector3 allButForward = ((transform.forward - Vector3.one) * -1);
    velocity = Vector3.Scale(velocity, allButForward);
    velocity += transform.forward * runSpeed;

    // juke movement
    if (jukeToPos != Vector3.zero) {
      if (crossedJukePos()) {
        transform.position = Vector3.Scale(transform.position, (transform.right * -1) + Vector3.one ) + Vector3.Scale(transform.position, transform.right);
        ResetJuking();
        jukeToPos = Vector3.zero;
      } else if (jukeCollision()) {
        // TODO: change jukeToPos and velocity to have us move back to the origin of the juke
      }
    }
  }

  public void ResetJuking() {
    // Left/Right velocity needs to be stopped. Constant forward velocity takes care of forward. Only thing we need to worry about is gravity.
    velocity = Vector3.Scale(velocity, transform.up);
  }

  private void doMovement() {
    controller.Move(velocity);
  }

  private void postMovementCleanup() {
    if (controller.isGrounded) {
      Vector3 allButUp = (transform.up - Vector3.one) * -1;
      velocity = Vector3.Scale(velocity, allButUp);
    }
  }

  private bool crossedJukePos() {
    Vector3 before = Vector3.Scale((transform.position - jukeToPos), transform.right);
    Vector3 after = Vector3.Scale(((transform.position + velocity) - jukeToPos), transform.right);
    // if the rightness before and after is on opposite sides of 0, that means that next move takes us over the line
    bool result = (before.x + before.y + before.z) * (after.x + after.y + after.z) <= 0;
    return result;
  }

  private bool jukeCollision() {
    // TODO: return true if the next position would cause a juke left/right collision
    return false;
  }

 private void jump() {
    if (controller.isGrounded) {
      velocity += transform.up * jumpForce;
    }
  }

  private void juke(int direction) {
    jukeToPos = transform.position + transform.right * direction * laneWidth;
    velocity += transform.right * direction * jukeSpeed;
    // controller.Move(jukeToPos);
  }

  private void slide() {
    Debug.Log("Sliding");
  }

  private Direction dragDirection(Vector3 initial, Vector3 terminal) {
    Vector3 vect = terminal - initial; 
    if (System.Math.Abs(vect.x) > System.Math.Abs(vect.y)) { 
      if (vect.x > 0) { return Direction.Right; } 
      else { return Direction.Left; } 
    } else { 
      if (vect.y > 0) { return Direction.Up; } 
      else { return Direction.Down; } 
    } 
  }

  private void dealWithKeyboardInput() {
    if (Input.GetKeyUp(KeyCode.UpArrow)) {
      jump();
    }
    if (Input.GetKeyUp(KeyCode.DownArrow)) {
      slide();
    }
    if (Input.GetKeyUp(KeyCode.LeftArrow)) {
      juke(-1);
    }
    if (Input.GetKeyUp(KeyCode.RightArrow)) {
      juke(1);
    }
    if (Input.GetKeyUp(KeyCode.Space)) {
      Debug.Log("transform.forward: " + transform.forward);
    }
  }

  private void dealWithTouchInput() {
    // if we decide that they've flicked up/down/etc...

    if (Input.touchCount > 0) {
      Touch touch = Input.touches[0];
      if (!dragging) {
        initialTouch = touch.position;
        dragging = true;
        dragComplete = false;
      }
      if (Vector3.Distance(initialTouch, touch.position) > DragThreshold && !dragComplete) {
        Vector3 newPos;
        switch(dragDirection(initialTouch, touch.position)) {
          case Direction.Up:
            jump();
            break;
          case Direction.Down:
            slide();
            break;
          case Direction.Left:
            juke(-1);
            break;
          case Direction.Right:
            juke(1);
            break;
        }
        dragComplete = true;
      }
      if (touch.phase == TouchPhase.Ended) {
        dragging = false;
      }
    }
  }
}
