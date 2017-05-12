//
//  OAuthResponseParser.cs
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
//
using System;
using System.Linq;
using System.Collections.Generic;
using SimpleJSON;
using Creatubbles.Api.Data;

namespace Creatubbles.Api.Parsers
{
    public class OAuthResponseParser: BaseJsonParser<OAuthResponse>
    {
        public OAuthResponseParser(OAuthResponse fallbackValue = null)
        {
            this.fallbackValue = fallbackValue;
        }

        protected override bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        protected override ParsingResult<OAuthResponse> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();

            var response = new OAuthResponse();
            json.ParseAndAssignString("access_token", t => response.token = t, errors, fallbackValue: null, isRequired: true);
            json.ParseAndAssignString("token_type", id => response.tokenType = id, errors, fallbackValue: null, isRequired: true);

            if (errors.Any())
            {
                return new ParsingResult<OAuthResponse>(errors);
            }

            return new ParsingResult<OAuthResponse>(response);
        }
    }
}

