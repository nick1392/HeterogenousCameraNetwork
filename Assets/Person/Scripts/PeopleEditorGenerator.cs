using UnityEngine;
using System.Collections;
using UMA;

public class PeopleEditorGenerator : MonoBehaviour {

   public UMAGeneratorBase generator;
   public SlotLibrary slotLibrarry;
   public OverlayLibrary overlayLibrary;
   public RaceLibrary raceLibrary;
   public RuntimeAnimatorController animController;

   private int numberOfSlots = 10;
   private UMADynamicAvatar umaDynamicAvatar;
   private UMAData umaData;
   private UMADnaHumanoid umaDna;
   private UMADnaTutorial umaTutorialDNA;


    //////////////////////////////////////////////////UI!!!!!!1

    private bool active = true;

    //////////////////////////////////////////////////UI!!!!!!1


    public Vector3 point;

   public Transform destination;
   public bool destroyAtDestination;

   public void GenerateUMA(GameObject Person) {
      // add UMA components to the game object
      umaDynamicAvatar = Person.AddComponent<UMADynamicAvatar>();

      // Initialize Avatar and grab a reference to it's data component
      umaDynamicAvatar.Initialize();
      umaData = umaDynamicAvatar.umaData;

      // Attach our generator
      umaDynamicAvatar.umaGenerator = generator;
      umaData.umaGenerator = generator;

      // Set up slot Array
      umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];

      // Set up our Morph references
      umaDna = new UMADnaHumanoid();
      umaTutorialDNA = new UMADnaTutorial();
      umaData.umaRecipe.AddDna(umaDna);
      umaData.umaRecipe.AddDna(umaTutorialDNA);

      CreateMale();

      umaDynamicAvatar.animationController = animController;

      // Generate our UMA
      umaDynamicAvatar.UpdateNewRace();

   }

   void CreateMale() {
      UMAData.UMARecipe umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
      umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

      SetSlot(0, "MaleEyes");
      AddOverlay(0, "EyeOverlay");

      SetSlot(1, "MaleInnerMouth");
      AddOverlay(1, "InnerMouth");

      SetSlot(2, "MaleFace");
      AddOverlay(2, "MaleHead02");
      AddOverlay(2, "MaleEyebrow01", Color.black);

      SetSlot(3, "MaleTorso");
      AddOverlay(3, "MaleBody02");
      AddOverlay(3, "MaleUnderwear01", Color.red);

      SetSlot(4, "MaleHands");
      LinkOverlay(4, 3);

      SetSlot(5, "MaleLegs");
      LinkOverlay(5, 3);

      SetSlot(6, "MaleFeet");
      LinkOverlay(6, 3);

   }

   ////////// Overlay Helpers //////////

   void AddOverlay(int slot, string overlayName) {
      umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));
   }

   void AddOverlay(int slot, string overlayName, Color color) {
      umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName, color));
   }

   void LinkOverlay(int slotNumber, int slotToLink) {
      umaData.umaRecipe.slotDataList[slotNumber].SetOverlayList(umaData.umaRecipe.slotDataList[slotToLink].GetOverlayList());
   }

   void RemoveOverlay(int slotNumber, string overlayName) {
      umaData.umaRecipe.slotDataList[slotNumber].RemoveOverlay(overlayName);
   }

   void ColorOverlay(int slotNumber, string overlayName, Color color) {
      umaData.umaRecipe.slotDataList[slotNumber].SetOverlayColor(color, overlayName);
   }

   ////////// Overlay Helpers //////////

   void SetSlot(int slotNumber, string SlotName) {
      umaData.umaRecipe.slotDataList[slotNumber] = slotLibrarry.InstantiateSlot(SlotName);
   }

   void RemoveSlot(int slotNumber) {
      umaData.umaRecipe.slotDataList[slotNumber] = null;
   }

   ////////// DNA Helpers //////////

   void SetBodyMass(float mass) {
      umaDna.upperMuscle = mass;
      umaDna.upperWeight = mass;
      umaDna.lowerMuscle = mass;
      umaDna.lowerWeight = mass;
      umaDna.armWidth = mass;
      umaDna.forearmWidth = mass;
   }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Person[] pr = GetComponentsInChildren<Person>();
            if (active)
            {
                foreach (Person comp in pr)
                {
                    comp.enabled = false;
                }
                active = false;
            }
            else
            {
                foreach (Person comp in pr)
                {
                    comp.enabled = true;
                }
                active = true;
            }
            
        }
    }
}
