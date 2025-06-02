namespace GameDevTV.RTS.Environment
{
    public interface IGatherable
    {
        public SupplySO supply {  get; }
        public int amount { get; }
        public bool isBusy { get; }

        public bool BeginGather();
        public int EndGather();
    }
}
