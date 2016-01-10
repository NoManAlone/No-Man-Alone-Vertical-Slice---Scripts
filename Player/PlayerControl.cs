using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour 
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	public int playerID;

    //Cameras
    GameObject[] playerCameras;
	public GameObject consoleCamera;
    GameObject unpowerAllButton;

	//PowerManager
	PowerManager powerManager;
    PhotonView gameManagerPhotonView;

	//Input Keys
	KeyCode left, right, up, A, D, W, E;

	//bools
	public bool grounded, movingAnimation, movingRight, movingLeft, jetpacking, usingConsole, teleportAnimation, invincible;
	public bool teleporterOverlap;
	public bool inControl = true;
	bool facingRight = true;

    public bool incapacitated;
	public bool reviving;

    //Physics Values
    float moveSpeed = 10, jetpackSpeed = 10;
	public GameObject interactObject;

    Vector2 enemyDirection;

    PlayerStats playerStats;
	
	Rigidbody2D playerRigidbody;
	
	Animator anim;
    PhotonView playerAnimPhotonView;
	HeartIconBehaviours heartIconBehaviours;
	JetpackBehaviours jetpackBehaviours;

    DialogBox dialogBox;

	public bool collidingWallLeft, collidingWallRight;

    Vector2 enemyHitPos;
	float enemyHitPosX;

    Collider2D[] colliderOverlaps;
    BoxCollider2D groundCheckCollider, wallCheckCollider;

    LayerMask physicalColliders;

    GameManager gameManager;
	GameMenuUI gameMenuUI;

	PlayerAudio playerAudio;
	AudioManager audioManager;

    public bool consoleTutorial = false;

	/*---------------------------------------------------- UPDATE & FIXED UPDATE ----------------------------------------------------*/

	void Update()
	{
		if(inControl && !gameMenuUI.menuOpen)
		{
			Controls();
		}

		else
		{
			movingRight = false;
			movingLeft = false;
            jetpacking = false;
        }

        CollisionChecks();
        Animation();
    }

	void FixedUpdate()
	{
		Movement();
	}

	/*---------------------------------------------------- ON TRIGGER ENTER & EXIT ----------------------------------------------------*/

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Health
        if (collider.gameObject.tag == "Health")
        {
            if (playerStats.health < playerStats.maxHealth)
            {
                collider.gameObject.SetActive(false);
                playerStats.AlterHealth(1);
				playerAudio.Health();
            }
        }

        //Power Expansion
        if(collider.gameObject.tag == "PowerExpansion")
        {
            collider.gameObject.SetActive(false);
            powerManager.EmptyOutPowerMeter();
            powerManager.meterLength++;
            powerManager.InstantiatePowerMeter();
            powerManager.AlterMaxPower(1);
			playerAudio.Power();
        }
    }

	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "Player Red" || collider.gameObject.tag == "Player Blue")
		{
			reviving = false;
		}
	}
    
	/*---------------------------------------------------- CONTROLS ----------------------------------------------------*/

    void Controls()
    {
        // Console control.
        if(Input.GetKeyDown(E))
        {
            if (usingConsole)
                DisableConsole();

            else
                EnableConsole();
        }

		// Movement controls.
		if(!usingConsole)
		{
			//Keyboard
			if(Input.GetKey(right) || Input.GetKey(D))
			{
				movingRight=true;
				movingLeft=false;          
            }
				
			else if(Input.GetKey(left) || Input.GetKey(A))
			{
				movingLeft=true;
				movingRight=false;
            }
				
			else 
			{
				movingLeft=false;
				movingRight=false;

				collidingWallRight = false;
                collidingWallLeft = false;
            }

            if (Input.GetKeyDown(up) || Input.GetKeyDown(W))
            {
                jetpacking = true;

				if(gameObject.tag == "Player Red")
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, true);
				else
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, false);

				jetpackBehaviours.CallSwitchJetpack(jetpacking);

                gameManagerPhotonView.RPC("AlterDepletionRate", PhotonTargets.AllBuffered, 3f); // NOTE: AlterDepletionRate must be called before SetRecharging.
                gameManagerPhotonView.RPC("SetRecharging", PhotonTargets.AllBuffered);
            }
            else if (jetpacking && ((Input.GetKeyUp(up) || Input.GetKeyUp(W))))
            {
                jetpacking = false;

				if(gameObject.tag == "Player Red")
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, true);
				else
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, false);

				jetpackBehaviours.CallSwitchJetpack(jetpacking);

                gameManagerPhotonView.RPC("AlterDepletionRate", PhotonTargets.AllBuffered, -3f); // NOTE: AlterDepletionRate must be called before SetRecharging.
                gameManagerPhotonView.RPC("SetRecharging", PhotonTargets.AllBuffered);
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
            }
            else if (jetpacking && powerManager.power <= 0)
            {
                jetpacking = false;

				if(gameObject.tag == "Player Red")
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, true);
				else
					gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, false);

				jetpackBehaviours.CallSwitchJetpack(jetpacking);

                gameManagerPhotonView.RPC("AlterDepletionRate", PhotonTargets.AllBuffered, -3f); // NOTE: AlterDepletionRate must be called before SetRecharging.
                gameManagerPhotonView.RPC("SetRecharging", PhotonTargets.AllBuffered);
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
            }
        }
	}

	/*---------------------------------------------------- MOVEMENT ----------------------------------------------------*/

	void Movement()
	{
        if (!usingConsole)
        {
            if (movingRight && !collidingWallRight)
            {
                playerRigidbody.velocity = new Vector2(moveSpeed, playerRigidbody.velocity.y);
                movingAnimation = true;
            }

            else if (movingLeft && !collidingWallLeft)
            {
                playerRigidbody.velocity = new Vector2(-moveSpeed, playerRigidbody.velocity.y);
                movingAnimation = true;
            }

            else
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                movingAnimation = false;
            }


            if (jetpacking)
			{             
                if (movingRight)
                {
                    playerRigidbody.velocity = new Vector2(jetpackSpeed, jetpackSpeed);
                }
                else if (movingLeft)
                {
                    playerRigidbody.velocity = new Vector2(-jetpackSpeed, jetpackSpeed);
                }
                else
                {
                    playerRigidbody.velocity = new Vector2(0, jetpackSpeed);
                }
            }

            //Limit Y Velocity
            if (playerRigidbody.velocity.y > 20)
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 20);
        }

        else
        {
            playerRigidbody.velocity = new Vector2(0, 0);
        }
	}

	/*---------------------------------------------------- COLLISION CHECKS ----------------------------------------------------*/

	void CollisionChecks()
    {
        //GROUNDED CHECK
        colliderOverlaps = Physics2D.OverlapAreaAll(new Vector2(groundCheckCollider.bounds.min.x, groundCheckCollider.bounds.min.y), 
            new Vector2(groundCheckCollider.bounds.max.x, wallCheckCollider.bounds.max.y), physicalColliders);

        if (colliderOverlaps.Length > 0)
            grounded = true;

        else
        {
            grounded = false;
            if (usingConsole)
                DisableConsole();
        }


        //WALL CHECK
        colliderOverlaps = Physics2D.OverlapAreaAll(new Vector2(wallCheckCollider.bounds.min.x, wallCheckCollider.bounds.min.y), new Vector2(wallCheckCollider.bounds.max.x, wallCheckCollider.bounds.max.y), physicalColliders);

        collidingWallRight = false;
        collidingWallLeft = false;

        if (colliderOverlaps.Length > 0)
        {
            foreach (Collider2D overlap in colliderOverlaps)
            {
                if (overlap.transform.position.x > transform.position.x)
                    collidingWallRight = true;

                if (overlap.transform.position.x < transform.position.x)
                    collidingWallLeft = true;
            }
        }
    }

	/*---------------------------------------------------- ANIMATION & FLIP SPRITE ----------------------------------------------------*/

	void Animation()
	{			
		anim.SetBool("Moving", movingAnimation);
		anim.SetBool("Grounded", grounded);
        anim.SetBool("UsingConsole", usingConsole);
        anim.SetBool("Teleporting", teleportAnimation);
        anim.SetBool("Incapacitated", incapacitated);
		
		if(movingRight && !facingRight)
			FlipSprite();
			
		else if(movingLeft && facingRight)
			FlipSprite();	
	}
	
	void FlipSprite()
	{
		// Switch the way the player is facing
		facingRight = !facingRight;
		
		// Multiply the player's animation x local scale by -1
		Vector3 scale = transform.FindChild("Player Animation").localScale;
		scale.x *= -1;
        transform.FindChild("Player Animation").localScale = scale;
    }

	/*---------------------------------------------------- KNOCKBACK ----------------------------------------------------*/

    public IEnumerator Knockback()
    {
        playerRigidbody.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
        inControl = false;

		if(jetpacking)
		{
			jetpacking = false;

			jetpackBehaviours.CallSwitchJetpack(jetpacking);

			if(gameObject.tag == "Player Red")
				gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, true);
			else
				gameManagerPhotonView.RPC("SetJetpackingIndicator", PhotonTargets.AllBuffered, jetpacking, false);
			
			gameManagerPhotonView.RPC("AlterDepletionRate", PhotonTargets.AllBuffered, -3f); // NOTE: AlterDepletionRate must be called before SetRecharging.
			gameManagerPhotonView.RPC("SetRecharging", PhotonTargets.AllBuffered);
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
		}

        playerStats.AlterHealth(-1);

        if (usingConsole)
            DisableConsole();

        StartCoroutine(InvincibleFlash());
        yield return new WaitForSeconds(.3f);

        if (playerStats.health > 0)
            inControl = true;
    }

	/*---------------------------------------------------- INVINCIBLE ----------------------------------------------------*/

    public IEnumerator InvincibleFlash()
    {
        SpriteRenderer sr = transform.FindChild("Player Animation").GetComponent<SpriteRenderer>();

        while(invincible)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(.07f);
            sr.enabled = true;
            yield return new WaitForSeconds(.07f);
        }
    }

	/*---------------------------------------------------- ENABLE CONSOLE ----------------------------------------------------*/

    void EnableConsole()
    {
        if(grounded)
        {
            foreach (GameObject camera in playerCameras)
                camera.SetActive(false);

            consoleCamera.SetActive(true);
            Cursor.visible = true;

            consoleCamera.GetComponent<ConsoleCameraControl>().FocusRoom(gameManager.myPlayerCurrentRoom);

            usingConsole = true;

			unpowerAllButton.GetComponent<Image>().enabled = true;
			unpowerAllButton.GetComponent<Button>().enabled = true;

            if(audioManager)
    			audioManager.ConsoleView();

            if (consoleTutorial)
                dialogBox.StartDialogBox("Enemy computer hacked successfully ... press e to access console control ...", 3);
        }
    }

	/*---------------------------------------------------- DISABLE CONSOLE ----------------------------------------------------*/

    void DisableConsole()
    {
        consoleCamera.SetActive(false);
        Cursor.visible = false;

        foreach (GameObject camera in playerCameras)
            camera.SetActive(true);

        usingConsole = false;

		unpowerAllButton.GetComponent<Image>().enabled = false;
		unpowerAllButton.GetComponent<Button>().enabled = false;

        if(audioManager)
		    audioManager.NormalView();

        if(consoleTutorial)
            dialogBox.EndDialogBox();
	}

	/*---------------------------------------------------- PLAYER INCAPACITATE ----------------------------------------------------*/

	public void PlayerIncapacitate()
	{
		incapacitated = true;

		if(gameManager.myPlayer.tag.Contains("Red"))
			gameManagerPhotonView.RPC("SetPlayersIncapacitated", PhotonTargets.AllBuffered, true, true);
		else
			gameManagerPhotonView.RPC("SetPlayersIncapacitated", PhotonTargets.AllBuffered, false, true);

        transform.FindChild("CollisionCheck").gameObject.layer = LayerMask.NameToLayer("PlayerNoCollision");
		inControl = false;
		movingAnimation = false;
		heartIconBehaviours.CallStartIconAnimate();
	}

	/*---------------------------------------------------- REVIVE PLAYER ----------------------------------------------------*/

	public IEnumerator RevivePlayer()
	{	
		reviving = true;

		float timer = 0f;

		heartIconBehaviours.CallSwitchReviving(true);

		while(reviving)
		{
			yield return null;
			timer += Time.deltaTime;
			if(timer > 2f)
			{
				reviving = false;
				heartIconBehaviours.CallEndIconAnimate();
				incapacitated = false;
				inControl = true;
				playerStats.AlterHealth(1);
                transform.FindChild("CollisionCheck").gameObject.layer = LayerMask.NameToLayer("PlayerCollision");

				if(gameManager.myPlayer.tag.Contains("Red"))
					gameManagerPhotonView.RPC("SetPlayersIncapacitated", PhotonTargets.AllBuffered, true, false);
				else
					gameManagerPhotonView.RPC("SetPlayersIncapacitated", PhotonTargets.AllBuffered, false, false);
            }
		}
		heartIconBehaviours.CallSwitchReviving(false);
    }

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

    public void Initialisation()
    {
        //Set manager script references
        if (GameObject.Find("GameManager") != null)
        {
            gameManagerPhotonView = GameObject.Find("GameManager").GetComponent<PhotonView>();
            powerManager = GameObject.Find("GameManager").GetComponent<PowerManager>();
        }

        else
            Debug.LogWarning("Could not find GameManager! Is there a GameManager in the scene?");

        gameMenuUI = GameObject.Find("GameMenuUI").GetComponent<GameMenuUI>();

        consoleCamera = transform.FindChild("ConsoleCamera").gameObject;
        groundCheckCollider = transform.FindChild("GroundCheck").GetComponent<BoxCollider2D>();
        wallCheckCollider = transform.FindChild("WallCheck").GetComponent<BoxCollider2D>();
        unpowerAllButton = GameObject.Find("Unpower All Button");

        physicalColliders = LayerMask.GetMask("Wall", "Door", "Platform", "InteriorDoor");

        //Initialise Player Stats
        playerStats = GetComponent<PlayerStats>();

        //Initialises Animator and Colliders.
        anim = transform.FindChild("Player Animation").GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody2D>();

        playerAnimPhotonView = transform.FindChild("Player Animation").GetComponent<PhotonView>();
        heartIconBehaviours = transform.FindChild("Player Animation").GetComponent<HeartIconBehaviours>();
        jetpackBehaviours = transform.FindChild("Player Animation").GetComponent<JetpackBehaviours>();
        print(playerAnimPhotonView.name + heartIconBehaviours.name + jetpackBehaviours.name);

        unpowerAllButton.GetComponent<Image>().enabled = false;
        unpowerAllButton.GetComponent<Button>().enabled = false;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Cursor.visible = false;
        GameObject.Find("GameManager").GetComponent<MouseBehaviours>().enabled = true;

        left = KeyCode.LeftArrow;
        right = KeyCode.RightArrow;
        up = KeyCode.UpArrow;
        W = KeyCode.W;
        A = KeyCode.A;
        D = KeyCode.D;
        E = KeyCode.E;

        playerCameras = new GameObject[3];
        for (byte counter = 0; counter < playerCameras.Length; counter++)
            playerCameras[counter] = transform.FindChild("Cameras").GetChild(counter).gameObject;

        playerAudio = GetComponent<PlayerAudio>();

        if (GameObject.Find("AudioManager"))
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        dialogBox = GameObject.Find("DialogBoxText").GetComponent<DialogBox>();
    }
}