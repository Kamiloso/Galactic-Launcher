using GalacticLauncher.Frontend.Repositories;
using System;

namespace GalacticLauncher.Frontend.Services.Data;

public interface IPreferenceManager
{
    bool IsThemeGalactic { get; set; }
    bool IsMenuExpanded { get; set; }
    bool IsAdminPanelVisible { get; set; }
    string LastUsername { get; set; }
    Guid Guid { get; set; }

    long? GetSelectedVersion(long gameId);
    void SetSelectedVersion(long gameId, long? versionId);

    bool GetGameBool(long gameId, string filterName, bool defaultValue);
    void SetGameBool(long gameId, string filterName, bool value);
}

internal class PreferenceManager : IPreferenceManager
{
    private const string MKEY_THEME = "galactic";
    private const string MKEY_EXPANDED = "expanded";
    private const string MKEY_ADMIN_PANEL = "admin-panel";
    private const string MKEY_USERNAME = "username";
    private const string MKEY_GUID = "guid";
    private static string MKEY_SEL_VERSION(long id) => $"sel-version-{id}";
    private static string MKEY_FILTER(long gameId, string name) => $"filter-{gameId}-{name}";

    private static bool DefaultThemeGalactic => true;
    private static bool DefaultMenuExpanded => true;
    private static bool DefaultAdminPanelVisible => false;

    private readonly IMemoryRepository _memoryRepository;

    public PreferenceManager(IMemoryRepository memoryRepository)
    {
        _memoryRepository = memoryRepository;

        if (Guid == default)
            Guid = Guid.NewGuid(); // single guid per launcher copy
    }

    private const string GALACTIC = "galactic";
    private const string BLUE = "blue";
    public bool IsThemeGalactic
    {
        set => _memoryRepository[MKEY_THEME] = value ? GALACTIC : BLUE;
        get => _memoryRepository[MKEY_THEME] switch
        {
            GALACTIC => true,
            BLUE => false,
            _ => DefaultThemeGalactic
        };
    }

    private const string EXPANDED = "expanded";
    private const string SHRINKED = "shrinked";
    public bool IsMenuExpanded
    {
        set => _memoryRepository[MKEY_EXPANDED] = value ? EXPANDED : SHRINKED;
        get => _memoryRepository[MKEY_EXPANDED] switch
        {
            EXPANDED => true,
            SHRINKED => false,
            _ => DefaultMenuExpanded
        };
    }

    private const string VISIBLE = "visible";
    private const string HIDDEN = "hidden";
    public bool IsAdminPanelVisible
    {
        set => _memoryRepository[MKEY_ADMIN_PANEL] = value ? VISIBLE : HIDDEN;
        get => _memoryRepository[MKEY_ADMIN_PANEL] switch
        {
            VISIBLE => true,
            HIDDEN => false,
            _ => DefaultAdminPanelVisible
        };
    }

    public string LastUsername
    {
        get => _memoryRepository[MKEY_USERNAME];
        set => _memoryRepository[MKEY_USERNAME] = value;
    }

    public Guid Guid
    {
        get => Guid.TryParse(_memoryRepository[MKEY_GUID], out var guid) ? guid : default;
        set => _memoryRepository[MKEY_GUID] = value.ToString();
    }

    public long? GetSelectedVersion(long gameId)
    {
        return long.TryParse(
            _memoryRepository[MKEY_SEL_VERSION(gameId)], out var value)
                ? value
                : null;
    }

    public void SetSelectedVersion(long gameId, long? versionId)
    {
        _memoryRepository[MKEY_SEL_VERSION(gameId)] = versionId.HasValue
            ? versionId.Value.ToString()
            : "";
    }

    public bool GetGameBool(long gameId, string filterName, bool defaultValue)
    {
        return bool.TryParse(
            _memoryRepository[MKEY_FILTER(gameId, filterName)], out var value)
                ? value
                : defaultValue;
    }

    public void SetGameBool(long gameId, string filterName, bool value)
    {
        _memoryRepository[MKEY_FILTER(gameId, filterName)] = value.ToString();
    }
}
