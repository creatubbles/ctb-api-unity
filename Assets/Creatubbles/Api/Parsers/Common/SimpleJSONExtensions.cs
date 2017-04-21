//
//  SimpleJSONExtensions.cs
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
    public static class SimpleJSONExtensions
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>Returns a dictionary representing given JSONObject</returns>
        public static Dictionary<string, string> AsDictionary(this JSONObject json)
        {
            var result = new Dictionary<string, string>();

            foreach (var keyValueObject in json)
            {
                var keyValuePair = (KeyValuePair<string, JSONNode>)keyValueObject;
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return result;
        }

        public static void ParseAndAssign<T>(this JSONNode json, string key, AssignerFunc<T> resultAssigner, List<ParsingError> errors, IJsonParser<T> parser, bool isRequired = false)
        {
            var parsingResult = parser.ParseNode(json, key, isRequired);

            if (parsingResult.IsError)
            {
                errors.AddRange(parsingResult.errors);
                return;
            }

            resultAssigner(parsingResult.result);
        }

        public static void ParseAndAssignInt(this JSONNode json, string key, AssignerFunc<int> resultAssigner, List<ParsingError> errors, int fallbackValue = 0, bool isRequired = false)
        {
            ParseAndAssign<int>(json, key, resultAssigner, errors, new IntParser(fallbackValue), isRequired);
        }

        public static void ParseAndAssignString(this JSONNode json, string key, AssignerFunc<string> resultAssigner, List<ParsingError> errors, string fallbackValue = "", bool isRequired = false)
        {
            ParseAndAssign<string>(json, key, resultAssigner, errors, new StringParser(fallbackValue), isRequired);
        }

        public static void ParseAndAssignBool(this JSONNode json, string key, AssignerFunc<bool> resultAssigner, List<ParsingError> errors, bool fallbackValue = false, bool isRequired = false)
        {
            ParseAndAssign<bool>(json, key, resultAssigner, errors, new BoolParser(fallbackValue), isRequired);
        }

        public static void ParseAndAssignDictionary(this JSONNode json, string key, AssignerFunc<Dictionary<string, string>> resultAssigner, List<ParsingError> errors, Dictionary<string, string> fallbackValue = null, bool isRequired = false)
        {
            ParseAndAssign<Dictionary<string, string>>(json, key, resultAssigner, errors, new DictionaryParser(fallbackValue), isRequired);
        }

        public static void ParseAndAssignObject<T>(this JSONNode json, string key, AssignerFunc<T> resultAssigner, List<ParsingError> errors, IJsonParser<T> parser, T fallbackValue = null, bool isRequired = false) where T: class
        {
            ParseAndAssign<T>(json, key, resultAssigner, errors, parser, isRequired);
        }
    }
}
