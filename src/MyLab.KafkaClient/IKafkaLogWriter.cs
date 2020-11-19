namespace MyLab.KafkaClient
{
    /// <summary>
    /// Writes log into string output
    /// </summary>
    public interface IKafkaLogWriter
    {
        /// <summary>
        /// Writes log line
        /// </summary>
        void WriteLine(string str);
    }
}