namespace CrmTechTitans.ViewModels
{
    public class MemberExportOptions
    {
        public List<int>? SelectedMemberIds { get; set; } // IDs of selected members
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all members
    }
}
