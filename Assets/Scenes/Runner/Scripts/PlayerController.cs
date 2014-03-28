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
  private bool juking;
  private int jukeDirection;
  private Vector3 jukeToPos;
  private int standOnFrame;
  private Vector2 initialTouch;
  private bool dragging = false;
  private bool dragComplete = false;

  void Start() {
    Lane = 0;
    sliding = false;
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
    vel.z = RunSpeed;
    rigidbody.velocity = vel;
  }

  private void checkStateChanges() {
    if (sliding && standOnFrame <= Time.frameCount) {
      standUp();
    }
    if (juking) {
      RaycastHit hitInfo;
      if (rigidbody.SweepTest(new Vector3(jukeDirection, 0, 0), out hitInfo, 0.1f)) {
        juke(jukeDirection * -1);
      }
      Vector3 nextPos = transform.position;
      nextPos.x += (jukeDirection * JukeSpeed) * Time.deltaTime;
      if ( (jukeToPos.x - transform.position.x) * jukeDirection <= 0 ) {
        juking = false;
        nextPos.x = jukeToPos.x;
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
    float xComponent;
    if (juking) {
      xComponent = jukeToPos.x + (LaneDist * direction);
    } else {
      xComponent = transform.position.x + (LaneDist * direction);
    }
    Vector3 newPos = new Vector3(xComponent, transform.position.y, transform.position.z);
    jukeToPos = newPos;
    juking = true;
    jukeDirection = direction;
  }

  private void oldJuke(int direction) {
    if (Lane == direction) {
      Debug.Log("Bonk!");
    } else {
      float xComponent = transform.position.x + (LaneDist * direction);
      Vector3 newPos = new Vector3(xComponent, transform.position.y, transform.position.z);
      rigidbody.MovePosition(newPos);
      Lane += direction;
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
