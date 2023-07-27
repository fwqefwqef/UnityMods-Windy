using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using MEC;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Random = System.Random;
using System.Collections;

namespace BlackSmithCheat
{
	[BepInPlugin(GUID, "BlackSmithCheat", version)]
	[BepInProcess("IRA.exe")]
	public class BlackSmithCheat : BaseUnityPlugin
	{
		public const string GUID = "windy.blacksmith";
		public const string version = "1.0.0";

		private static readonly Harmony harmony = new Harmony(GUID);

		void Awake()
		{
			harmony.PatchAll();
		}
		void OnDestroy()
		{
			if (harmony != null)
				harmony.UnpatchAll(GUID);
		}

        [HarmonyPatch(typeof(BlacksmithInterfaceManager), "EquipmentUpgradeRequest")]
		class Upgrade
		{
			static bool Prefix(BlacksmithInterfaceManager __instance, IEquipment targetEquipment)
			{
				Debug.Log("Equip Upgrade");
				PlayerCharacter.instance.WeaponUpgradeMoney += 1000;
				__instance.text_ResultNotice_Upgrade.text = LocaleManager.GetUILocal(__instance.locale_ResultNotice_Upgrade_GreatSuccess);
				__instance.text_ResultNotice_Upgrade.color = __instance.color_ResultNotice_Upgrade_GreatSuccess;
				__instance.resultNotice_Upgrade.SetActive(true);
				__instance.UpgradeWeapon(targetEquipment, BlacksmithInterfaceManager.UpgradeResult.GreatSuccess);
				__instance.slotEffect_UpgradeSuccess.SetActive(true);
				__instance.attachedBlacksmithNPC.ScriptFloating(__instance.attachedBlacksmithNPC.monologData_Upgrade_GreatSuccess);
				return false;
			}
		}

		[HarmonyPatch(typeof(BlacksmithInterfaceManager), "WeaponRerollRequest")]
		class Reroll
		{
			static bool Prefix(BlacksmithInterfaceManager __instance, IEquipment targetEquipment)
			{
				Debug.Log("Equip Reroll");
				PlayerCharacter.instance.WeaponUpgradeMoney += 1000;
				bool flag = false;
				int num = 0;
				while (!flag)
				{
					if (num >= 100)
					{
						Debug.LogError("아이템 생성을 100번을 시도했지만 안 중복이 없었따!!!!");
						return false;
					}
					float[] weights;
					weights = __instance.rerollChance_GradeS;

					float[] weights2;
					weights2 = __instance.rerollChance_UpgradeThree;

					int num2 = RandomTool.WeightedRandom(weights);
					int num3 = RandomTool.WeightedRandom(weights2);
					int num4 = UnityEngine.Random.Range(0, __instance.rerollWeaponTables[num2].weaponTable.Length);
					if (IPlayerItemV3.itemAppearHistory.Contains(__instance.rerollWeaponTables[num2].weaponTable[num4].weaponCodeName))
					{
						Debug.Log("생성된 적이 있는 무기였음 " + __instance.rerollWeaponTables[num2].weaponTable[num4].weaponCodeName);
						num++;
					}
					else
					{
						IEquipment equipment = __instance.rerollWeaponTables[num2].weaponTable[num4];
						for (int i = 1; i < num3; i++)
						{
							if (equipment.nextLevelEquipment != null)
							{
								equipment = equipment.nextLevelEquipment;
							}
						}
						IEquipment targetEquipment2 = UnityEngine.Object.Instantiate<IEquipment>(equipment, __instance.transform.position, Quaternion.identity);
						PlayerCharacter.instance.DropWeapon(targetEquipment);
						UnityEngine.Object.DestroyImmediate(targetEquipment.gameObject);
						MethodInfo privMethod = __instance.GetType().GetMethod("DelayEquipRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
						if (__instance.inventorySlots[__instance.currentSlotIndex].equipSlotType.Equals(HoldingType.Weapon))
						{
							Timing.RunCoroutine(((IEnumerator<float>)privMethod.Invoke(__instance, new object[] { targetEquipment2, true, false, 999999 })).CancelWith(__instance.gameObject));
						}
						else
						{
							Timing.RunCoroutine(((IEnumerator<float>)privMethod.Invoke(__instance, new object[] { targetEquipment2, false, false, 999999 })).CancelWith(__instance.gameObject));
						}
						__instance.slotEffect_UpgradeSuccess.SetActive(true);
						flag = true;
					}
				}
				return false;
			}
		}
	}
}