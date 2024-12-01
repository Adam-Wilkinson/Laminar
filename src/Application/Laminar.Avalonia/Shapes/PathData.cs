using Avalonia.Media;

namespace Laminar.Avalonia.Shapes;

public static class PathData
{
    public static Geometry AddIcon { get; } = PathGeometry.Parse("M19,13H13V19H11V13H5V11H11V5H13V11H19V13Z");

    public static Geometry DeleteIcon { get; } = PathGeometry.Parse("M6,19A2,2 0 0,0 8,21H16A2,2 0 0,0 18,19V7H6V19M8,9H16V19H8V9M15.5,4L14.5,3H9.5L8.5,4H5V6H19V4H15.5Z");

    public static Geometry RenameIcon { get; } = PathGeometry.Parse("M15 16L11 20H21V16H15M12.06 7.19L3 16.25V20H6.75L15.81 10.94L12.06 7.19M18.71 8.04C19.1 7.65 19.1 7 18.71 6.63L16.37 4.29C16.17 4.09 15.92 4 15.66 4C15.41 4 15.15 4.1 14.96 4.29L13.13 6.12L16.88 9.87L18.71 8.04Z");
}