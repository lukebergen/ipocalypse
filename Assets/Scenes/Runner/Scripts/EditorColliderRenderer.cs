using UnityEngine;
using System.Collections;

public class EditorColliderRenderer : MonoBehaviour {

  void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(transform.position, transform.lossyScale);
  }
}
