//
//  BaseJsonParser.cs
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
    public delegate void AssignerFunc<T>(T value);

    public abstract class BaseJsonParser<T>: IJsonParser<T>
    {
        private T FallbackValue { get { return fallbackValue; } }
        protected T fallbackValue;

        protected abstract bool CanParseValue(JSONNode json);

        protected abstract ParsingResult<T> ParseValue(JSONNode json);

        public ParsingResult<T> ParseString(string json, string key, bool isRequired = false)
        {
            return ParseNode(JSON.Parse(json), key, isRequired);
        }

        public ParsingResult<T> ParseNode(JSONNode json, string key, bool isRequired = false)
        {
            var jsonValue = string.IsNullOrEmpty(key) ? json : json[key];
            var jsonValueIsBlank = jsonValue == null || jsonValue.IsNull;

            if (jsonValueIsBlank)
            {
                if (isRequired)
                {
                    return new ParsingResult<T>(new ParsingError("Found null for required data '" + key + "' when parsing '" + typeof(T).Name + "'"));
                }
                else
                {
                    return new ParsingResult<T>(FallbackValue);
                }
            }

            if (!jsonValueIsBlank && !CanParseValue(jsonValue))
            {
                return new ParsingResult<T>(new ParsingError("Expected value '" + jsonValue + "' for key '" + key + "' to be a '" + typeof(T).Name));
            }

            return ParseValue(jsonValue);
        }
    }
}
