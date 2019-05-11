namespace IOCLite
{
    /// <summary>
    /// To be implemented by any module classes that are to be installed on an IOCLiteContainer.
    /// </summary>
    public interface IIOCLiteModule
    {
        void RegisterComponents(IOCLiteContainer container);
    }
}
