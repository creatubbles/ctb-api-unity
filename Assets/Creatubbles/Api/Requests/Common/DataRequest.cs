//
//  DataRequest.cs
//  Creatubbles API Client Unity SDK
//
//  Copyright (c) 2017 Creatubbles Pte. Ltd.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Creatubbles.Api;
using Creatubbles.Api.Parsers;

namespace Creatubbles.Api.Requests
{
    public class DataRequest<T>: Request
    {
        public T ParsedResponse { get; private set; }

        private IJsonParser<T> responseParser;
        private string responseParserKey;

        public DataRequest(IJsonParser<T> responseParser, string responseParserKey)
        {
            this.responseParser = responseParser;
            this.responseParserKey = responseParserKey;
        }

        override public void HandleResponse()
        {
            base.HandleResponse();

            if (IsError)
            {
                return;
            }

            if (responseParser == null)
            {
                Errors.Add(new RequestError("No parser defined for DataRequest<" + typeof(T).Name + ">.", ErrorType.Parsing));
                return;
            }

            var parseResult = responseParser.ParseString(TextResponse, responseParserKey, true);

            if (parseResult.IsError)
            {
                Errors.AddRange(parseResult.errors.Select(e => new RequestError(e.message, ErrorType.Parsing)));
                return;
            }

            ParsedResponse = parseResult.result;
        }
    }
}
