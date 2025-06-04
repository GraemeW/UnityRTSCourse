namespace GameDevTV.RTS.Environment
{
    public interface IGatherable
    {
        public SupplySO supplySO {  get; }
        public int amount { get; }
        public bool isBusy { get; }

        public bool BeginGather();
        public int EndGather();
        public void AbortGather();
    }
}
