namespace Test_Cs.Errors
{
    public class NotFoundException : Exception
    {
        public int StatusCode { get; }

        public NotFoundException(string msg)
            : base(msg)
        {
            StatusCode = 404;
        }
    }
}
