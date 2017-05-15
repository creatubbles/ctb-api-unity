//
//  CreationParser.cs
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
    public class CreationParser: BaseJsonParser<CreationDto>
    {
        public CreationParser(CreationDto fallbackValue = null)
        {
            this.fallbackValue = fallbackValue;
        }

        override protected bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        override protected ParsingResult<CreationDto> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();
            var creation = new CreationDto();
            var attributesJson = json["attributes"];

            json.ParseAndAssignString("id", id => creation.id = id, errors);
            json.ParseAndAssignString("type", t => creation.type = t, errors);
            attributesJson.ParseAndAssignString("name", n => creation.name = n, errors);
            attributesJson.ParseAndAssignBool("approved", a => creation.approved = a, errors);
            attributesJson.ParseAndAssignString("approval_status", a => creation.approval_status = a, errors);
            attributesJson.ParseAndAssignString("created_at_age", ca => creation.created_at = ca, errors);
            attributesJson.ParseAndAssignObject("image", i => creation.images = i, errors, new CreationImageUrlsParser());
            attributesJson.ParseAndAssignInt("image_status", i => creation.image_status = i, errors);
            attributesJson.ParseAndAssignInt("bubbles_count", bc => creation.bubbles_count = bc, errors);
            attributesJson.ParseAndAssignInt("comments_count", cc => creation.comments_count = cc, errors);
            attributesJson.ParseAndAssignInt("views_count", vc => creation.views_count = vc, errors);
            attributesJson.ParseAndAssignString("last_bubbled_at", lba => creation.last_bubbled_at = lba, errors);
            attributesJson.ParseAndAssignString("last_commented_at", lca => creation.last_commented_at = lca, errors);
            attributesJson.ParseAndAssignString("last_submitted_at", lsa => creation.last_submitted_at = lsa, errors);
            attributesJson.ParseAndAssignString("short_url", surl => creation.short_url = surl, errors);
            attributesJson.ParseAndAssignString("created_at", ca => creation.created_at = ca, errors);

            if (errors.Any())
            {
                return new ParsingResult<CreationDto>(errors);
            }

            return new ParsingResult<CreationDto>(creation);
        }
    }
}
