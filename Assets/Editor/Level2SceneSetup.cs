using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Level 2 sahne kurulum aracı - DÜZELTILMIŞ VERSİYON
/// - Emoji karakterleri kaldırıldı
/// - Işık sayısı azaltıldı
/// - Kamera hızı artırıldı
/// - Duplicate UI kontrolü eklendi
/// </summary>
public class Level2SceneSetup : EditorWindow
{
    private bool createCanvas = true;
    private bool createGameManager = true;
    private bool setupCamera = true;
    private bool addTowers = true;
    private bool addLighting = true;

    // Renkler
    private Color zombieBarColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    private Color waveBarColor = new Color(0.2f, 0.5f, 0.9f, 1f);
    private Color baseBarColor = new Color(0.2f, 0.8f, 0.3f, 1f);

    [MenuItem("Tools/Level 2 Scene Setup (Duzeltilmis)")]
    static void Init()
    {
        Level2SceneSetup window = GetWindow<Level2SceneSetup>("Level 2 Setup v3");
        window.minSize = new Vector2(420, 600);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Level 2 Kurulum (DUZELTILMIS)", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "DUZELTMELER:\n" +
            "- Emoji karakterleri kaldirildi\n" +
            "- Isik sayisi azaltildi (max 10)\n" +
            "- Kamera 2.5x hizlandi (200 speed)\n" +
            "- Duplicate UI kontrolu eklendi\n" +
            "- Start Wave butonu duzeltildi",
            MessageType.Info
        );

        GUILayout.Space(15);

        EditorGUILayout.LabelField("Kurulum Secenekleri", EditorStyles.boldLabel);
        createCanvas = EditorGUILayout.Toggle("Canvas & UI", createCanvas);
        createGameManager = EditorGUILayout.Toggle("GameManager", createGameManager);
        setupCamera = EditorGUILayout.Toggle("Hizli Kamera (2.5x)", setupCamera);
        addTowers = EditorGUILayout.Toggle("Kuleleri Getir", addTowers);
        addLighting = EditorGUILayout.Toggle("Isiklandirma (az)", addLighting);

        GUILayout.Space(20);

        // TEMIZLE butonu
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("ONCE ESKI UI'YI TEMIZLE", GUILayout.Height(35)))
        {
            CleanupAllUI();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("TUM KURULUMU YAP", GUILayout.Height(50)))
        {
            RunFullSetup();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Tek Tek Kurulum", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. UI Olustur", GUILayout.Height(30)))
        {
            CleanupAllUI();
            CreateNewUI();
        }
        
        if (GUILayout.Button("2. GameManager", GUILayout.Height(30)))
        {
            CreateGameManager();
        }
        
        if (GUILayout.Button("3. Kamera (HIZLI - 200)", GUILayout.Height(30)))
        {
            SetupFastCamera();
        }

        if (GUILayout.Button("4. Isiklari Temizle", GUILayout.Height(30)))
        {
            CleanupLights();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ctrl+S ile kaydedin!", EditorStyles.miniLabel);
    }

    void CleanupAllUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Tum panelleri temizle - DAHA AGRESIF
        string[] panelsToRemove = { 
            "TopBar", "ScorePanel", "WavePanel", "BaseHealthPanel", 
            "CoinPanel", "GameOverPanel", "VictoryPanel", "WaveCompletedPanel",
            "CoinButton", "Panel_WaveUI" // Eski Level 1 UI'lari da temizle
        };

        // Birden fazla kez calistir (duplicate'ler icin)
        for (int pass = 0; pass < 3; pass++)
        {
            Transform[] allChildren = canvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t == null || t == canvas.transform) continue;
                
                foreach (string panelName in panelsToRemove)
                {
                    if (t.name == panelName || t.name.Contains(panelName))
                    {
                        Undo.DestroyObjectImmediate(t.gameObject);
                        break;
                    }
                }
            }
        }

        Debug.Log("[Setup] Eski UI temizlendi (3 pass)!");
    }

    void CleanupLights()
    {
        GameObject lightsContainer = GameObject.Find("StreetLights");
        if (lightsContainer != null)
        {
            Undo.DestroyObjectImmediate(lightsContainer);
            Debug.Log("[Setup] Isiklar temizlendi!");
        }
    }

    void RunFullSetup()
    {
        CleanupAllUI();
        CleanupLights();
        
        if (createCanvas) CreateNewUI();
        if (createGameManager) CreateGameManager();
        if (setupCamera) SetupFastCamera();
        if (addTowers) ImportTowers();
        if (addLighting) AddFewLights();
        
        EditorUtility.DisplayDialog("Kurulum Tamamlandi!", 
            "Tum kurulum yapildi!\n\n" +
            "- UI olusturuldu\n" +
            "- Kamera hizi: 200\n" +
            "- Isik sayisi: max 10\n\n" +
            "Ctrl+S ile kaydedin!", 
            "Tamam");
    }

    void CreateNewUI()
    {
        Canvas existingCanvas = FindFirstObjectByType<Canvas>();
        GameObject canvasObj;
        
        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
        }
        else
        {
            canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
        }

        // SCORE (Sol Alt) - Emoji YOK
        GameObject scorePanel = CreatePanel(canvasObj.transform, "ScorePanel",
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(100, 30), new Vector2(160, 50));
        CreateText(scorePanel.transform, "ScoreText", "Score: 0",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 24, Color.yellow);

        // WAVE PROGRESS (Ust Orta - Mavi)
        GameObject wavePanel = CreatePanel(canvasObj.transform, "WavePanel",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -25), new Vector2(350, 45));
        CreateSlider(wavePanel.transform, "WaveProgressSlider", waveBarColor,
            new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, new Vector2(300, 16));
        CreateText(wavePanel.transform, "WaveText", "Wave 1/5",
            new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), Vector2.zero, 14, Color.white);

        // BASE HEALTH (Sag Ust - Yesil)
        GameObject basePanel = CreatePanel(canvasObj.transform, "BaseHealthPanel",
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-100, -25), new Vector2(180, 45));
        CreateText(basePanel.transform, "BaseLabel", "Base HP",
            new Vector2(0.2f, 0.7f), new Vector2(0.2f, 0.7f), Vector2.zero, 12, Color.white);
        CreateSlider(basePanel.transform, "BaseHealthSlider", baseBarColor,
            new Vector2(0.55f, 0.4f), new Vector2(0.55f, 0.4f), Vector2.zero, new Vector2(100, 14));

        // COIN (Sag Ust kose)
        GameObject coinPanel = CreatePanel(canvasObj.transform, "CoinPanel",
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-50, -75), new Vector2(90, 30));
        CreateText(coinPanel.transform, "CoinText", "500",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 18, Color.yellow);

        // GAME OVER PANEL
        GameObject gameOverPanel = CreateDarkPanel(canvasObj.transform, "GameOverPanel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(450, 300));
        gameOverPanel.SetActive(false);
        CreateText(gameOverPanel.transform, "GameOverText", "YOU LOST!",
            new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), Vector2.zero, 42, Color.red);
        CreateText(gameOverPanel.transform, "FinalScoreText", "Score: 0",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 28, Color.yellow);
        CreateButton(gameOverPanel.transform, "RestartButton", "RESTART",
            new Vector2(0.3f, 0.2f), new Vector2(0.3f, 0.2f), Vector2.zero, new Vector2(120, 40), new Color(0.2f, 0.6f, 0.2f));
        CreateButton(gameOverPanel.transform, "MenuButton", "MENU",
            new Vector2(0.7f, 0.2f), new Vector2(0.7f, 0.2f), Vector2.zero, new Vector2(120, 40), new Color(0.6f, 0.2f, 0.2f));

        // VICTORY PANEL
        GameObject victoryPanel = CreateDarkPanel(canvasObj.transform, "VictoryPanel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(450, 300));
        victoryPanel.SetActive(false);
        CreateText(victoryPanel.transform, "VictoryText", "LEVEL COMPLETE!",
            new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), Vector2.zero, 38, Color.green);
        CreateText(victoryPanel.transform, "VictoryScoreText", "Score: 0",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 28, Color.yellow);
        CreateButton(victoryPanel.transform, "NextLevelButton", "NEXT LEVEL",
            new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), Vector2.zero, new Vector2(160, 45), new Color(0.2f, 0.5f, 0.8f));

        // WAVE COMPLETED PANEL
        GameObject waveCompPanel = CreateDarkPanel(canvasObj.transform, "WaveCompletedPanel",
            new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, new Vector2(300, 110));
        waveCompPanel.SetActive(false);
        CreateText(waveCompPanel.transform, "WaveCompletedText", "Wave Completed!",
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, 24, Color.green);
        CreateButton(waveCompPanel.transform, "StartWaveButton", "START NEXT WAVE",
            new Vector2(0.5f, 0.3f), new Vector2(0.5f, 0.3f), Vector2.zero, new Vector2(180, 35), new Color(0.3f, 0.6f, 0.3f));

        Debug.Log("[Setup] UI olusturuldu!");
    }

    void CreateGameManager()
    {
        Level2GameManager existing = FindFirstObjectByType<Level2GameManager>();
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        // Mevcut CoinManager varsa koru
        CoinManager existingCoin = FindFirstObjectByType<CoinManager>();

        GameObject gmObj = new GameObject("=== GAME MANAGER ===");
        Undo.RegisterCreatedObjectUndo(gmObj, "Create GameManager");

        Level2GameManager gm = gmObj.AddComponent<Level2GameManager>();
        BaseHealth baseHealth = gmObj.AddComponent<BaseHealth>();
        ScoreManager scoreManager = gmObj.AddComponent<ScoreManager>();
        WaveProgressUI waveProgress = gmObj.AddComponent<WaveProgressUI>();
        
        // CoinManager ekle (yoksa)
        CoinManager coinManager = existingCoin;
        if (coinManager == null)
        {
            coinManager = gmObj.AddComponent<CoinManager>();
            coinManager.coins = 500;
        }

        gm.baseHealth = baseHealth;
        gm.scoreManager = scoreManager;
        gm.waveProgressUI = waveProgress;

        Level2EnemySpawner spawner = FindFirstObjectByType<Level2EnemySpawner>();
        if (spawner != null)
        {
            gm.spawner = spawner;
            spawner.waveProgressUI = waveProgress;
        }

        ConnectUIReferences(baseHealth, scoreManager, waveProgress, spawner, coinManager);
        Debug.Log("[Setup] GameManager + CoinManager olusturuldu!");
    }

    void ConnectUIReferences(BaseHealth baseHealth, ScoreManager scoreManager, WaveProgressUI waveProgress, Level2EnemySpawner spawner, CoinManager coinManager)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
         if (canvas == null) return;

        // Score
        TextMeshProUGUI scoreText = FindUIElement<TextMeshProUGUI>(canvas.transform, "ScoreText");
        if (scoreText != null) scoreManager.scoreText = scoreText;

        // CoinManager - CoinText baglantisi
        if (coinManager != null)
        {
            TextMeshProUGUI coinText = FindUIElement<TextMeshProUGUI>(canvas.transform, "CoinText");
            if (coinText != null)
            {
                coinManager.coinText = coinText;
                coinText.text = coinManager.coins.ToString();
            }
        }

        // Base Health
        Slider baseSlider = FindUIElement<Slider>(canvas.transform, "BaseHealthSlider");
        if (baseSlider != null)
        {
            baseHealth.baseHealthSlider = baseSlider;
            baseHealth.sliderFillImage = baseSlider.fillRect?.GetComponent<Image>();
        }

        // Wave Progress
        Slider waveSlider = FindUIElement<Slider>(canvas.transform, "WaveProgressSlider");
        if (waveSlider != null)
        {
            waveProgress.waveProgressSlider = waveSlider;
            waveProgress.sliderFillImage = waveSlider.fillRect?.GetComponent<Image>();
        }

        TextMeshProUGUI waveText = FindUIElement<TextMeshProUGUI>(canvas.transform, "WaveText");
        if (waveText != null) waveProgress.waveText = waveText;

        // Game Over Panel + Buttons
        Transform gameOverPanel = FindChildRecursive(canvas.transform, "GameOverPanel");
        if (gameOverPanel != null)
        {
            baseHealth.gameOverPanel = gameOverPanel.gameObject;
            baseHealth.gameOverText = FindUIElement<TextMeshProUGUI>(gameOverPanel, "GameOverText");
            baseHealth.finalScoreText = FindUIElement<TextMeshProUGUI>(gameOverPanel, "FinalScoreText");
            
            // Restart button
            Button restartBtn = FindUIElement<Button>(gameOverPanel, "RestartButton");
            if (restartBtn != null)
            {
                restartBtn.onClick.RemoveAllListeners();
                restartBtn.onClick.AddListener(() => baseHealth.RestartGame());
            }
            
            // Menu button
            Button menuBtn = FindUIElement<Button>(gameOverPanel, "MenuButton");
            if (menuBtn != null)
            {
                menuBtn.onClick.RemoveAllListeners();
                menuBtn.onClick.AddListener(() => baseHealth.GoToMainMenu());
            }
        }

        // Victory Panel + Button
        Transform victoryPanel = FindChildRecursive(canvas.transform, "VictoryPanel");
        if (victoryPanel != null)
        {
            waveProgress.victoryPanel = victoryPanel.gameObject;
            waveProgress.victoryText = FindUIElement<TextMeshProUGUI>(victoryPanel, "VictoryText");
            waveProgress.victoryScoreText = FindUIElement<TextMeshProUGUI>(victoryPanel, "VictoryScoreText");
            
            // Next Level button
            Button nextBtn = FindUIElement<Button>(victoryPanel, "NextLevelButton");
            if (nextBtn != null)
            {
                nextBtn.onClick.RemoveAllListeners();
                nextBtn.onClick.AddListener(() => waveProgress.LoadNextLevel());
            }
        }

        // Wave Completed + Start Button (ONEMLI!)
        if (spawner != null)
        {
            Transform waveCompPanel = FindChildRecursive(canvas.transform, "WaveCompletedPanel");
            if (waveCompPanel != null)
            {
                spawner.waveUIPanel = waveCompPanel.gameObject;
                spawner.waveCompletedText = FindUIElement<TextMeshProUGUI>(waveCompPanel, "WaveCompletedText");
                
                Button startBtn = FindUIElement<Button>(waveCompPanel, "StartWaveButton");
                if (startBtn != null)
                {
                    spawner.startWaveButton = startBtn;
                    // Listener'i runtime'da ekleyecek, editor'da calismaz
                    Debug.Log("[Setup] StartWaveButton referansi atandi!");
                }
            }
            else
            {
                Debug.LogWarning("[Setup] WaveCompletedPanel bulunamadi!");
            }
        }
    }

    void SetupFastCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        CameraController oldController = mainCam.GetComponent<CameraController>();
        if (oldController != null)
        {
            Undo.DestroyObjectImmediate(oldController);
        }

        Level2CameraController controller = mainCam.GetComponent<Level2CameraController>();
        if (controller == null)
        {
            controller = Undo.AddComponent<Level2CameraController>(mainCam.gameObject);
        }

        Undo.RecordObject(controller, "Setup Fast Camera");
        
        // 2.5x HIZLI KAMERA
        controller.moveSpeed = 200f;          // 2.5x hizli (80 * 2.5)
        controller.moveSmoothTime = 0.08f;    // Daha responsive
        controller.heightSpeed = 120f;        // Hizli yukseklik
        controller.zoomSpeed = 40f;
        controller.minHeight = 30f;           // Daha yuksekten
        controller.maxHeight = 150f;
        controller.enableEdgeScroll = true;
        controller.edgeScrollSize = 30f;

        // Transform duzelt - DAHA YUKSEKTEN BAK
        Undo.RecordObject(mainCam.transform, "Fix Camera");
        mainCam.transform.localScale = Vector3.one;
        
        Vector3 pos = mainCam.transform.position;
        pos.y = Mathf.Max(pos.y, 80f); // Minimum 80 yukseklik
        mainCam.transform.position = pos;
        mainCam.transform.rotation = Quaternion.Euler(55f, 0f, 0f); // Daha dik

        CalculateMapBounds(controller);
        Debug.Log("[Setup] Kamera hizlandirildi! Speed: 200");
    }

    void CalculateMapBounds(Level2CameraController controller)
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        Vector3 min = new Vector3(float.MaxValue, 0, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, 0, float.MinValue);
        
        foreach (Renderer r in allRenderers)
        {
            if (r.gameObject.name.Contains("Ground") || r.gameObject.name.Contains("Road"))
            {
                min = Vector3.Min(min, r.bounds.min);
                max = Vector3.Max(max, r.bounds.max);
            }
        }

        if (min.x < float.MaxValue)
        {
            controller.minX = min.x - 50f;
            controller.maxX = max.x + 50f;
            controller.minZ = min.z - 70f;
            controller.maxZ = max.z + 50f;
            controller.useBounds = true;
        }
    }

    void ImportTowers()
    {
        string[] guids = AssetDatabase.FindAssets("Tower1new t:Prefab");
        if (guids.Length == 0) return;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject towerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (towerPrefab == null) return;

        BuildMenu buildMenu = FindFirstObjectByType<BuildMenu>();
        if (buildMenu != null && buildMenu.towerPrefabs != null)
        {
            bool exists = false;
            foreach (var t in buildMenu.towerPrefabs)
            {
                if (t != null && t.name.Contains("Tower1new")) { exists = true; break; }
            }

            if (!exists)
            {
                Undo.RecordObject(buildMenu, "Add Tower");
                var newList = new System.Collections.Generic.List<GameObject>(buildMenu.towerPrefabs);
                newList.Add(towerPrefab);
                buildMenu.towerPrefabs = newList.ToArray();
            }
        }
        Debug.Log("[Setup] Kuleler hazirlandi!");
    }

    void AddFewLights()
    {
        CleanupLights();

        GameObject container = new GameObject("StreetLights");
        Undo.RegisterCreatedObjectUndo(container, "Create Lights");

        // Sadece 8 isik ekle (performans icin)
        Vector3[] positions = {
            new Vector3(-100, 8, 0),
            new Vector3(-50, 8, 0),
            new Vector3(0, 8, 0),
            new Vector3(-100, 8, -50),
            new Vector3(-50, 8, -50),
            new Vector3(0, 8, -50),
            new Vector3(-75, 8, -25),
            new Vector3(-25, 8, -25)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject lightObj = new GameObject($"Light_{i}");
            lightObj.transform.parent = container.transform;
            lightObj.transform.position = positions[i];
            
            Light pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = new Color(1f, 0.9f, 0.7f);
            pointLight.intensity = 1.5f;
            pointLight.range = 20f;
            pointLight.shadows = LightShadows.None; // Golge yok - performans
        }

        Debug.Log($"[Setup] {positions.Length} isik eklendi!");
    }

    // === HELPER METHODS ===

    GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);
        
        Undo.RegisterCreatedObjectUndo(panel, "Create Panel");
        return panel;
    }

    GameObject CreateDarkPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject panel = CreatePanel(parent, name, anchorMin, anchorMax, position, size);
        panel.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.95f);
        return panel;
    }

    GameObject CreateText(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 40);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.fontStyle = FontStyles.Bold;
        
        Undo.RegisterCreatedObjectUndo(textObj, "Create Text");
        return textObj;
    }

    GameObject CreateSlider(Transform parent, string name, Color fillColor, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        RectTransform rect = sliderObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.interactable = false;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        
        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.25f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.2f);
        fillAreaRect.anchorMax = new Vector2(1, 0.8f);
        fillAreaRect.offsetMin = new Vector2(2, 0);
        fillAreaRect.offsetMax = new Vector2(-2, 0);
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = fillColor;
        
        slider.fillRect = fillRect;
        
        Undo.RegisterCreatedObjectUndo(sliderObj, "Create Slider");
        return sliderObj;
    }

    GameObject CreateButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Color bgColor)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        
        Undo.RegisterCreatedObjectUndo(btnObj, "Create Button");
        return btnObj;
    }

    T FindUIElement<T>(Transform parent, string name) where T : Component
    {
        Transform found = FindChildRecursive(parent, name);
        return found != null ? found.GetComponent<T>() : null;
    }

    Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
