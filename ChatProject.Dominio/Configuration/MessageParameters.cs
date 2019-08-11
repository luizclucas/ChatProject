namespace ChatProject.Domain.Configuration
{
    public static class MessageParameters
    {
        public static string MessageEnd => "*END*";
        public static int BufferSize { get; set; } = 1024;
    }
}
