using System;
using System.ComponentModel;
using BepInEx;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Utilla;

namespace GripVisionCode
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		//stuff for the rooms
		bool inRoom;
		static bool modEnabled = true;
		static bool passThrough;
		static bool inPrivate = PhotonNetwork.InRoom;

		//stuff for the teleport if you're not in a private
		static bool netOffDocumented = false;
		static bool netOff = true;
		static bool netOffKey = false;
		static Vector3 recordedPos;

		//stuff for the gameobjects / maps
		static GameObject TreeRoom;
		static GameObject Forest1;
		static GameObject Forest2;
		static GameObject Caves;
		static GameObject Canyons;
		static GameObject City1;
		static GameObject City2;
		static GameObject City3;
		static GameObject City4;
		static GameObject Mountain1;
		static GameObject Mountain2;

		//stuff for network triggers
		static GameObject NetworkingTrigger;
		static GameObject QuitBox;

		static void SetView(bool yesOrNo)
        {
			TreeRoom.GetComponent<MeshRenderer>().enabled = yesOrNo;
			Forest1.GetComponent<MeshRenderer>().enabled = yesOrNo;
			Forest2.SetActive(yesOrNo);
			Caves.GetComponent<MeshRenderer>().enabled = yesOrNo;
			Canyons.GetComponent<MeshRenderer>().enabled = yesOrNo;
			City1.GetComponent<MeshRenderer>().enabled = yesOrNo;
			City2.GetComponent<MeshRenderer>().enabled = yesOrNo;
			City3.GetComponent<MeshRenderer>().enabled = yesOrNo;
			City4.GetComponent<MeshRenderer>().enabled = yesOrNo;
			Mountain1.GetComponent<MeshRenderer>().enabled = yesOrNo;
			Mountain2.GetComponent<MeshRenderer>().enabled = yesOrNo;
		}

		static void SetCollision(bool yesOrNo)
		{
			TreeRoom.GetComponent<MeshCollider>().enabled = yesOrNo;
			Forest1.GetComponent<MeshCollider>().enabled = yesOrNo;
			Forest2.SetActive(yesOrNo);
			Caves.GetComponent<MeshCollider>().enabled = yesOrNo;
			Canyons.GetComponent<MeshCollider>().enabled = yesOrNo;
			City1.GetComponent<MeshCollider>().enabled = yesOrNo;
			City2.GetComponent<MeshCollider>().enabled = yesOrNo;
			City3.GetComponent<MeshCollider>().enabled = yesOrNo;
			City4.GetComponent<MeshCollider>().enabled = yesOrNo;
			Mountain1.GetComponent<MeshCollider>().enabled = yesOrNo;
			Mountain2.GetComponent<MeshCollider>().enabled = yesOrNo;
		}

		static void SetNetworkView(bool yesOrNo)
        {
			NetworkingTrigger.SetActive(yesOrNo);
			QuitBox.SetActive(yesOrNo);
		}

		void OnEnable() {
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			//HarmonyPatches.ApplyHarmonyPatches();
			Utilla.Events.GameInitialized += OnGameInitialized;
			modEnabled = true;
		}

		void OnDisable() {
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			//HarmonyPatches.RemoveHarmonyPatches();
			Utilla.Events.GameInitialized -= OnGameInitialized;
			modEnabled = false;
			SetView(true); // fix for exploit
			SetNetworkView(true);
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */

			TreeRoom = GameObject.Find("Level/treeroom/tree/Uncover TreeAtlas/CombinedMesh-TreeAtlas-mesh/TreeAtlas-mesh-mesh/");
			Forest1 = GameObject.Find("Level/forest/Uncover ForestCombined/CombinedMesh-GameObject (1)-mesh/GameObject (1)-mesh-mesh/");
			Forest2 = GameObject.Find("Level/forest/SmallTrees/");
			Caves = GameObject.Find("Level/cave/");
			Canyons = GameObject.Find("Level/canyon/");
			City1 = GameObject.Find("Level/city/CosmeticsRoomAnchor/Mesh Bakers/Uncover RoomAtlasOnly/CombinedMesh-RoomAtlasOnly-mesh/RoomAtlasOnly-mesh-mesh/");
			City2 = GameObject.Find("Level/city/CosmeticsRoomAnchor/Mesh Bakers/Uncover HatlasandShinyHatlas/CombinedMesh-StaticHats-mesh/StaticHats-mesh-mesh/");
			City3 = GameObject.Find("Level/city/CosmeticsRoomAnchor/Mesh Bakers/Uncover DefaultMat/CombinedMesh-StaticHead-mesh/StaticHead-mesh-mesh/");
			City4 = GameObject.Find("Level/city/CosmeticsRoomAnchor/Mesh Bakers/Uncover NoLighting/CombinedMesh-NoLighting-mesh/NoLighting-mesh-mesh/");
			Mountain1 = GameObject.Find("Level/mountain/Geometry/mountainside/");
			Mountain2 = GameObject.Find("Level/mountain/Geometry/mountainsideice/");
			NetworkingTrigger = GameObject.Find("NetworkTriggers/Networking Trigger/");
			QuitBox = GameObject.Find("NetworkTriggers/QuitBox/");
		}

		void Update()
		{
			/* Code here runs every frame when the mod is enabled */

			inPrivate = PhotonNetwork.InRoom;

			if (inRoom)
            {
				if (modEnabled)
                {
					InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out passThrough);
					if (passThrough)
                    {
						SetView(false);
					}
					else
                    {
						SetView(true);
					}

					//unused stuff for a righttrigger 
					//InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out netOffKey);
					if (netOffKey && passThrough)
                    {
						inPrivate = PhotonNetwork.InRoom;
						SetNetworkView(false);
						if (netOff && !netOffDocumented)
                        {
							//Debug.Log(inPrivate);
							netOffDocumented = true;
							netOff = false;
							recordedPos = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().transform.position;
						}
					}
					else
                    {
						SetCollision(false);
						SetNetworkView(true);
						//GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero; fix for goofy ahh bug
						//GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = true; fix for goofy ahh bug

						//GorillaLocomotion.Player.Instance.transform.position = recordedPos; fix for goofy ahh bug

						//GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = false; fix for goofy ahh bug
						if (netOffDocumented)
                        {
							//SetCollision(false);
							GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
							GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = true;

							GorillaLocomotion.Player.Instance.transform.position = recordedPos;

							GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = false;
							//SetCollision(true);
						}
						SetCollision(true);
						if (!netOff && netOffDocumented)
                        {
							netOffDocumented = false;
							//if (inPrivate)
                           // {
								//Debug.Log(inPrivate);
								


							//}
						}
						netOff = true;
					}
				} 
				else
                {
					SetView(true);
					SetNetworkView(true);
				}
            }
			else
            {
				SetView(true);
				SetNetworkView(true);
			}
		}

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;
		}

		/* This attribute tells Utilla to call this method when a modded room is left */
		[ModdedGamemodeLeave]
		public void OnLeave(string gamemode)
		{
			/* Deactivate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = false;
		}
	}
}
