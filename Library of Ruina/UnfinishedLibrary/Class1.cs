using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace UnfinishedLibrary
{

	[HarmonyPatch]
	public class InviteChange : ModInitializer
	{
		public const string GUID = "UnfinishedLibrary";
		public override void OnInitializeMod()
		{
			new Harmony(GUID).PatchAll();
		}

		//[HarmonyPatch(typeof(DropBookInventoryModel), "GetBookList_invitationBookList")]
		//[HarmonyPostfix]
		//public static List<LorId> AddInvitationBooks(List<LorId> result)
		//{
		//	result.Add(new LorId("UnfinishedLibrary", 1));

		//	//Debug.Log("Added ... book");

		//	return result;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(StageController), "InitStageByInvitation")]
		//public static void StageControllerPostfix(StageController __instance)
		//{
		//	AccessTools.FieldRef<StageController, List<LorId>> usedbooks = AccessTools.FieldRefAccess<List<LorId>>(typeof(StageController), "_usedBooks");
		//	usedbooks(__instance).Clear();
		//	Debug.Log("Used books: " + usedbooks(__instance).Count);
		//}

		//static StageClearInfoListModel clearinfo = null;

		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(LibraryModel), "LoadFromSaveData")]
		//public static void DropBookPostfix(GameSave.SaveData data)
		//{
		//	clearinfo.LoadFromSaveData(data.GetData("clearInfo"));
		//	Debug.Log("clearinfo null " + (clearinfo == null));
		//	// Make my own copy because the game doesnt like it when I reference its own copy
		//}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(DropBookInventoryModel), "LoadFromSaveData")]
		public static void DropBookPostfix(DropBookInventoryModel __instance)
		{
			__instance.AddBook(new LorId("UnfinishedLibrary", 1), 99);
			//Debug.Log("Add Books");
		}

		[HarmonyPatch(typeof(UI.UIInvitationRightMainPanel), "SetUpdatePanel")]
		[HarmonyPostfix]
		public static void Postfix(UI.UIInvitationRightMainPanel __instance)
		{

			List<DropBookXmlInfo> appliedbooks = __instance.GetAppliedBookModel();
			if (appliedbooks.Count == 0 || appliedbooks[0].Name != "...")
			{
				//Debug.Log("Not ... book");
				return;
			}

			//Debug.Log("Found ... book, Changing Invite"); 

			StageClassInfo lowerIconData = null;

			StageClearInfoListModel clearinfo = LibraryModel.Instance.ClearInfo;


			List<StageClassInfo> stages = Singleton<StageClassInfoList>.Instance.GetAllDataList();

			//Debug.Log("There are " + stages.Count);

			AccessTools.FieldRef<UI.UIInvitationRightMainPanel, UI.UIInvitationRightCustomPanel> invpanel = AccessTools.FieldRefAccess<UI.UIInvitationRightCustomPanel>(typeof(UI.UIInvitationRightMainPanel), "customInvPanel");


            for (int i = 0; i < stages.Count; i++)
            {
                if (clearinfo.GetClearCount(stages[i].id) > 0 || stages[i].stageName == "")
                {
                    stages.Remove(stages[i]);
                    i--;
                }
            }

            stages.Reverse();

			//Debug.Log("There are " + stages.Count);

			if (stages.Count == 0)
			{
				lowerIconData = Singleton<StageClassInfoList>.Instance.GetDataFromBooks(appliedbooks);
			}
			else if (stages.Count == 1)
			{
				lowerIconData = stages[0];
			}
			else
			{
				//Debug.Log("More than 1");
				lowerIconData = null;
				invpanel(__instance).Open();
				invpanel(__instance).SetData(stages);
				//Debug.Log("Finished setdata");
			}

			__instance.SetLowerIconData(lowerIconData);
			//Debug.Log("Finished");
		}

        //// Change slotted books on apply
        //      [HarmonyPatch(typeof(UI.UIInvitationRightCustomPanel), "OnClickInApplyButton")]
        //      [HarmonyPrefix]
        //      public static bool Prefix(UI.UIInvitationRightCustomPanel __instance)
        //      {
        //	AccessTools.FieldRef<UI.UIInvitationRightCustomPanel, StageClassInfo> info = AccessTools.FieldRefAccess<StageClassInfo>(typeof(UI.UIInvitationRightCustomPanel), "_currentSelectStageInfo");
        //	List<LorId> needbooks = info(__instance).invitationInfo.needsBooks;

        //	AccessTools.FieldRef<UI.UIInvitationRightCustomPanel, UI.UIInvitationRightMainPanel> rootpanel = AccessTools.FieldRefAccess<UI.UIInvitationRightMainPanel>(typeof(UI.UIInvitationRightCustomPanel), "rootpanel");

        //	for (int i = 0; i < rootpanel(__instance).invitationbookSlots.Count; i++ )
        //          {
        //		if (needbooks.Count == i) // end of needbooks index
        //              {
        //			rootpanel(__instance).invitationbookSlots[i].SetEmptySlot();
        //		}
        //		else 
        //              {
        //			rootpanel(__instance).invitationbookSlots[i].SetNeedSlot(needbooks[i]);
        //		}
        //          }
        //	return true;
        //      }

        [HarmonyPatch(typeof(UI.UIInvitationRightMainPanel), "GetBookRecipe")]
        [HarmonyPrefix]
        public static bool Prefix(UI.UIInvitationRightMainPanel __instance, ref StageClassInfo __result)
        {
            List<DropBookXmlInfo> appliedbooks = __instance.GetAppliedBookModel();
            if (appliedbooks.Count == 0 || appliedbooks[0].Name != "...")
            {
                //Debug.Log("Not ... book");
                return true;
            }

            else
            {
                //Debug.Log("GetBookRecipe hijack");
                AccessTools.FieldRef<UI.UIInvitationRightMainPanel, StageClassInfo> current = AccessTools.FieldRefAccess<StageClassInfo>(typeof(UI.UIInvitationRightMainPanel), "currentSelectedNormalstage");
                __result = current(__instance);
                return false;
            }
        }
	}
}
