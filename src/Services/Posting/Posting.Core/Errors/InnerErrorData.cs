namespace Posting.Core.Errors
{
    public class InnerErrorData
    {
        public string Code { get; set; }
        public InnerErrorData InnerError { get; set; }
    }
}