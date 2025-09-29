using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UXMLs")]
    public VisualTreeAsset pauseMenuUXML;
    public VisualTreeAsset settingsUXML;
    public VisualTreeAsset creditsUXML;

    private UIDocument uiDocument;
    private bool isPaused = false;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        // Carrega o menu de pause no início
        LoadUI(pauseMenuUXML);
        SetGamePaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        SetGamePaused(!isPaused);
    }

    private void SetGamePaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        uiDocument.rootVisualElement.style.display = paused ? DisplayStyle.Flex : DisplayStyle.None;

        if (paused)
        {
            LoadUI(pauseMenuUXML); // Sempre volta para o menu principal ao pausar
            UnityEngine.Cursor.lockState = CursorLockMode.None; // libera o mouse
            UnityEngine.Cursor.visible = true; // mostra
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; // trava (FPS style)
            UnityEngine.Cursor.visible = false; // esconde
        }
    }

    private void LoadUI(VisualTreeAsset vta)
    {
        uiDocument.visualTreeAsset = vta;
        var root = uiDocument.rootVisualElement;

        if (vta == pauseMenuUXML)
        {
            var resumeButton = root.Q<Button>("resumeButton");
            var settingsButton = root.Q<Button>("settingsButton");
            var creditsButton = root.Q<Button>("creditsButton");
            var quitButton = root.Q<Button>("quitButton");

            resumeButton.clicked += () => SetGamePaused(false);
            settingsButton.clicked += () => LoadUI(settingsUXML);
            creditsButton.clicked += () => LoadUI(creditsUXML);
            quitButton.clicked += OnQuitClicked;

            SetupAnimatedButton(resumeButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));
            SetupAnimatedButton(settingsButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));
            SetupAnimatedButton(creditsButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));
            SetupAnimatedButton(quitButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));
        }
        else if (vta == settingsUXML || vta == creditsUXML)
        {
            var backButton = root.Q<Button>("BackToMenu");
            backButton.clicked += () => LoadUI(pauseMenuUXML);

            SetupAnimatedButton(backButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));
        }
    }

    private void SetupAnimatedButton(Button button, Color hoverColor, Color clickColor)
    {
        Color normalColor = Color.white;
        float animTime = 0.15f;
        float scaleAmount = 1.05f;

        button.RegisterCallback<MouseEnterEvent>(evt =>
        {
            LeanTween.value(gameObject,
                (Color val) => button.style.backgroundColor = val,
                button.resolvedStyle.backgroundColor, hoverColor, animTime);

            LeanTween.value(gameObject, 0f, 1f, animTime).setOnUpdate((float t) =>
            {
                float scale = Mathf.Lerp(1f, scaleAmount, t);
                button.style.scale = new Scale(new Vector3(scale, scale, 1f));
            }).setEase(LeanTweenType.easeOutBack);
        });

        button.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            LeanTween.value(gameObject,
                (Color val) => button.style.backgroundColor = val,
                button.resolvedStyle.backgroundColor, normalColor, animTime);

            LeanTween.value(gameObject, 0f, 1f, animTime).setOnUpdate((float t) =>
            {
                float scale = Mathf.Lerp(scaleAmount, 1f, t);
                button.style.scale = new Scale(new Vector3(scale, scale, 1f));
            }).setEase(LeanTweenType.easeOutBack);
        });

        button.RegisterCallback<ClickEvent>(evt =>
        {
            LeanTween.value(gameObject,
                (Color val) => button.style.backgroundColor = val,
                button.resolvedStyle.backgroundColor, clickColor, animTime / 2)
            .setOnComplete(() =>
            {
                Color target = button.worldBound.Contains(evt.originalMousePosition) ? hoverColor : normalColor;
                LeanTween.value(gameObject,
                    (Color val) => button.style.backgroundColor = val,
                    button.resolvedStyle.backgroundColor, target, animTime);
            });
        });
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SceneManager.LoadScene("MainMenu");
        // ou Application.Quit();
#endif
    }
}