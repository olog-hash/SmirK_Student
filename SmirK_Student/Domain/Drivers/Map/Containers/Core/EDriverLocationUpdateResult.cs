namespace SmirK_Student.Domain.Drivers.Map.Containers.Core
{

    public enum EDriverLocationUpdateResult
    {
        InvalidPosition,
        OccupiedPosition,
        InvalidDriver,
        Added,
        Updated,
        Removed,
    }
}