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
  public LayerMask BoundaryLayerMask;

  public int Lane;

  private bool sliding;

  // juke vars
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
    snapToGrid();
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
    checkStandUp();
    checkOnJuke();
  }

  private void checkStandUp() {
    if (sliding && standOnFrame <= Time.frameCount) {
      standUp();
    }
  }

  private void checkOnJuke() {
    if (juking) {
      RaycastHit info;
      if (Physics.Raycast(transform.position + Vector3.up + (transform.right * jukeDirection * 0.1f), transform.right * jukeDirection, out info, 0.1f, BoundaryLayerMask)) {
        Debug.Log("Hit: " + info.collider.gameObject.name);
        juking = false;
        juke(jukeDirection * -1, jukeToPos);
      }

      bool madeIt = (
        ((transform.right.x >  0.5f) && ((transform.position.x - jukeToPos.x) * jukeDirection) > 0) ||
        ((transform.right.x < -0.5f) && ((transform.position.x - jukeToPos.x) * jukeDirection) < 0) ||
        ((transform.right.y >  0.5f) && ((transform.position.y - jukeToPos.y) * jukeDirection) > 0) ||
        ((transform.right.y < -0.5f) && ((transform.position.y - jukeToPos.y) * jukeDirection) < 0) ||
        ((transform.right.z >  0.5f) && ((transform.position.z - jukeToPos.z) * jukeDirection) > 0) ||
        ((transform.right.z < -0.5f) && ((transform.position.z - jukeToPos.z) * jukeDirection) < 0)
      );
      if (madeIt) {
        Debug.Log("No more juking because we've reached the target");
        juking = false;
        Vector3 vel = rigidbody.velocity;
        Vector3 pos = transform.position;
        if (System.Math.Abs(transform.right.x) > 0.5f) { vel.x = 0; pos.x = jukeToPos.x; }
        if (System.Math.Abs(transform.right.y) > 0.5f) { vel.y = 0; pos.y = jukeToPos.y; }
        if (System.Math.Abs(transform.right.z) > 0.5f) { vel.z = 0; pos.z = jukeToPos.z; }
        rigidbody.velocity = vel;
        transform.position = pos;
      }
    }
  }

  private void snapToGrid() {
    if (!juking) {
      // TODO: x to Math.Round
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
    Debug.Log("velocity: " + rigidbody.velocity);
    if (grounded()) {
      Vector3 vel = rigidbody.velocity;
      vel.y = JumpForce;
      rigidbody.velocity = vel;
    }
  }

  private void juke(int direction) {
    juke(direction, transform.position);
  }

  private void juke(int direction, Vector3 fromPos) {
    if (!juking) {
      juking = true;
      jukeDirection = direction;
      jukeToPos = fromPos + transform.right * direction * LaneDist;
      Vector3 vel = rigidbody.velocity;
      if (System.Math.Abs(transform.right.x) > 0.5f) {vel.x = direction * JukeSpeed;}
      if (System.Math.Abs(transform.right.y) > 0.5f) {vel.y = direction * JukeSpeed;}
      if (System.Math.Abs(transform.right.z) > 0.5f) {vel.z = direction * JukeSpeed;}
      Debug.Log("Setting velocity to: " + vel);
      rigidbody.velocity = vel;
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
