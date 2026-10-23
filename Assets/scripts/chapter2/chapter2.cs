
//using GLTFast.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
//using System.Collections;
using Unity.Mathematics;
public class chapter2 : MonoBehaviour
{
    public School school_script;
    walk playermovement;
    cam_rotation fps_Cam;
    public RayCast Raycast;
    bool hidden;
    [SerializeField]
    private AudioClip[] PainEffects;
    private bool alive, levelCheck = false,ClosedDoorCheck = false;
    private float jumpsave, PassCode;
    private float[] RandomGeneate = new float[4];
    public float Level2jump = 30f, jumpspeed = 0.8f;
    public GameObject Chaser;
    public TextMesh codepaperText;
    public TextMeshPro digitalInputText;
    public UIDocument uIDocument;
    private Label PaperPanel, PaperText;
    private Button ExitButton;
    private int CodeNumber,codeindex = 0;
    private int[] codes = new int[4];
    public Animator DoorAnimator;
    public Material Redlight, GreenLight;
    public AudioSource KeyPadLockAudioSource,DoorAudioSource,KeyAudioSource;
    public AudioClip correctpass, WrongPass,beep;
    private Animator KeysAnimator;
    
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        var root = uIDocument.rootVisualElement;
        playermovement = GetComponent<walk>();
        fps_Cam = GetComponentInChildren<cam_rotation>();
        Debug.Log("playermovement =" + playermovement.name);
        Debug.Log("fps_Cam =" + fps_Cam.name);
        alive = true;
        jumpsave = playermovement.jumpForce;
        PaperPanel = root.Q<Label>("PaperPanel");
        PaperText = root.Q<Label>("PaperText");
        ExitButton = root.Q<Button>("ExitButton");
        ExitButton.clicked += PaperExit;
        PaperPanel.style.display = DisplayStyle.None;
        if (Redlight.HasProperty("_EmissionColor"))
        {
            Redlight.SetColor(
                       "_EmissionColor",
                        Color.red
                   );
        }
        if (GreenLight.HasProperty("_EmissionColor"))
        {
            GreenLight.SetColor(
                       "_EmissionColor",
                        Color.black
                   );
        }
        DoorAnimator.SetBool("Close", true);
        // DoorAnimator.Play("Close");
        DoorAudioSource.enabled = false;
    }

    void Update()
    {
        RayCastOutput("");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("level1"))
        {
            Level1();
        }
        if (other.CompareTag("look"))
        {
            hidden = false;
            school_script.HiddenPlayer(hidden);
        }
        if (other.CompareTag("level2"))
        {
            Level2();
            Debug.Log(other.name);
        }
        if (other.CompareTag("level3"))
        {
            Level3();
            levelCheck = false;
            Debug.Log(other.name);
        }
        if(other.CompareTag("Close"))
        {
            if (!ClosedDoorCheck)
            {
                CloseDoor();
                ClosedDoorCheck = true;
            }
            other.gameObject.SetActive(false);

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("look"))
        {
            hidden = true;
            school_script.HiddenPlayer(hidden);
        }
        if (other.CompareTag("level2"))
        {
            if (transform.position.z > other.transform.position.z && !levelCheck)
            {
                Collider c = other.GetComponent<Collider>();
                c.isTrigger = false;
                levelCheck = true;
            }
            if (transform.position.z < other.transform.position.z)
            {
                school_script.enabled = true;
            }
        }
        if (other.CompareTag("level3"))
        {
            if (transform.position.z > other.transform.position.z && !levelCheck)
            {
                Collider c = other.GetComponent<Collider>();
                c.isTrigger = false;
                levelCheck = true;
            }
            if (transform.position.z < other.transform.position.z)
            {
                school_script.enabled = true;
            }
        }
    }
    public void Level1()
    {
        school_script.enabled = true;
    }
    public void Level2()
    {
        if (!Chaser.activeSelf)
        {
            Chaser.SetActive(true);
        }
        school_script.enabled = false;
        playermovement.jumpForce = Level2jump;
        playermovement.character_ani.SetFloat("jumpspeed", jumpspeed);
        //levelCheck = true;
    }
    public void Level3()
    {
        playermovement.jumpForce = jumpsave;
        playermovement.thirdperson = false;
        playermovement._first_person();
        fps_Cam.enabled = true;
        playermovement.character_ani.SetFloat("jumpspeed", 1f);
        if (PassCode * 1000 == 0)
        {
            for (int x = 0; x < 4; x++)
            {
                int y = UnityEngine.Random.Range(1, 11);
                RandomGeneate[x] = y;

            }
            if (PassCode < 10000)
            {
                PassCode = RandomGeneate[0] + (RandomGeneate[1] * 10) + (RandomGeneate[2] * 100) + (RandomGeneate[3] * 1000);
                codepaperText.text += PassCode;
            }
            else
            {
                PassCode = PassCode / 1000;
                PassCode = math.round(PassCode);
                codepaperText.text += PassCode;
            }
            Debug.Log(PassCode);
        }
    }

    public void GameOver()
    {
        playermovement.character_ani.enabled = false;
        playermovement.WalkAudioSource.enabled = false;
        playermovement.RunAudioSource.enabled = false;
        playermovement.enabled = false;
        fps_Cam.enabled = false;
        //school_script.enabled = false;
        if (alive)
        {
            SoundFxManeger.instance.playsoundFX(PainEffects, transform, 1f);
            Debug.Log("playing sound");
            alive = false;
        }
        enabled = false;
    }
    public void RayCastOutput(string Name)
    {
        
        if(Raycast.HitName != null)
            Name = Raycast.HitName;
        switch (Name)
        {
            case "Paper1":
                PaperPanel.style.display = DisplayStyle.Flex;
                PaperText.text = "Code: " + PassCode;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                playermovement.ani.SetBool("walk", false);
                playermovement.WalkAudioSource.enabled = false;
                playermovement.RunAudioSource.enabled = false;
                playermovement.enabled = false;
                fps_Cam.enabled = false;
                Raycast.HitName = null;
                //Name = null;
                break;

        }
         if (Name.StartsWith("Key") && Raycast.HitName != null)
        {
            if (Raycast.hit.collider != null)
            {
                KeysAnimator = Raycast.hit.collider.GetComponent<Animator>();
            }
            if (KeysAnimator != null)
            {
                KeysAnimator.speed = 2f;
                KeysAnimator.Play("KeyClicked");
            }
            KeyAudioSource.clip = beep;
            KeyAudioSource.Play();
            if (Name == "Key_Enter")
            {
                if (codeindex == codes.Length && CodeNumber == PassCode)
                {
                    OpenTheDoor();
                    Raycast.HitName = null;
                }
                else
                {
                    KeyPadLockAudioSource.clip = WrongPass;
                    KeyPadLockAudioSource.Play();
                    CodeNumber = 0;
                    codeindex = 0;

                    System.Array.Clear(codes, 0, codes.Length);
                    digitalInputText.text = "";
                    if (Redlight.HasProperty("_EmissionColor"))
                    {
                        Redlight.SetColor(
                                   "_EmissionColor",
                                    Color.red
                               );
                    }
                    if (GreenLight.HasProperty("_EmissionColor"))
                    {
                        GreenLight.SetColor(
                                   "_EmissionColor",
                                    Color.black
                               );
                    }
                    CloseDoor();
                }
                Raycast.HitName = null;
                return;
            }

            if (Name == "Key_x")
            {
                CodeNumber = 0;
                codeindex = 0;

                System.Array.Clear(codes, 0, codes.Length);
                digitalInputText.text = "";

                Raycast.HitName = null;
                return;
            }

            if (int.TryParse(Name.Substring(4), out int x))
            {
                if (codeindex < codes.Length)
                {
                    codes[codeindex] = x;
                    digitalInputText.text += x;
                    codeindex++;

                    CodeNumber = 0;

                    for (int i = 0; i < codeindex; i++)
                    {
                        CodeNumber = CodeNumber * 10 + codes[i];
                    }

                    Debug.Log("Entered digits: " + digitalInputText.text);
                }

                Raycast.HitName = null;
            }
        }
    }
    public void OpenTheDoor()
    {
        DoorAudioSource.enabled = true;
        DoorAnimator.SetBool("Close",false);
        if (Redlight.HasProperty("_EmissionColor"))
        {
            Redlight.SetColor(
                       "_EmissionColor",
                        Color.black
                   );
        }
        if (GreenLight.HasProperty("_EmissionColor"))
        {
            GreenLight.SetColor(
                       "_EmissionColor",
                        Color.green
                   );
        }
        KeyPadLockAudioSource.clip = correctpass;
        KeyPadLockAudioSource.Play();
        
    }
    public void CloseDoor()
    {
        DoorAnimator.SetBool("Close",true);
        if(DoorAudioSource.isActiveAndEnabled)
        { 
        DoorAudioSource.Play();
            //DoorAudioSource.enabled = false;
            }
    }
    public void PaperExit()
    {
        PaperPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        playermovement.enabled = true;
        fps_Cam.enabled = true;
        Debug.Log("Exit");

    }
}
