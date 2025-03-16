using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    
    private const string ResourceMapPath = "ResourcesMap"; // Убираем .txt, так как Resources.Load не поддерживает расширения
    
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    private Dictionary<string, UIResource> _uiResourceMap = new Dictionary<string, UIResource>();
    private Dictionary<string, GameObject> _activeUI = new Dictionary<string, GameObject>(); // Для хранения активных UI

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUIResourceMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadUIResourceMap()
    {
        // Загружаем файл из папки Resources
        TextAsset jsonFile = Resources.Load<TextAsset>(ResourceMapPath);
        if (jsonFile == null)
        {
            Debug.LogError("UI Resource Map not found in Resources!");
            return;
        }

        try
        {
            _uiResourceMap = JsonConvert.DeserializeObject<Dictionary<string, UIResource>>(jsonFile.text);
            Debug.Log("UI Resource Map loaded successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($" Error parsing UI Resource Map: {ex.Message}");
        }
    }

    public GameObject ShowUI(string uiKey)
    {
        if (!_uiResourceMap.ContainsKey(uiKey))
        {
            Debug.LogError($"⚠ UI key '{uiKey}' not found in UI Resource Map.");
            return null;
        }

        UIResource resource = _uiResourceMap[uiKey];

        Debug.Log($"🔍 Trying to load UI prefab at path: {resource.Path}");

        // Загружаем префаб
        GameObject prefab = Resources.Load<GameObject>(resource.Path);
        if (prefab == null)
        {
            Debug.LogError($"❌ UI Prefab not found at path: {resource.Path}");
            return null;
        }

        // Создаём UI-объект
        GameObject instance = Instantiate(prefab, _canvas.transform, false);
        _activeUI[uiKey] = instance;
        return instance;
    }

    public void HideUI(string uiKey)
    {
        if (_activeUI.ContainsKey(uiKey))
        {
            Destroy(_activeUI[uiKey]);
            _activeUI.Remove(uiKey);
        }
    }

    private void Start()
    {
        // Показываем UI при старте (например, "NoAdsOffer80Window")
        ShowUI("NoAdsOffer80Window");
    }
}
