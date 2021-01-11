using System;

namespace Posting.Core.Errors
{
    public class InnerErrorBuilder
    {
        private readonly string _code;
        private Func<InnerErrorBuilder> _innerBuilder;

        public InnerErrorBuilder(string code)
        {
            _code = code;
        }

        public InnerErrorData Build()
        {
            return new InnerErrorData
            {
                Code = _code,
                InnerError = _innerBuilder().Build()
            };
        }

        public InnerErrorBuilder WithInner(Func<InnerErrorBuilder> innerBuilderFunc)
        {
            _innerBuilder = innerBuilderFunc;
            return this;
        }
    }
}