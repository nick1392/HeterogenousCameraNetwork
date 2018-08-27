using UnityEngine;
using System.Collections;
using UnityEditor;
using UMA;

[CustomEditor(typeof(PeopleEditorGenerator))]
public class PeopleEditorGenerator_Editor : Editor {

   private PeopleEditorGenerator script;

   private bool foldout_UMA;

   void OnEnable() {
      script = (PeopleEditorGenerator) target;
   }


   public override void OnInspectorGUI() {

      GUILayout.BeginHorizontal();
      GUILayout.Space(10);
      GUILayout.BeginVertical();

      foldout_UMA = EditorGUILayout.Foldout(foldout_UMA, "UMA");
      if(foldout_UMA) {
         script.generator = (UMAGenerator) EditorGUILayout.ObjectField("Generator", script.generator, typeof(UMAGenerator), true);
         script.slotLibrarry = (SlotLibrary) EditorGUILayout.ObjectField("Slot Library", script.slotLibrarry, typeof(SlotLibrary), true);
         script.overlayLibrary = (OverlayLibrary) EditorGUILayout.ObjectField("Overlay Library", script.overlayLibrary, typeof(OverlayLibrary), true);
         script.raceLibrary = (RaceLibrary) EditorGUILayout.ObjectField("Race Library", script.raceLibrary, typeof(RaceLibrary), true);
         script.animController = (RuntimeAnimatorController) EditorGUILayout.ObjectField("Animation Controller", script.animController, typeof(RuntimeAnimatorController), false);
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();



      script.point = EditorGUILayout.Vector3Field("point", script.point);
      if(GUILayout.Button("Spawn")) {
         GameObject Person = new GameObject("Person");
         Person.transform.parent = GameObject.Find("People").transform;
         Person.transform.position = script.point;
         Person.AddComponent<Person>();
         Person Personscript = Person.GetComponent<Person>();
         Personscript.target = script.destination;
         Personscript.destroyAtDestinantion = script.destroyAtDestination;
         script.GenerateUMA(Person);
      }
      
      DrawDefaultInspector();
   }
   
}
