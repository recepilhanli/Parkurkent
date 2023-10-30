
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

    public static Pause Instance = null;

    public GameObject PuasedScreen;
    public GameObject TryAgainScreen;
    public GameObject FinishScreen;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Instance = this;
    }

    public void RestartScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        Instance = null; //just in case
        SceneManager.LoadScene("Beykent");
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        PuasedScreen.SetActive(false);
    }

    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
        Instance = null; //just in case
        SceneManager.LoadScene("Ayazaga");
    }

    public void Quit()
    {
        Application.Quit();


    }

    public void Finish()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.Instance.animator.SetBool("isVaulting", false);
        Player.Instance.animator.SetBool("isEdgeClimbing", false);
        Player.Instance.animator.SetBool("isWallRunning", false);
        Player.Instance.animator.SetBool("isWallRunning", false);
        Player.Instance.animator.SetBool("isSwinging", false);
        Player.Instance.animator.SetBool("isWallSliding", false);

        Time.timeScale = 0.5f;
        FinishScreen.SetActive(true);
    }

    public void KillPlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.Instance.animator.SetBool("isDead", true);

        Player.Instance.animator.SetBool("isVaulting", false);
        Player.Instance.animator.SetBool("isEdgeClimbing", false);
        Player.Instance.animator.SetBool("isWallRunning", false);
        Player.Instance.animator.SetBool("isWallRunning", false);
        Player.Instance.animator.SetBool("isSwinging", false);
        Player.Instance.animator.SetBool("isWallSliding", false);

        Player.Instance.EnabledMovement = false;
        Player.Instance.EnabledGravity = false;

        Player.Instance.controller.detectCollisions = false;
        Player.Instance.transform.position = Player.Instance.transform.position + Vector3.down * 0.85f;

        Time.timeScale = 0.25f;
        TryAgainScreen.SetActive(true);
    }


    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                PuasedScreen.SetActive(true);
            }
        }

    }


}
