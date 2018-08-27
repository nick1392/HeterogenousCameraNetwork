using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {
   public int GroupID;

   // Use this for initialization
   void Start() {
      GroupID = IDGenerator.getNewGroupID();
      gameObject.name = gameObject.name + string.Format("_{0:D06}", GroupID);
   }
}