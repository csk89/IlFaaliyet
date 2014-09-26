namespace Turkok.Core.Settings
{
    public interface IApplicationSettings
    {
        bool EnableCaching { get; }
        int IndexPageSize { get; }
        int PageSize { get; }
        string AnnouncementServerPath { get; }
        string AnnouncementAccessPath { get; }
        string DocumentsServerPath { get; }
        string DocumentsAccessPath { get; }
        string ProjectServerPath { get; }
        string ProjectAccessPath { get; }
        string TaskServerPath { get; }
        string TaskAccessPath { get; }
        string MeetingServerPath { get; }
        string MeetingAccessPath { get; }
    }
}