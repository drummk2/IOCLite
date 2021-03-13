namespace IOCLite.Enums
{
    /// <summary>
    /// Represents an implementation's lifespan.
    /// </summary>
    public enum IOCLiteLifeSpan
    {
        /// <summary>
        /// Specifies that a single instance of the registration should be created for
        /// the duration of the application's lifespan. 
        /// </summary>
        SINGLETON,
        
        /// <summary>
        /// Specifies that a new instance should be created everyting a Resolve() call
        /// is made to the IOCLite container.
        /// </summary>
        TRANSIENT
    }
}