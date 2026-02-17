namespace GameDevTV.RTS.UI
{
    public interface IUIElement<T>
    {
        void EnableFor(T setBaseBuilding);
        void Disable();
    }

    public interface IUIElement<T1, T2>
    {
        void EnableFor(T1 item, T2 callback);
        void Disable();
    }
}
