namespace CSRogue.Map_Generation
{
    public interface IMap
    {
        MapLocationData this[int iCol, int iRow] { get; set; }
        MapLocationData this[MapCoordinates loc] { get; set; }
        int Height { get; }
        int Width { get; }
    }
}
