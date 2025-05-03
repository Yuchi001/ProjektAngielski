namespace MinimapPack
{
    public interface IRenderStrategy
    {
        public bool Render(out MinimapElement minimapElement, string key);
    }
}