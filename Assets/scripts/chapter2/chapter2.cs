
using UnityEngine;
using UnityEngine.UIElements;
public class chapter2 : MonoBehaviour
{
    public School school_script;
    walk playermovement;
    cam_rotation fps_Cam;
    public RayCast Raycast;
    bool hidden;
    [SerializeField]
    private AudioClip[] PainEffects;
    private bool alive, levelCheck = false;
    private float jumpsave, PassCode;
    private float[] RandomGeneate = new float[4];
    public float Level2jump = 30f, jumpspeed = 0.8f;
    public GameObject Chaser;
    public TextMesh codepaperText;
    public UIDocument uIDocument;
    private Label PaperPanel, PaperText;
    private Button ExitButton;
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
                int y = Random.Range(1, 11);
                RandomGeneate[x] = y;

            }
            PassCode = RandomGeneate[0] + (RandomGeneate[1] * 10) + (RandomGeneate[2] * 100) + (RandomGeneate[3] * 1000);
            codepaperText.text += PassCode;
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
        Name = Raycast.HitName;
        switch (Name)
        {
            case "Paper1":
                PaperPanel.style.display = DisplayStyle.Flex;
                PaperText.text = "Code: "+PassCode;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                playermovement.enabled = false;
                Raycast.HitName = null;
                //Name = null;
                break;

        }
    }
    public void PaperExit()
    {
        PaperPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        playermovement.enabled = true;
        Debug.Log("Exit");
    }
}
