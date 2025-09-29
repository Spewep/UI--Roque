using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UXMLs")]
    public VisualTreeAsset pauseMenuUXML;
    public VisualTreeAsset settingsUXML;
    public VisualTreeAsset creditsUXML;
    public VisualTreeAsset fadeOverlayUXML;

    private UIDocument uiDocument;
    private VisualElement fadeOverlay;
    private bool isPaused = false;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        // Carrega overlay separado
        fadeOverlay = fadeOverlayUXML.CloneTree();
        uiDocument.rootVisualElement.Add(fadeOverlay);

        // Inicializa menu de pause invisível
        LoadUIImmediate(pauseMenuUXML);
        SetGamePaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    private void ToggleMenu()
    {
        SetGamePaused(!isPaused);
    }

    private void SetGamePaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            FadeInOverlay();
            LoadUI(pauseMenuUXML);
        }
        else
        {
            FadeOutOverlay();
            FadeOut(uiDocument.rootVisualElement);
        }
    }

    // ---------- Overlay ----------
    private void FadeInOverlay(float targetOpacity = 0.5f, float duration = 0.3f)
    {
        fadeOverlay.style.display = DisplayStyle.Flex;
        LeanTween.value(gameObject, fadeOverlay.resolvedStyle.opacity, targetOpacity, duration)
            .setOnUpdate(val => fadeOverlay.style.opacity = val);
    }

    private void FadeOutOverlay(float duration = 0.3f)
    {
        LeanTween.value(gameObject, fadeOverlay.resolvedStyle.opacity, 0f, duration)
            .setOnUpdate(val => fadeOverlay.style.opacity = val)
            .setOnComplete(() => fadeOverlay.style.display = DisplayStyle.None);
    }

    // ---------- Carregamento de UI ----------
    private void LoadUI(VisualTreeAsset vta)
    {
        var root = uiDocument.rootVisualElement;

        // Fade out do menu atual se existir (ignora overlay)
        VisualElement currentMenu = root.Children().FirstOrDefault(e => e.style.display == DisplayStyle.Flex && e != fadeOverlay);
        if (currentMenu != null)
            FadeOut(currentMenu, () => LoadUIImmediate(vta));
        else
            LoadUIImmediate(vta);
    }

    private void LoadUIImmediate(VisualTreeAsset vta)
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

            FadeIn(root);
        }
        else if (vta == settingsUXML || vta == creditsUXML)
        {
            var backButton = root.Q<Button>("BackToMenu");
            backButton.clicked += () => LoadUI(pauseMenuUXML);
            SetupAnimatedButton(backButton, new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f));

            FadeIn(root);
        }
    }

    // ---------- Botões animados ----------
    private void SetupAnimatedButton(Button button, Color hoverColor, Color clickColor)
    {
        Color normalColor = Color.white;
        float animTime = 0.15f;
        float scaleAmount = 1.05f;

        // Hover entra
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

        // Hover sai
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

        // Clique
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

    private void FadeOut(VisualElement element, System.Action onComplete = null, float duration = 0.3f)
    {
        LeanTween.value(gameObject, element.resolvedStyle.opacity, 0f, duration)
            .setOnUpdate(val => element.style.opacity = val)
            .setOnComplete(() =>
            {
                element.style.display = DisplayStyle.None;
                onComplete?.Invoke();
            });
    }

    private void FadeIn(VisualElement element, float duration = 0.3f)
    {
        element.style.display = DisplayStyle.Flex;
        element.style.opacity = 0f;
        LeanTween.value(gameObject, 0f, 1f, duration)
            .setOnUpdate(val => element.style.opacity = val);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SceneManager.LoadScene("MainMenu");
#endif
    }
}
