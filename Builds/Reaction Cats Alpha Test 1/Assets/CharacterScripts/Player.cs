﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

//The Most Complicated Script Yet!
public class Player : NetworkBehaviour {
	//public string NewShownUpdate = "Update 2";
	[SyncVar] public NetworkInstanceId parentNetId;
	public PlayerAssign playerAssignSystem;
	public bool DoneActiveSelection = false;
	public bool DonePassiveSelection = false;
	public Cat curcat;
	public bool SetToCurrentHex = true;
	public GameObject AttackMenu;
	public bool Disabled = false;
	Animator anim;
	public TurnSystem tSystem; //Not Used Right Now!
	public float SpeedInMovement = 0;
	public int CurDirection = 1;
	public float CurrentBattleSpeed;
	public float TorsoHP = 120;
	public float Leg1HP;
	public float Leg2HP;
	public float Leg3HP;
	public float Leg4HP;
	public float HeadHP;
	public bool playingAnimation = false;
	public int Frame;
	public bool DidAReset = false;
	public bool DA;
	public bool DP;
	public float TailHP;
	public bool MovementInitilized = false;
	public int Team;
	public int Vision = 10;
	public int frameNumber = 1;
	public int currenthexrow = 0;
	public int currenthexnum = 0;
	//public Rigidbody2D playerRigidBody;
	public float DamageReduction;
    public int attacks = 2;
	public float maxSpeed = 8;
    public float speed = 8;
	public float JumpDistance = 2;
	public float SpeedIncrease;
    public string currenthex = "0 0";
	//public Ability Ability1;
	//public Ability Ability2;
	public List<Ability> abilitysList = new List<Ability>();
	public List<Vector3> endpoints = new List<Vector3>();
	//private Vector3 endpoint;
    public bool MOVINGCAT = false;
    public bool CATATTACKING = false;
	public bool DoneSpawningAttackMenu = false;
	public bool CATSKIPPINGPASSIVE = false;
	public bool CATSKIPPINGACTIVE = false;
	public bool CATJUMPING = false;
	public bool AbleToAttack = true;
	public bool RechargeEnabled = true;
	public List<Hex> hexes = new List<Hex>();
	public List<Hex> jumphexes = new List<Hex>();
	public List<Hex> banlist = new List<Hex>();
    public List<Hex> attackedhexes = new List<Hex>();
    public int? attackhexrow = 0;
    public int? attackhexnum = 0;
	public static Transform player;
	public bool BeenDoingSomething = false;
	public bool BeenDoingSomethingActive = false;
	//[SyncVar] public GameObject hexId;
	//[SyncVar] public Color objectColor;
	//public Transform actualPlayer;
    public Transform hexmap;
    public bool stopped = false;
    public bool running = false;
    public bool attRunning = false;
    public int oldhri = 0;
    public int oldhni = 0;
    public Hex ourhex;
    public IEnumerator move;
	public IEnumerator reset;
    public bool attacklock = false;
    public int? ari = 0; //Nullable Int, Made Like "int?" Instead Of Just "int", Can Be Set To "null"
    public int? ani = 0;
	public IEnumerator skipPas;
	public IEnumerator skipAct;
    public static bool donePassive = false;
    public static bool doneActive = false;
    public bool firstTimeSet = false;
    public IEnumerator attackq;
	public IEnumerator skipturn;
    public string happiness = ":|" ;
	public int CatId;
    public bool attackBlocker = false;
    public float staminaMax = 0;
    public float stamina = 0;
	public int itech = 0;
	public int CurHexNumR = 0;
	public int CurHexRowR = 0;
    public float lastStamina;
    public float numbersBetween = 0;
	public bool StopIncrease = false;
	public string CatName = "Ziva";
	public float AttackValue = 17;
	public float DefenseValue = 3;
	public static float BattleSpeed;
	public int AttackSpeed = 1;
	public int AttackRecharge = 1;
	public int CurrentTurnsRecharging = 0;
	public int AttacksPreformed = 0;
	public GameObject AttackMenusParent;
	public bool DoneActiveThing = false;
	public SpriteRenderer rend;
	public GameObject PlayerGameObject;
	public Transform CatAttackMenu;
	public List<Sprite> cats = new List<Sprite> ();
	public bool EnableShadowTest = false;
	public MeshFilter[] allMeshes;                              // array for all of the meshes in our scene
	public List<Vector3> allVertices = new List<Vector3>();     // list for vertices
	public Material shadowMaterial;
	public Camera mainCamera;
    public int holdinghexrow;
    public int holdinghexnum;

	//private NetworkIdentity objNetId;

	//Sprite[] catsprites = Resources.LoadAll<Sprite>("CatSprites/RCSPRITESHEETULTIMATE");

	void Awake() {
		GetComponent<Renderer>().shadowCastingMode =  UnityEngine.Rendering.ShadowCastingMode.TwoSided;
		GetComponent<Renderer>().receiveShadows = true;
		GetComponent<SpriteRenderer>().shadowCastingMode =  UnityEngine.Rendering.ShadowCastingMode.TwoSided;
		GetComponent<SpriteRenderer>().receiveShadows = true;
	}

	public override void OnStartClient()
	{
		// When we are spawned on the client,
		// find the parent object using its ID,
		// and set it to be our transform's parent.
		GameObject parentObject = ClientScene.FindLocalObject(parentNetId);
			transform.SetParent (parentObject.transform);
			if (!parentObject.GetComponent<PlayerAssign> ().catlist.Contains (this)) {
				parentObject.GetComponent<PlayerAssign> ().catlist.Add (this);
			}

			if (!TurnSystem.players.Contains (parentObject.GetComponent<PlayerAssign> ())) {
				TurnSystem.players.Add (parentObject.GetComponent<PlayerAssign> ());
			}
			CatId = parentObject.GetComponent<PlayerAssign> ().catlist.IndexOf (this) + 1;
			Debug.Log ("Syncing Playerassign Cats...");
	}

	[Command] public void CmdSyncCatParent() {
		RpcSyncCatParent ();
		Debug.Log ("Setting Proper Parent Sync");
	}

	[ClientRpc] public void RpcSyncCatParent() {
		GameObject parentObject = ClientScene.FindLocalObject (parentNetId);
		transform.SetParent (parentObject.transform);
		if (!parentObject.GetComponent<PlayerAssign> ().catlist.Contains (this)) {
			parentObject.GetComponent<PlayerAssign> ().catlist.Add (this);
		}

		if (!TurnSystem.players.Contains (parentObject.GetComponent<PlayerAssign> ())) {
			TurnSystem.players.Add (parentObject.GetComponent<PlayerAssign> ());
		}

		CatId = parentObject.GetComponent<PlayerAssign> ().catlist.IndexOf (this) + 1;
		Debug.Log ("Parent Syncing On Cient Scene");
	}


    /* The Starter Machine Of Pure D.E.R.P (Derpily Epic Rotor Power) */
    public void Start ()
	{
		Debug.Log ("Playerscript Running On Update 2");
		PlayerGameObject = this.gameObject;
		player = this.transform;
		//ourhex = GameObject.Find ("WHY YOU DO DIS NICK. -TO PREVENT DANG ERRORS").GetComponent<Hex> ();
		hexmap = GameObject.Find ("HexGrid").transform;
		AttackMenusParent = GameObject.Find ("GameManager/GamePlayMechanics");
		AttackMenu = GameObject.Find ("GameManager/GamePlayMechanics/CatPartTargeting");
		CatAttackMenu = AttackMenu.transform;
		playerAssignSystem = transform.parent.GetComponent<PlayerAssign> ();
		TurnSystem.cats.Add (this);
		Sprite[] catsprites = Resources.LoadAll<Sprite>("CatSprites/RCSPRITESHEETULTIMATE");
		foreach (Sprite sprite in catsprites) {
			cats.Add (sprite);
		}
		staminaMax = curcat.MaxStamina;
		catsprites = Resources.LoadAll<Sprite>("RCSPRITESHEETULTIMATE");
		if (cats [1] != null) {
			print ("Sprite Initilization Successful!");
		}
		rend = player.GetComponent<SpriteRenderer> ();
		var LegsHP = TorsoHP / 4;
		var HeadsHP = TorsoHP / 3;
		var TailsHP = TorsoHP / 5;
		Leg1HP = LegsHP;
		Leg2HP = LegsHP;
		Leg3HP = LegsHP;		
		Leg4HP = LegsHP;
		TailHP = TailsHP;
		HeadHP = HeadsHP;
		rend.sprite = (Sprite)cats[13];
		print (rend.sprite.bounds.center);
		BattleSpeed = maxSpeed;
        player = this.transform;
        lastStamina = staminaMax;
		//if (Ability1 != null) {
		//	Ability1.player = this;
		//	if (Ability1.AbilityType == "Utility") {
		//		Ability1.PreformAbility ();
		//	}
		//}
		//if (Ability2 != null) {
		//	Ability2.player = this;
		//	if (Ability2.AbilityType == "Utility") {
		//		Ability2.PreformAbility ();
		//	}
	    //}
		anim = GetComponent<Animator> ();
		anim.runtimeAnimatorController = Resources.Load ("Animations/Player") as RuntimeAnimatorController;
		AttackMenu.SetActive (false);
		DefenseValue = curcat.Defense;
		AttackValue = curcat.Attack;
		TorsoHP = curcat.TorsoHP;
		maxSpeed = curcat.MaxSpeed;
		//SpeedIncrease = curcat.IncreaseInSpeed;
		CatName = curcat.CatName;
		AttackSpeed = curcat.AttackSpeed;
		AttackRecharge = curcat.AttackRecharge;
		for (var uz = 0; uz < curcat.abilitys.Count; uz++) {
			abilitysList.Add (curcat.abilitys.ElementAt (uz));
		}

		foreach (Ability thingAbility in abilitysList) {
			thingAbility.player = this;
			if (thingAbility.AbilityType == "Utility") {
				thingAbility.PreformAbility ();
			}
		}
		//for (int i = 0; i < curcat.abilitys.Count; i++) {
		///	abilitysList.Add (curcat.abilitys.ElementAt(i));
		//}
		//Ability1 = curcat.abilitys.ElementAt (0);
		//Ability2 = curcat.abilitys.ElementAt (1);
		speed = maxSpeed;
		Team = playerAssignSystem.Team;
		if (Team == playerAssignSystem.Team) {
			Debug.Log ("Team Is Properly Set!");
		}
		if (!this.transform.parent.gameObject.GetComponent<PlayerAssign> ().catlist.Contains (this)) {
			this.transform.parent.gameObject.GetComponent<PlayerAssign> ().catlist.Add (this);
		}
		if (Team == 1) {
			if (playerAssignSystem.catlist.IndexOf (this) == 0) {
				currenthexrow = 0;
				currenthexnum = 0;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 1) {
				currenthexrow = 0;
				currenthexnum = 1;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 2) {
				currenthexrow = 1;
				currenthexnum = 0;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 3) {
				currenthexrow = 1;
				currenthexnum = 1;
			}
		}
		if (Team == 2) {
			if (playerAssignSystem.catlist.IndexOf (this) == 0) {
				currenthexrow = 9;
				currenthexnum = 9;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 1) {
				currenthexrow = 9;
				currenthexnum = 8;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 2) {
				currenthexrow = 8;
				currenthexnum = 9;
			}
			if (playerAssignSystem.catlist.IndexOf (this) == 3) {
				currenthexrow = 8;
				currenthexnum = 8;
			}
		}

		foreach (Transform properHex in hexmap) {
			string TheProperHex = currenthexrow + " " + currenthexnum;
			if (properHex.name == TheProperHex) {
				this.transform.position = properHex.position;
            //    CmdChangeHex(properHex.name, "Player", PlayerGameObject);
            }
        }

		mainCamera = Camera.main;
	//	foreach (Transform hexamajigset in hexmap) {
	//		string realHexName = currenthexrow + " " + currenthexnum;
	//		if (hexamajigset.name == realHexName) {
	//			var trueHex = hexamajigset.GetComponent<Hex> ();
	//			tr/ueHex.objectOnhex = this.transform;
	//			//trueHex.OnHex = "Player";
	//		}
	//	}

		//endpoint = transform.position;
		//playerRigidBody = player.gameObject.GetComponent<Rigidbody2D> ();
		//player.position = new Vector3(transform.position.x, transform.position.y + rend.bounds.extents.y, transform.position.z);
	}

	//private void OnDrawGizmos()
	//{
	//	Bounds b = rend.bounds;
	//	Gizmos.DrawWireCube(b.center, b.extents * 2);
	//}


    /* Updater 3000 Is Amazing */
	void FixedUpdate() {
		//if (Input.GetMouseButtonDown (0)) {
		//	var mouseSelection = CheckForObject ();
		//	if (mouseSelection == null) {
		//		Debug.Log ("No Object Was Detected.");
		//	}
		//	if (mouseSelection != null) {
		//		Debug.Log (transform.name + " Was Detected.");
		//	}
		//}

		//if (Input.GetMouseButtonDown (0)) {
		//	var ActualMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		//	Vector3 direction = (transform.position - ActualMousePosition).normalized;
		//	Ray ray = Ray (transform.position, direction);
			//
	//	}

		rend.receiveShadows = true;
		rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		if (playerAssignSystem == null) {
			playerAssignSystem = transform.parent.GetComponent<PlayerAssign> ();
			print ("Player Assign System Is Null!");
		} 
		DP = donePassive;
		DA = doneActive;
		//if (!isLocalPlayer) {
		//	return;
		//}
			CorrectDirection ();
			CorrectSpeed ();

			anim.SetFloat ("Speed", SpeedInMovement);
			anim.SetInteger ("Direction", CurDirection);
			if (MovementInitilized == true) {
				var point = endpoints.First ();
				transform.position = Vector3.Lerp (transform.position, point, 6f * Time.deltaTime);
				if (Vector3.Distance (transform.position, point) < 0.8) {
					endpoints.Remove (point);
				}
				if (endpoints.Count <= 0) {
					transform.position = point;
					MovementInitilized = false;
				}
			}
		}


	private GameObject CheckForObject() {
		Vector2 TouchPosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D[] collidersAtPosition = Physics2D.RaycastAll (TouchPosition, Vector2.zero);

		SpriteRenderer closest = null;
		foreach (RaycastHit2D hit in collidersAtPosition) {
			if (closest == null) {
				closest = hit.collider.gameObject.GetComponent<SpriteRenderer> ();
				continue;
			}

			var hitSprite = hit.collider.gameObject.GetComponent<SpriteRenderer> ();

			if (hitSprite == null)
				continue;

			if (hitSprite.sortingOrder > closest.sortingOrder) {
				closest = hitSprite;
			}
		}

		return closest != null ? closest.gameObject : null;
	}

    void Update()
    {
        //if (!isLocalPlayer) {
        //	return;
        //}
        //if(Input.GetMouseButtonDown(0))
        //{
        //	var mouseSelection = CheckForObject();
        //	if(mouseSelection == null)
        //	{
        //		Debug.Log("Nothing Detected By Mouse");
        //	}
        //	else
        //	{
        //		Debug.Log(mouseSelection.gameObject + "Was Detected");
        //		if (mouseSelection.gameObject.GetComponent<Player> ()) {
        //			Debug.Log ("A Player Was Detected.");
        //		}
        //	}
        //}

        reset = SwitchBack();
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        rend.receiveShadows = true;
        //this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 1);
        if (Disabled == false)
        {
            if (playerAssignSystem != null)
            {
                if (playerAssignSystem.currentlyControlling != CatId)
                {
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Player Assign System Is Null!");
            }
            //if (!isLocalPlayer) {
            //	return;
            //}
            CorrectDirection();
            CorrectSpeed();

            anim.SetFloat("Speed", SpeedInMovement);
            anim.SetInteger("Direction", CurDirection);
            //rend.transform.position = player.transform.position;
            if (Disabled == false)
            {

                foreach (Ability thingAbility in abilitysList)
                {
                    thingAbility.player = this;
                    if (thingAbility.AbilityType == "Constant")
                    {
                        thingAbility.PreformAbility();
                    }
                }

                //if (Ability1 != null) {
                //	Ability1.player = this;
                //	if (Ability1.AbilityType == "Constant") {
                //		Ability1.PreformAbility ();
                //	}
                //	Ability1.player = this;
                //	}
                //	if (Ability2 != null) {
                //		Ability2.player = this;
                //		if (Ability2.AbilityType == "Constant") {
                //			Ability2.PreformAbility ();
                ///		}
                //		Ability2.player = this;
                //	}
                CurrentBattleSpeed = BattleSpeed;
                if (stamina < lastStamina)
                {
                    lastStamina = stamina;
                    speed = speed - 1;
                }
                if (stamina > lastStamina)
                {
                    lastStamina = stamina;
                    speed = speed + 1;
                }
                if (stamina > staminaMax)
                {
                    stamina = staminaMax;
                    speed = maxSpeed;
                    lastStamina = stamina;
                }
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
                if (CATSKIPPINGPASSIVE == true)
                {
                    Hex.playerscript = this;
                    skipPas = SkipPassive();
                    StartCoroutine(skipPas);
                }
                if (CATSKIPPINGACTIVE == true)
                {
                    Hex.playerscript = this;
                    skipAct = SkipActive();
                    StartCoroutine(skipAct);
                }
                if (AttacksPreformed >= AttackSpeed)
                {
                    RechargeEnabled = false;
                    CurrentTurnsRecharging = AttackRecharge;
                    AttacksPreformed = 0;
                }
                if (CurrentTurnsRecharging == 0)
                {
                    AbleToAttack = true;
                }
                else
                {
                    AbleToAttack = false;
                }
                if (CATATTACKING == true && CurrentTurnsRecharging == 0)
                {
                    if (BeenDoingSomethingActive == false)
                    {
                        playerAssignSystem.DoingSomething = true;
                        BeenDoingSomethingActive = true;
                    }
                    Hex.playerscript = this;
                    doneActive = true;
                    MOVINGCAT = false;
                    stopped = false;
                    attackhexrow = currenthexrow;
                    attackhexnum = currenthexnum;
                    ari = Hex.attackrowid;
                    ani = Hex.attacknumid;

                    if (Input.GetKeyDown(KeyCode.C) && running == false)
                    {
                        attackq = Attack();
                        StartCoroutine(attackq);
                        playerAssignSystem.NeedingCatSelected = true;
                        playerAssignSystem.SetupActiveReset();
                    }
                    if (ari >= attackhexrow - 1 && ari <= attackhexrow + 1 && ani <= attackhexnum + 1 && ani >= attackhexnum - 1)
                    {
                        if (ari > attackhexrow && ani > attackhexnum && ari % 2 != 0)
                        {
                            stopped = true;
                        }
                        if (ari < attackhexrow && ani > attackhexnum && ari % 2 != 0)
                        {
                            stopped = true;
                        }
                        if (ari > attackhexrow && ani < attackhexnum && ari % 2 == 0)
                        {
                            stopped = true;
                        }
                        if (ari < attackhexrow && ani < attackhexnum && ari % 2 == 0)
                        {
                            stopped = true;
                        }

                        if (stopped == false && attackBlocker == false)
                        {
                            if (running == false)
                            {
                                foreach (Transform hex in hexmap)
                                {
                                    attackhexrow = ari;
                                    attackhexnum = ani;
                                    string supermaster = ari + " " + ani;
                                    if (hex.name == supermaster)
                                    {
                                        var hexenon = hex.GetComponent<Hex>();
                                        if (!attackedhexes.Contains(hexenon))
                                        {
                                            attackedhexes.Add(hexenon);
                                        }

                                        if (!attackedhexes.Contains(hexenon))
                                        {
                                            attackBlocker = true;
                                            attackedhexes.Add(hexenon);
                                        }

                                        //if (attackedhexes.Count >= attacks) {
                                        //	attackq = Attack ();
                                        //	StartCoroutine (attackq);
                                        //	}
                                    }
                                }
                            }
                        }
                    }
                }

                if (MOVINGCAT == true)
                {
                    //PlayerTracker.player = this.transform;
                    //PlayerTracker.offset = PlayerTracker.thisCamera.transform.position - this.transform.position;
                    //Debug.Log ("A Player Is Being Moved");

                    if (donePassive == false)
                    {
                        if (BeenDoingSomething == false)
                        {
                            playerAssignSystem.DoingSomething = true;
                            BeenDoingSomething = true;
                        }
                        Hex.playerscript = this;
                        donePassive = false;
                        CATATTACKING = false;
                        stopped = false;
                        int hri = 0;
                        int hni = 0;
                        //if (SetToCurrentHex == true) {
                        //	hri = currenthexrow;
                        //	hni = currenthexnum;
                        //	SetToCurrentHex = false;
                        //}
                        if (Input.GetKeyDown(KeyCode.Q))
                        {
                            if (hexes.Count > 1)
                            {
                                Hex lasthex = hexes.Last();
                                Int32 lastindex = hexes.IndexOf(lasthex) - 1;
                                Hex finale = hexes.ElementAt(lastindex);
                                PlayerTracker.m_Target = finale.transform;
                                hexes.Remove(lasthex);
                                banlist.Add(lasthex);
                                hri = finale.datarowid;
                                hni = finale.datanumid;
                                currenthexnum = hni;
                                currenthexrow = hri;
                                oldhri = hri;
                                oldhni = hni;
                            }
                        }
                        else
                        {
                            hri = Hex.hexrowid;
                            hni = Hex.hexnumid;
                        }
                        if (Input.GetKeyDown(KeyCode.C) && running == false)
                        {
                            // if (hexes.Count <= speed / 2)
                            // {
                            //     stamina += 0;
                            // }
                            move = Move();
                            StartCoroutine(move);
                            playerAssignSystem.SetupPassiveReset();
                            playerAssignSystem.NeedingCatSelected = true;
                        }

                        if (SetToCurrentHex == true)
                        {
                            hri = currenthexrow;
                            hni = currenthexnum;
                            Hex.hexrowid = currenthexrow;
                            Hex.hexnumid = currenthexnum;
                            SetToCurrentHex = false;
                        }
                        Transform theProperHex = null;
                        Transform theSelectedHex = null;

                       // bool MovementOn = true;
                       // if (MovementOn == true)
                       // {
                         //  Transform theProperHex = this.transform;
                            foreach (Transform aHexThing in hexmap)
                            {
                                if (aHexThing.name == currenthexrow + " " + currenthexnum)
                                {
                                    theProperHex = aHexThing;
                              }
                            }
                            foreach (Transform selectedHexThing in hexmap)
                            {
                                if (selectedHexThing.name == hri + " " + hni)
                                {
                                    theSelectedHex = selectedHexThing;
                                }
                            }
                       //     MovementOn = false;
                     //   }
                        //if (hri >= currenthexrow - 1 && hri <= currenthexrow + 1 && hni <= currenthexnum + 1 && hni >= currenthexnum - 1) {
                        if (Vector2.Distance(theProperHex.position, theSelectedHex.position) < 7)
                        {
                          //  Debug.Log(Vector2.Distance(theProperHex.position, theSelectedHex.position).ToString());
                            if (hexes.Count < speed)
                            {
                               // if (hri > currenthexrow && hni > currenthexnum && hri % 2 != 0)
                               // {
                               //     stopped = true;
                               // }
                               // if (hri < currenthexrow && hni > currenthexnum && hri % 2 != 0)
                               // {
                               //     stopped = true;
                               // }
                               // if (hri > currenthexrow && hni < currenthexnum && hri % 2 == 0)
                               // {
                                //    stopped = true;
                                //}
                                //if (hri < currenthexrow && hni < currenthexnum && hri % 2 == 0)
                               // {
                                  //  stopped = true;
                                //}
                                //Transform theProperHex = this.transform;
                                //foreach(Transform aHexThing in hexmap)
                                // {
                                // if (aHexThing.name == currenthexrow + " " + currenthexnum)
                                //{
                                //      theProperHex = aHexThing;
                                //    }
                                // }
                                for (int y = 0; y < banlist.Count; y++)
                                {
                                    var incednium = banlist.ElementAt(y);
                                    if (currenthexnum == incednium.datanumid && currenthexrow == incednium.datarowid)
                                    {
                                        Hex.hexnumid = oldhni;
                                        Hex.hexrowid = oldhri;
                                    }
                                }

                                if (stopped == false)
                                {
                                    if (running == false)
                                    {
                                        foreach (Transform hex in hexmap)
                                        {
                                            string supermaster = hri + " " + hni;
                                            if (hex.name == supermaster)
                                            {
                                                var hexcontrol = hex.GetComponent<Hex>();
                                                //Debug.Log("Distance Between: " + theProperHex.name + " And: " + hexcontrol.transform.name + " Is: " + Vector2.Distance(theProperHex.position, hexcontrol.transform.position).ToString());
                                                if (hexcontrol.OnHex != "DesComp")
                                                {
                                                    if (hexcontrol.OnHex != "Player" || hexcontrol.objectOnhex == this.transform)
                                                    {
                                                        if (!hexes.Contains(hexcontrol) && !banlist.Contains(hexcontrol))
                                                        {
                                                            hexes.Add(hexcontrol);
                                                            PlayerTracker.m_Target = hexcontrol.transform;
                                                            // endpoints.Add (hex.position);
                                                            currenthexrow = hri;
                                                            currenthexnum = hni;
                                                           // MovementOn = false;
                                                            //currenthexrow = hri;
                                                            //currenthexnum = hni;
                                                        }


                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

	void Travel() {
		SpeedInMovement = 1;
	}
	void SetDirection(int Direction) {
		//int Direct = Mathf.Clamp (Direction, 1, 6);
		anim.SetInteger ("Direction", Direction);
		CurDirection = Direction;
	}
	void CorrectDirection() {
		anim.SetInteger ("Direction", CurDirection);
	}
	void CorrectSpeed() {
		anim.SetFloat ("Speed", SpeedInMovement);
	}

    IEnumerator Move()
	{
		DonePassiveSelection = true;
		playerAssignSystem.DoingSomething = false;
        PlayerTracker.m_Target = this.transform;
        // PlayerTracker.player = this.gameObject;
        // PlayerTracker.offset = PlayerTracker.thisCamera.transform.position - this.transform.position;
        //CmdFinishPassive ();
        //if (TurnSystem.cats.Count > 1) {
        yield return new WaitUntil (() => playerAssignSystem.AllPlayersDonePassive);
		//	yield return new WaitUntil (() => TurnSystem.P2PassiveFinish == true);
		//}
		for (var q = 0; q < hexes.Count; q++) {

			var quad = hexes [q];
            if (quad.OnHex == "Player" && quad.objectOnhex != null)
            {
                break;
            }
            GameObject hexo = quad.gameObject;
			string hexoName = quad.transform.name;
			CmdRemoveHex (hexoName);
			//playingAnimation = true;
			endpoints.Add (quad.transform.position);
			currenthex = quad.datarowid + " " + quad.datanumid;
			if (quad == hexes.Last()) {
				CmdChangeHex(hexoName, "Player", PlayerGameObject);
			}
			//Travel ();
			CorrectDirection();
			SpeedInMovement = 1;
			//quad.GetComponent<SpriteRenderer> ().color = new Color32 (255, 224, 53, 255);
			quad.GetComponent<SpriteRenderer>().sprite = quad.selectedForMoveSprite;
			quad.forceseter = true;
			CorrectDirection ();
			if (quad.datanumid < CurHexNumR && quad.datarowid == CurHexRowR) {
				SetDirection (4);
				//rend.sprite = cats [13];
				//print ("Left!");
			}
			if (quad.datanumid > CurHexNumR && quad.datarowid == CurHexRowR) {
				SetDirection (1);
				//rend.sprite = cats [25];
				//print ("Right!");
			}

			if (quad.transform.position.x > player.position.x) {
				if (quad.datanumid < CurHexNumR && quad.datarowid > CurHexRowR) {
					SetDirection (3);
					//rend.sprite = cats [4];
					//print ("Left Down!");
				}
				if (quad.datanumid >= CurHexNumR && quad.datarowid > CurHexRowR) {
					SetDirection (2);
					//rend.sprite = cats [27];
					//print ("Right Down!");
				}
			}
			if (quad.transform.position.x < player.position.x) {
			if (quad.datanumid <= CurHexNumR && quad.datarowid > CurHexRowR) {
				SetDirection (3);
				//rend.sprite = cats [4];
				//print ("Left Down!");
			}
			if (quad.datanumid > CurHexNumR && quad.datarowid > CurHexRowR) {
				SetDirection (2);
				//rend.sprite = cats [27];
				//print ("Right Down!");
			}
		}

			if (quad.transform.position.x > player.position.x) {
				if (quad.datanumid < CurHexNumR && quad.datarowid < CurHexRowR) {
					SetDirection (5);
					//rend.sprite = cats [16];
					//print ("Left Up!");
				}
				if (quad.datanumid >= CurHexNumR && quad.datarowid < CurHexRowR) {
					SetDirection (6);
					//rend.sprite = cats [46];
					//print ("Right Up!");
				}
			}
			if (quad.transform.position.x < player.position.x) {
				if (quad.datanumid <= CurHexNumR && quad.datarowid < CurHexRowR) {
					SetDirection (5);
					//rend.sprite = cats [16];
					//print ("Left Up!");
				}
				if (quad.datanumid > CurHexNumR && quad.datarowid < CurHexRowR) {
					SetDirection (6);
					//rend.sprite = cats [46];
					//print ("Right Up!");
				}
			}
			CorrectDirection ();
			CorrectDirection ();
			CorrectSpeed ();

			CurHexRowR = quad.datarowid;
			CurHexNumR = quad.datanumid;
			//Travel ();
			//yield return new WaitForSeconds (0.1f);
			while (Vector3.Distance (transform.position, quad.transform.position) > 0.3) {
				CorrectDirection ();
				CorrectSpeed ();
				Vector3 properTransformPosition = new Vector3 (this.transform.position.x, this.transform.position.y, 1);
				//Vector3 quadProperPosition = new Vector3 (quad.transform.position.x, quad.transform.position.y, 1);
				transform.position = Vector3.MoveTowards (properTransformPosition, quad.transform.position, 10f * Time.deltaTime);
				CorrectDirection ();
				CorrectSpeed ();
				SpeedInMovement = 1;
				yield return new WaitForEndOfFrame ();
			}
			CorrectDirection ();
			CorrectSpeed ();

			transform.position = quad.transform.position;
			SpeedInMovement = 0;
		    //endpoint = quad.transform.position;
			//rend.transform.position = player.position;

           // player.position = quad.transform.position;
			//player.position = Vector3.MoveTowards(player.position, quad.transform.position, 3 * Time.deltaTime);
			//rend.transform.position = quad.transform.position;
            running = true;
        }
		//playingAnimation = false;
        running = false;
        MOVINGCAT = false;
		SpeedInMovement = 0;
		//MovementInitilized = true;
		//if (MovementInitilized == false) {
			
			//StopIncrease = false;
			float speedhalffalse = speed / 2;
			float speedquarterfalse = speed / 4;
			double speedHalf = Math.Ceiling (speedhalffalse);
			double speedQuarter = Math.Ceiling (speedquarterfalse);
			if (speedQuarter < 2) {
				speedQuarter = 2;
			}
			if (hexes.Count > speedHalf) {
				stamina = stamina - 1;
			}
			if (hexes.Count <= speedQuarter && StopIncrease == false) {
				stamina = stamina + 1;
			}
			for (var n = 0; n < hexes.Count; n++) {
				var en = hexes [n];
				en.forceseter = false;
				//en.GetComponent<SpriteRenderer> ().color = new Color32 (255, 224, 53, 255);
			}
			StopIncrease = true;
			hexes.Clear ();
			banlist.Clear ();
			endpoints.Clear ();
			yield return new WaitForSeconds (0.2f);
		    Hex.hexrowid = currenthexrow;
		    Hex.hexnumid = currenthexnum;
			StopIncrease = false;
		//ActionInitilized.disablePassive = true;
			attackhexnum = currenthexnum;
			attackhexrow = currenthexrow;
		    //DonePassiveSelection = true;
	    //playerAssignSystem.currentlyControlling += 1;
		//playerAssignSystem.SetupPassiveReset();
		//if (playerAssignSystem.currentlyControlling == playerAssignSystem.NumberOfCats) {
		//	playerAssignSystem.currentlyControlling = 1;
		//	TurnSystem.Locked = false;
		//	donePassive = true;
		//} else {
		//	playerAssignSystem.currentlyControlling = 1;
		//	playerAssignSystem.SetupPassiveReset ();
		//}
			//TurnSystem.Locked = false;
			//donePassive = true;
		//}
    }
		
    IEnumerator Attack()
    {
		//CmdFinishActive ();
		//playerAssignSystem.DoingActive = true;
		DoneActiveSelection = true;
		playerAssignSystem.DoingSomething = false;
        yield return new WaitUntil (() => playerAssignSystem.AllPlayersDoneActive == true);
        yield return new WaitForSeconds(0.43f);
		for (var n = 0; n < attackedhexes.Count; n++) {
			attackBlocker = true;
			var quint = attackedhexes [n];
			quint.GetComponent<SpriteRenderer> ().color = new Color32 (255, 149, 36, 255);
			quint.forceseter = true;
			Hex hexman = quint.GetComponent<Hex> ();
			if (hexman.datanumid > currenthexnum && hexman.datarowid == currenthexrow) {
				SetDirection (1);
			}
			if (hexman.datanumid < currenthexnum && hexman.datarowid == currenthexrow) {
				SetDirection (4);
			}

			if (quint.transform.position.x > player.position.x) {
				if (quint.datanumid < currenthexnum && quint.datarowid < currenthexrow) {
					SetDirection (5);
					//rend.sprite = cats [16];
					//print ("Left Up!");
				}
				if (quint.datanumid >= currenthexnum && quint.datarowid < currenthexrow) {
					SetDirection (6);
					//rend.sprite = cats [46];
					//print ("Right Up!");
				}
			}
			if (quint.transform.position.x < player.position.x) {
				if (hexman.datanumid <= currenthexnum && hexman.datarowid < currenthexrow) {
					SetDirection (5);
					//rend.sprite = cats [16];
					//print ("Left Up!");
				}
				if (hexman.datanumid > currenthexnum && hexman.datarowid < currenthexrow) {
					SetDirection (6);
					//rend.sprite = cats [46];
					//print ("Right Up!");
				}
			}

			if (quint.OnHex == "DesComp") {
				DoneActiveSelection = false;
				DesComp damager = quint.objectOnhex.GetComponent<DesComp> ();
				float EnemyDefense = damager.DefenseValue;
				float EnemySpeed = damager.speed;
				//float DamageCalculation = AttackValue * AttackValue / AttackValue - EnemyDefense;
				//double Damage = Math.Ceiling (DamageCalculation);
				//GameObject ClonedMenu = Instantiate (AttackMenu, AttackMenusParent.transform) as GameObject;
				//ClonedMenu.SetActive(true);
				//GameObject ProperMenu = (GameObject) Instantiate(CatAttackMenu.gameObject);
				//ProperMenu.transform.SetParent (AttackMenusParent.transform, false);
				//AttackMenu.SetActive(true);
				if (DoneSpawningAttackMenu == false) {
					GameObject properMenu = (GameObject)Instantiate (AttackMenu, AttackMenusParent.transform);
					properMenu.transform.SetParent (AttackMenusParent.transform, false);
					properMenu.SetActive (true);
					DoneSpawningAttackMenu = true;
					Debug.Log ("Attacked!");
					var SetTarget = AttackMenu.GetComponent<CatPartTargeting> ();
					SetTarget.AttackValue = AttackValue;
					SetTarget.SpeedValue = BattleSpeed;
					SetTarget.TargetSpeed = EnemySpeed;
					SetTarget.TargetDefense = EnemyDefense;
					SetTarget.Target = quint.objectOnhex;
					SetTarget.Vision = Vision;
					SetTarget.SpeedIncreaseA += SpeedIncrease;
				//	yield return new WaitUntil (() => properMenu == null);
				}
				//DoneActiveThing = true;
				//DoneActiveSelection = true;
			}
			if (quint.OnHex == "Player" && quint.objectOnhex != this.transform) {
				Player damager = quint.objectOnhex.GetComponent<Player> ();
				if (damager.Team != Team) {
					
					float EnemyDefense = damager.DefenseValue;
					float EnemySpeed = damager.CurrentBattleSpeed;
					float EnemySpeedIncrease = damager.SpeedIncrease;
					//float DamageCalculation = AttackValue * AttackValue / AttackValue - EnemyDefense;
					//double Damage = Math.Ceiling (DamageCalculation);
					//damager.TorsoHP -= Damage;
					if (DoneSpawningAttackMenu == false) {
						GameObject properMenu = (GameObject)Instantiate (AttackMenu, AttackMenusParent.transform);
						properMenu.transform.SetParent (AttackMenusParent.transform, false);
						properMenu.SetActive (true);
						DoneSpawningAttackMenu = true;
						Debug.Log ("Attacked!");
						var SetTarget = AttackMenu.GetComponent<CatPartTargeting> ();
						SetTarget.DamageReduction = damager.DamageReduction;
						SetTarget.AttackValue = AttackValue;
						SetTarget.SpeedValue = BattleSpeed;
						SetTarget.TargetSpeed = EnemySpeed;
						SetTarget.TargetDefense = EnemyDefense;
						SetTarget.Target = quint.objectOnhex;
						SetTarget.Vision = Vision;
						SetTarget.SpeedIncreaseA += SpeedIncrease;
						SetTarget.SpeedIncreaseD += EnemySpeedIncrease;
					//	yield return new WaitUntil (() => properMenu == null);
					}
					//	AttackMenu.SetActive (true);
					//	var SetTarget = AttackMenu.GetComponent<CatPartTargeting> ();
					//	SetTarget.DamageReduction = damager.DamageReduction;
					//	SetTarget.AttackValue = AttackValue;
					//	SetTarget.SpeedValue = BattleSpeed;
					//	SetTarget.TargetSpeed = EnemySpeed;
					//	SetTarget.TargetDefense = EnemyDefense;
					//	SetTarget.Target = quint.objectOnhex;
					//	SetTarget.Vision = Vision;
					//	SetTarget.SpeedIncreaseA += SpeedIncrease;
					//	SetTarget.SpeedIncreaseD += EnemySpeedIncrease;
					//}
				}
				//yield return new WaitForSeconds(0.2f);
				running = true;
				quint.WobbleLikeTheWeebles ();
			}
		}
        yield return new WaitForSeconds(0.2f);
		CATATTACKING = false;
        for (var u = 0; u < attackedhexes.Count; u++)
        {
            var yu = attackedhexes[u];
            yu.forceseter = false;
            yu.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
			AttacksPreformed += 1;
        }
        attackedhexes.Clear();
        yield return new WaitForSeconds(0.2f);
		//DoneActiveSelection = true;
		TurnSystem.Locked = false;
		doneActive = true;
        running = false;
        firstTimeSet = false;
		attackhexrow = 0;
        attackhexnum = 0;
        ari = 0;
        ani = 0;
		DoneActiveThing = true;
		DoneActiveSelection = true;
		//DoneActiveSelection = true;
		//playerAssignSystem.currentlyControlling += 1;
		//playerAssignSystem.SetupActiveReset ();
		//reset = SwitchBack ();
		//StartCoroutine (reset);
		//if (playerAssignSystem.currentlyControlling == playerAssignSystem.NumberOfCats) {
		//	TurnSystem.Locked = false;
		//	doneActive = true;
		//	playerAssignSystem.currentlyControlling = 1;
		//	playerAssignSystem.SetupActiveReset ();
		//	//reset = SwitchBack ();
		//	//StartCoroutine (reset);
		//} else {
		//	playerAssignSystem.currentlyControlling += 1;
		//	TurnSystem.Locked = false;
		//	doneActive = true;
		//	playerAssignSystem.SetupActiveReset ();
		//}
	}
	IEnumerator SwitchBack() {
		yield return new WaitForSeconds (1.0f);
     	playerAssignSystem.currentlyControlling = 1;
		playerAssignSystem.AllPlayersDonePassive = false;
		playerAssignSystem.AllPlayersDoneActive = false;
		playerAssignSystem.PlayersDoneActiveThings = false;
		playerAssignSystem.FinishedOfficalPassive = false;
		playerAssignSystem.FinishedOfficalActive = false;
		DoneSpawningAttackMenu = false;
		DoneActiveThing = false;
		SetToCurrentHex = true;
		DonePassiveSelection = false;
		DoneActiveSelection = false;
		playerAssignSystem.DoingPassive = true;
		playerAssignSystem.DoingActive = false;
		ActionInitilized.disableActive = false;
		ActionInitilized.disablePassive = false;
		TurnSystem.pasStop = false;
		TurnSystem.actStop = false;
		donePassive = false;
		doneActive = false;
		TurnSystem.actLocked = false;
		TurnSystem.passiveAction = true;
		TurnSystem.activeAction = false;
		if (CurrentTurnsRecharging > 0 && RechargeEnabled == true) {
			CurrentTurnsRecharging = 0;
		}
		//DidAReset = false;
		RechargeEnabled = true;
		TurnSystem.PlayersCompletePassive = false;
		TurnSystem.PlayersCompleteActive = false;
		playerAssignSystem.finishedPassive = false;
		playerAssignSystem.finishedActive = false;
		playerAssignSystem.DoingPassive = true;
		playerAssignSystem.DoingActive = false;
		BeenDoingSomething = false;
		BeenDoingSomethingActive = false;

		//Complete Passive = false
		//Complete Active = false
		//RechargeEnabled = true;
	}		
	IEnumerator SkipPassive() {
		CATSKIPPINGPASSIVE = false;
		if (StopIncrease == false) {
			stamina = stamina + 1;
		}
		StopIncrease = true;
		yield return new WaitForSeconds (2.0f);
		StopIncrease = false;
		donePassive = true;
		TurnSystem.Locked = false;
	}
	IEnumerator SkipActive() {
		CATSKIPPINGACTIVE = false;
		yield return new WaitForSeconds (2.0f);
		doneActive = true;
		reset = SwitchBack ();
		StartCoroutine (reset);
	}

	[Command] public void CmdChangeHex(string HexName, string nOnHex, GameObject nobjectOnhex) {
	   RpcChangeHex (HexName, nOnHex, nobjectOnhex);
	}

	[ClientRpc] public void RpcChangeHex(string HexName, string nOnHex, GameObject nobjectOnhex) {
		var realObjectOnHex = nobjectOnhex.transform;
		Hex hexthing = null;
		foreach (Transform hex in hexmap) {
			if (hex.name == HexName) {
				hexthing = hex.GetComponent<Hex> ();
			}
		}
		if (hexthing != null) {
			Debug.Log ("HexThing Is Detected!");
		}
		hexthing.OnHex = nOnHex;
		hexthing.objectOnhex = realObjectOnHex;

		//if (hexThing != null) {
		//	Hex realHex = hexThing.GetComponent<Hex> ();
		//} else {
		//	Debug.Log ("HexThing Is Set To Null!");
		//}
		//if (realHex != null) {
		//	realHex.OnHex = nOnHex;
		//	realHex.objectOnhex = realObjectOnHex;
		//} else {
		//	print ("Hex Is Set To Null!");
		//}
	}

	[Command] public void CmdRemoveHex(string HexName) {
		RpcRemoveHex (HexName);
	}

	[ClientRpc] public void RpcRemoveHex(string HexName) {
		Hex hexamajig = null;
		foreach (Transform hexo in hexmap) {
			if (hexo.name == HexName) {
				hexamajig = hexo.GetComponent<Hex> ();
			}
		}
		Debug.Log ("Removed Things From Hex!");
		hexamajig.OnHex = " ";
		hexamajig.objectOnhex = null;
	}

	//public void OnMouseDown() {
	//	Debug.Log ("A Cat Was Clicked");
	//}
}

//OVER 1200 LINES!