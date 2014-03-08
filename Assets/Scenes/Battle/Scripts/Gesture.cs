using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gesture {

  public const int Hold       = 0;
  public const int Tap        = 1;
  public const int Scratch    = 2;
  public const int Flick      = 3;
  public const int ObjectDrag = 4;
  public const int Circle     = 5;

  private List<Touch> touches;
  private int type;

  private float startFrameTime;

  private bool resolved;
  public bool Resolved {
    get {return resolved;}
  }

  public Gesture() {
    // sane default assumption
    type = Gesture.Hold;
    touches = new List<Touch>();
    resolved = false;
    startFrameTime = Time.time;
  }

  public void AddTouch(Touch touch) {
    // update the gesture based on this new touch
    touches.Add(touch);
    updateType();
    resolve();
  }

  public int Type() {
    return type;
  }

  private void updateType() {
    int oldType = this.type;
    // based on touches, decide what our type is
    // probably make use of current value of type
    // to help figure out if we have changed since then
    int halfScr = Screen.width / 2;
    if ((touches[0].position.x < halfScr && touches[touches.Count - 1].position.x > halfScr) ||
      (touches[0].position.x > halfScr && touches[touches.Count - 1].position.x < halfScr )) {
      this.type = Gesture.Tap;
    }
    if (this.type != oldType) {
      Debug.Log("Changing Type [fingerId, oldType, newType]: [" + touches[0].fingerId + ", " + oldType + ", " + this.type + "]");
    }
  }

  private void resolve() {
    // consider if this gesture is resolved/complete
    // if so, trigger any effects and mark this.resolved as false
  }
}
