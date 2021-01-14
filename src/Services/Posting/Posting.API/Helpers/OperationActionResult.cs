﻿using Microsoft.AspNetCore.Mvc;
using Posting.Core.Interfaces.Asp;

namespace Posting.API.Helpers
{
    public class OperationActionResult<T> : ObjectResult
    {
        private readonly IOperationResult<T> _value;

        public OperationActionResult(IOperationResult<T> value) : base(value)
        {
            _value = value;
        }

        public override void OnFormatting(ActionContext context)
        {
            if (_value.IsSuccess)
            {
                context.HttpContext.Response.StatusCode = (int) _value.StatusCode;
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int) _value.Error.StatusCode;
            }
        }
    }
}