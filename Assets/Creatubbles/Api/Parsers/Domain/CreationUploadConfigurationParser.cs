﻿//
//  CreationUploadConfigurationParser.cs
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
    public class CreationUploadConfigurationParser: BaseJsonParser<CreationUploadConfiguration>
    {
        public CreationUploadConfigurationParser(CreationUploadConfiguration fallbackValue = null)
        {
            this.fallbackValue = fallbackValue;
        }

        override protected bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        override protected ParsingResult<CreationUploadConfiguration> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();
            var configuration = new CreationUploadConfiguration();
            var attributesJson = json["attributes"];

            attributesJson.ParseAndAssignString("content_type", ct => configuration.content_type = ct, errors, isRequired: true);
            attributesJson.ParseAndAssignString("ping_url", url => configuration.ping_url = url, errors, isRequired: true);
            attributesJson.ParseAndAssignString("post_url", url => configuration.post_url = url, errors, isRequired: true);
            attributesJson.ParseAndAssignDictionary("post_credentials", c => configuration.post_credentials = c, errors, isRequired: true);

            if (errors.Any())
            {
                return new ParsingResult<CreationUploadConfiguration>(errors);
            }

            return new ParsingResult<CreationUploadConfiguration>(configuration);
        }
    }
}

