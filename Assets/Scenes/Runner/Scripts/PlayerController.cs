using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  public enum Direction {Up, Down, Left, Right};
  public enum State {Running, Juking, Jumping, Rolling, Stumbling};

  public float RunSpeed;
  public float JumpForce;
  public float DragThreshold;
  public float LaneDist;
  public int SlideLength;
  public float JukeSpeed;

  public int Lane;

  private bool sliding;
  public bool juking;
  private int jukeDirection;
  private int queuedJuke;
  private Vector3 jukeToPos;
  private int standOnFrame;
  private Vector2 initialTouch;
  private bool dragging = false;
  private bool dragComplete = false;

  void Start() {
    Lane = 0;
    sliding = false;
    queuedJuke = 0;
    // rigidbody.AddForce(new Vector3(0, 0, RunSpeed));
  }

  // Update is called once per frame
  void Update () {
    // dealWithTouchInput();
    dealWithKeyboardInput();
    applyConstantForce();
    checkStateChanges();
    // doMovement();
  }

  private void applyConstantForce() {
    Vector3 vel = rigidbody.velocity;
    if (System.Math.Abs(transform.forward.x) >= 0.001f) {
      vel.x = RunSpeed;
    }
    if (System.Math.Abs(transform.forward.y) >= 0.001f) {
      vel.y = RunSpeed;
    }
    if (System.Math.Abs(transform.forward.z) >= 0.001f) {
      vel.z = RunSpeed;
    }
    rigidbody.velocity = vel;
  }

  private void checkStateChanges() {
    if (sliding && standOnFrame <= Time.frameCount) {
      standUp();
    }
    if (juking) {
      RaycastHit hitInfo;
      if (rigidbody.SweepTest(transform.right * jukeDirection, out hitInfo, 0.2f)) {
        juke(jukeDirection * -1, true);
      }
      float incr = (jukeDirection * JukeSpeed) * Time.deltaTime;
      Vector3 nextPos = transform.position + (transform.right * incr);
      bool passed = false;
      if (System.Math.Abs(transform.right.x) >= 0.001f && ((jukeToPos.x - transform.position.x) * jukeDirection <= 0)) { passed = true; }
      if (System.Math.Abs(transform.right.y) >= 0.001f && ((jukeToPos.y - transform.position.y) * jukeDirection <= 0)) { passed = true; }
      if (System.Math.Abs(transform.right.z) >= 0.001f && ((jukeToPos.z - transform.position.z) * jukeDirection <= 0)) { passed = true; }
      if ( passed ) {
        juking = false;
        nextPos = transform.position;
        Debug.Log("passed. transform.right: " + transform.right);
        if (System.Math.Abs(transform.right.x) >= 0.001f) { nextPos.x = jukeToPos.x; }
        if (System.Math.Abs(transform.right.y) >= 0.001f) { nextPos.y = jukeToPos.y; }
        if (System.Math.Abs(transform.right.z) >= 0.001f) { nextPos.z = jukeToPos.z; }
        if (queuedJuke != 0) {
          juke(queuedJuke);
        }
        queuedJuke = 0;
      }
      transform.position = nextPos;
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

  private void jump() {
    if (grounded()) {
      Vector3 vel = rigidbody.velocity;
      vel.y = JumpForce;
      rigidbody.velocity = vel;
    }
  }

  private void juke(int direction) {
    juke(direction, false);
  }

  private void juke(int direction, bool force) {
    if (!juking || force) {
      Vector3 originalPos;
      if (juking) { originalPos = jukeToPos; }
      else { originalPos = transform.position; }
      Vector3 newPos = originalPos + (transform.right * direction * LaneDist);
      jukeToPos = newPos;
      juking = true;
      jukeDirection = direction;
    } else {
      queuedJuke = direction;
    }
  }

  private void slide() {
    sliding = true;
    Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
    rigidbody.MovePosition(newPos);
    rigidbody.MoveRotation(Quaternion.Euler(new Vector3(-90, 0, 0)));

    standOnFrame = Time.frameCount + SlideLength;
  }

  private void standUp() {
    sliding = false;
    rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 0, 0)));
  }

  private bool grounded() {
    Vector3 origin = transform.position;
    origin.y += 0.1f;
    return (Physics.Raycast(origin, Vector3.down, 0.2f));
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
}
