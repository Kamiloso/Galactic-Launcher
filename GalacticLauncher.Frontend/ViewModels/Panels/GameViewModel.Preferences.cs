namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel
{
    private const string INS_SNAPSHOT = "ins-snapshot";
    private const string AVB_SNAPSHOT = "avb-snapshot";

    private const string INS_EXPANDED = "ins-expanded";
    private const string AVB_EXPANDED = "avb-expanded";

    private void InitializePreferences()
    {
        FilterInstalledSnapshot = _preferenceManager.GetGameBool(_id, INS_SNAPSHOT, true);
        FilterAvailableSnapshot = _preferenceManager.GetGameBool(_id, AVB_SNAPSHOT, false);

        IsInstalledSectionExpanded = _preferenceManager.GetGameBool(_id, INS_EXPANDED, true);
        IsAvailableSectionExpanded = _preferenceManager.GetGameBool(_id, AVB_EXPANDED, true);
    }

    partial void OnFilterInstalledSnapshotChanged(bool value)
    {
        _preferenceManager.SetGameBool(_id, INS_SNAPSHOT, value);

        RefreshListsAndSelection();
    }

    partial void OnFilterAvailableSnapshotChanged(bool value)
    {
        _preferenceManager.SetGameBool(_id, AVB_SNAPSHOT, value);

        RefreshListsAndSelection();
    }

    partial void OnIsInstalledSectionExpandedChanged(bool value)
    {
        _preferenceManager.SetGameBool(_id, INS_EXPANDED, value);
    }

    partial void OnIsAvailableSectionExpandedChanged(bool value)
    {
        _preferenceManager.SetGameBool(_id, AVB_EXPANDED, value);
    }
}
