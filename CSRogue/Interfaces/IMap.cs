using CSRogue.Map_Generation;

namespace CSRogue.Interfaces
{
    public interface IMap
    {
        IMapLocationData this[int iCol, int iRow] { get; set; }
        IMapLocationData this[MapCoordinates loc] { get; set; }
        int Height { get; set; }
        int Width { get; set; }
    }
}
