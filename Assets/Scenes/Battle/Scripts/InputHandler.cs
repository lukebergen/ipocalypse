using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

  public Gesture[] Gestures = new Gesture[10];

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  // every fixed update, update existing gestures and such
  void FixedUpdate() {
    Gesture gest;
    Touch touch;
    for (int i = 0 ; i < Input.touchCount ; i++) {
      touch = Input.touches[i];
      if (touch.phase == TouchPhase.Began) {
        gest = new Gesture();
        Gestures[touch.fingerId] = gest;
      }
      gest = Gestures[touch.fingerId];
      if (gest != null) {
        gest.AddTouch(touch);
        if (gest.Resolved) {
          Gestures[touch.fingerId] = null;
        }
      }
    }
  }
}
