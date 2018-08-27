using UnityEngine;
using UnityEditor;
using UMA;

[CustomEditor(typeof(RectangularSource))]
public class RectangularSource_Editor : Editor {

   private RectangularSource script;
   private bool foldout_UMA;
   private bool foldout_SourceParam;
   private bool foldout_Target;

   void OnEnable() {
      script = (RectangularSource) target;
   }

   public override void OnInspectorGUI() {

      script.selection= (GameObject)EditorGUILayout.ObjectField("Selection", script.selection, typeof(GameObject), true);

        Vector3 temp = script.transform.localScale;
      if(script.transform.localScale.x < 0) {
         temp.x = 0;
      }
      if(script.transform.localScale.y != 0) {
         temp.y = 0;
      }
      if(script.transform.localScale.z < 0) {
         temp.z = 0;
      }
      script.transform.localScale = temp;
      
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


      GUILayout.BeginHorizontal();
      GUILayout.Space(10);
      GUILayout.BeginVertical();
      foldout_SourceParam = EditorGUILayout.Foldout(foldout_SourceParam, "Generator Parameter");
      if(foldout_SourceParam) {
         
         script.minTime = EditorGUILayout.FloatField("Min Time", script.minTime);
         if(script.minTime < 0.0f) {
            script.minTime = 0.0f;
         }
         if(script.minTime > script.maxTime) {
            script.minTime = script.maxTime;
         }
         script.maxTime = EditorGUILayout.FloatField("Max Time", script.maxTime);
         if(script.minTime > script.maxTime) {
            script.maxTime = script.minTime;
         }

         script.var = EditorGUILayout.FloatField("Var: ", script.var);
         if(script.var < 0.0001f) {
            script.var = 0.0001f;
         }

         // Poisson distribution
         float coef = Mathf.Pow(2.718281828f, -script.var);
         EditorGUILayout.BeginVertical("box");
         ProgressBar(string.Format("p1: {0:F03}%", coef * Mathf.Pow(script.var, 1) / (1 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 1) / (1 * (1 - coef)));
         ProgressBar(string.Format("p2: {0:F03}%", coef * Mathf.Pow(script.var, 2) / (2 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 2) / (2 * (1 - coef)));
         ProgressBar(string.Format("p3: {0:F03}%", coef * Mathf.Pow(script.var, 3) / (6 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 3) / (6 * (1 - coef)));
         ProgressBar(string.Format("p4: {0:F03}%", coef * Mathf.Pow(script.var, 4) / (24 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 4) / (24 * (1 - coef)));
         ProgressBar(string.Format("p5: {0:F03}%", coef * Mathf.Pow(script.var, 5) / (120 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 5) / (120 * (1 - coef)));
         ProgressBar(string.Format("p6: {0:F03}%", coef * Mathf.Pow(script.var, 6) / (720 * (1 - coef)) * 100), coef * Mathf.Pow(script.var, 6) / (720 * (1 - coef)));
         EditorGUILayout.EndVertical();
         
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Space(10);
      GUILayout.BeginVertical();
      foldout_Target = EditorGUILayout.Foldout(foldout_Target, "Targets");
      if(foldout_Target) {
         if(GUILayout.Button("Add Target")) {
            script.targetList.Add(new TargetType());
         }

         GUIStyle XButtonStyle = new GUIStyle(GUI.skin.button);
         XButtonStyle.normal.textColor = Color.red;
         XButtonStyle.fixedWidth = 25;

         GUILayout.BeginHorizontal("box");
         GUILayout.Space(20);
         GUILayout.BeginVertical();
         for(int i = 0; i < script.targetList.Count; i++) {
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            script.targetList[i].target = (Transform) EditorGUILayout.ObjectField(script.targetList[i].target, typeof(Transform), true);
            if(GUILayout.Button("X", XButtonStyle)) {
               script.targetList.RemoveAt(i);
               return;
            }
            GUILayout.EndHorizontal();

            script.targetList[i].probability = EditorGUILayout.FloatField("Probability", script.targetList[i].probability);
            if(script.targetList[i].probability < 0) {
               script.targetList[i].probability = 0;
            }else if(script.targetList[i].probability > 1) {
               script.targetList[i].probability = 1;
            }

            script.targetList[i].destroyAtDestination = EditorGUILayout.Toggle("Destroy At Destination", script.targetList[i].destroyAtDestination);

            GUILayout.EndVertical();
         }

      
         if(GUILayout.Button("Normalize probability")) {
            float sum = 0;
            while(sum != 1.0f) {
               sum = 0;
               for(int i = 0; i < script.targetList.Count; ++i) {
                  sum += script.targetList[i].probability;
               }
               if(sum != 1.0f) {
                  sum = (1.0f - sum) / script.targetList.Count;
                  for(int i = 0; i < script.targetList.Count; ++i) {
                     if(script.targetList[i].probability + sum < 0) {
                        script.targetList[i].probability = 0.0f;
                     } else if(script.targetList[i].probability + sum > 1) {
                        script.targetList[i].probability = 1.0f;
                     } else {
                        script.targetList[i].probability += sum;
                     }
                  }
               }
            }
         }

         GUILayout.EndVertical();
         GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
   }

   // Custom GUILayout progress bar.
   void ProgressBar(string label, float value) {
      // Get a rect for the progress bar using the same margins as a textfield:
      Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
      EditorGUI.ProgressBar(rect, value, label);
      EditorGUILayout.Space();
   }
}
