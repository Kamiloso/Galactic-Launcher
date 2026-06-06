using GalacticLauncher.Frontend.Repositories;

namespace GalacticLauncher.Frontend.Services;

public interface IPreferenceManager
{
    bool IsThemeGalactic { get; set; }
    bool IsMenuExpanded { get; set; }
    bool IsAdminPanelVisible { get; set; }
    string LastUsername { get; set; }

    long? GetSelectedVersion(long gameId);
    void SetSelectedVersion(long gameId, long? versionId);

    bool GetFilterState(long gameId, string filterName, bool defaultValue);
    void SetFilterState(long gameId, string filterName, bool value);
}

internal class PreferenceManager(
    IMemoryRepository memoryRepository) : IPreferenceManager
{
    private const string MKEY_THEME = "galactic";
    private const string MKEY_EXPANDED = "expanded";
    private const string MKEY_ADMIN_PANEL = "admin-panel";
    private const string MKEY_USERNAME = "username";
    private static string MKEY_SEL_VERSION(long id) => $"sel-version-{id}";
    private static string MKEY_FILTER(long gameId, string name) => $"filter-{gameId}-{name}";

    private const string GALACTIC = "galactic";
    private const string BLUE = "blue";
    private const string EXPANDED = "expanded";
    private const string SHRINKED = "shrinked";
    private const string VISIBLE = "visible";
    private const string HIDDEN = "hidden";

    private static bool DefaultThemeGalactic => true;
    private static bool DefaultMenuExpanded => true;
    private static bool DefaultAdminPanelVisible => false;

    public bool IsThemeGalactic
    {
        set => memoryRepository[MKEY_THEME] = value ? GALACTIC : BLUE;
        get => memoryRepository[MKEY_THEME] switch
        {
            GALACTIC => true,
            BLUE => false,
            _ => DefaultThemeGalactic
        };
    }

    public bool IsMenuExpanded
    {
        set => memoryRepository[MKEY_EXPANDED] = value ? EXPANDED : SHRINKED;
        get => memoryRepository[MKEY_EXPANDED] switch
        {
            EXPANDED => true,
            SHRINKED => false,
            _ => DefaultMenuExpanded
        };
    }

    public bool IsAdminPanelVisible
    {
        set => memoryRepository[MKEY_ADMIN_PANEL] = value ? VISIBLE : HIDDEN;
        get => memoryRepository[MKEY_ADMIN_PANEL] switch
        {
            VISIBLE => true,
            HIDDEN => false,
            _ => DefaultAdminPanelVisible
        };
    }

    public string LastUsername
    {
        get => memoryRepository[MKEY_USERNAME];
        set => memoryRepository[MKEY_USERNAME] = value;
    }

    public long? GetSelectedVersion(long gameId)
    {
        return long.TryParse(
            memoryRepository[MKEY_SEL_VERSION(gameId)], out var value)
                ? value
                : null;
    }

    public void SetSelectedVersion(long gameId, long? versionId)
    {
        memoryRepository[MKEY_SEL_VERSION(gameId)] = versionId.HasValue
            ? versionId.Value.ToString()
            : "";
    }

    public bool GetFilterState(long gameId, string filterName, bool defaultValue)
    {
        return bool.TryParse(
            memoryRepository[MKEY_FILTER(gameId, filterName)], out var value)
                ? value
                : defaultValue;
    }

    public void SetFilterState(long gameId, string filterName, bool value)
    {
        memoryRepository[MKEY_FILTER(gameId, filterName)] = value.ToString();
    }
}
