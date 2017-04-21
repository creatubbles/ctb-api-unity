//
//  UserParser.cs
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
using System.Collections.Generic;
using SimpleJSON;
using Creatubbles.Api.Data;

namespace Creatubbles.Api.Parsers
{
    public class UserParser: BaseJsonParser<UserDto>
    {
        public UserParser(UserDto fallbackValue = null)
        {
            this.fallbackValue = fallbackValue;
        }

        protected override bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        protected override ParsingResult<UserDto> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();
            var attributesJson = json["attributes"];

            var user = new UserDto();
            json.ParseAndAssignString("type", t => user.type = t, errors);
            json.ParseAndAssignString("id", id => user.id = id, errors);
            attributesJson.ParseAndAssignString("username", un => user.username = un, errors);
            attributesJson.ParseAndAssignString("display_name", dn => user.display_name = dn, errors);
            attributesJson.ParseAndAssignString("list_name", ln => user.list_name = ln, errors);
            attributesJson.ParseAndAssignString("name", n => user.name = n, errors);
            attributesJson.ParseAndAssignString("role", r => user.role = r, errors);
            attributesJson.ParseAndAssignString("age", a => user.age = a, errors);
            attributesJson.ParseAndAssignString("gender", g => user.gender = g, errors);
            attributesJson.ParseAndAssignString("country_code", cc => user.country_code = cc, errors);
            attributesJson.ParseAndAssignString("country_name", cn => user.country_name = cn, errors);
            attributesJson.ParseAndAssignString("avatar_url", au => user.avatar_url = au, errors);
            attributesJson.ParseAndAssignString("short_url", su => user.short_url = su, errors);
            attributesJson.ParseAndAssignInt("added_bubbles_count", c => user.added_bubbles_count = c, errors);
            attributesJson.ParseAndAssignInt("activities_count", c => user.activities_count = c, errors);
            attributesJson.ParseAndAssignInt("bubbles_count", c => user.activities_count = c, errors);
            attributesJson.ParseAndAssignInt("creations_count", c => user.creations_count = c, errors);
            attributesJson.ParseAndAssignInt("comments_count", c => user.comments_count = c, errors);
            attributesJson.ParseAndAssignInt("creators_count", c => user.creators_count = c, errors);
            attributesJson.ParseAndAssignInt("galleries_count", c => user.galleries_count = c, errors);
            attributesJson.ParseAndAssignInt("managers_count", c => user.managers_count = c, errors);
            attributesJson.ParseAndAssignString("last_bubbled_at", lba => user.last_bubbled_at = lba, errors);
            attributesJson.ParseAndAssignString("last_commented_at", lca => user.last_commented_at = lca, errors);
            attributesJson.ParseAndAssignBool("signed_up_as_instructor", suai => user.signed_up_as_instructor = suai, errors);
            attributesJson.ParseAndAssignString("what_do_you_teach", wdyt => user.what_do_you_teach = wdyt, errors);
            attributesJson.ParseAndAssignString("interests", i => user.interests = i, errors);
            attributesJson.ParseAndAssignBool("home_schooling", hs => user.home_schooling = hs, errors);

            if (errors.Any())
            {
                return new ParsingResult<UserDto>(errors);
            }

            return new ParsingResult<UserDto>(user);
        }
    }
}