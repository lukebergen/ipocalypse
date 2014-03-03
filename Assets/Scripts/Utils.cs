using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

  public static GameObject TouchToObject(Touch touch) {
    GameObject obj = null;
    if (touch.position != null) {
      GameObject[] touchColliders = GameObject.FindGameObjectsWithTag("TouchInputCollider");
      Ray hitRay = Camera.main.ScreenPointToRay(touch.position);
      RaycastHit hitInfo;
      for (int i = 0 ; i < touchColliders.Length ; i++) {
        Collider col = (Collider) touchColliders[i].collider;
        if (col.Raycast(hitRay, out hitInfo, 200)) {
          obj = col.gameObject.transform.root.gameObject;
        }
      }
    }
    return obj;
  }

}
