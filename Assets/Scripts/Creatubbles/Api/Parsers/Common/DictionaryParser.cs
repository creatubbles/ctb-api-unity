//
//  DictionaryParser.cs
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
using Creatubbles.Api.Data;

namespace Creatubbles.Api.Parsers
{
    public class DictionaryParser: BaseJsonParser<Dictionary<string, string>>
    {
        public DictionaryParser(Dictionary<string, string> fallbackValue)
        {
            this.fallbackValue = fallbackValue;
        }

        protected override bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        protected override ParsingResult<Dictionary<string, string>> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();

            if (!json.IsObject)
            {
                errors.Add(new ParsingError("Expected to be a dictionary but found: " + "'" + json + "'"));
                return new ParsingResult<Dictionary<string, string>>(errors);
            }

            var result = json.AsObject.AsDictionary();

            if (result == null)
            {
                errors.Add(new ParsingError("Failed to parse following JSON as dictionary: " + "'" + json + "'"));
                return new ParsingResult<Dictionary<string, string>>(errors);
            }

            return new ParsingResult<Dictionary<string, string>>(result);
        }
    }
}
