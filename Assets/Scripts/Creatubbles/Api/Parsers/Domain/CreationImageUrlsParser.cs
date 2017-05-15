//
//  CreationImageUrlsParser.cs
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
    public class CreationImageUrlsParser: BaseJsonParser<CreationImageUrlsDto>
    {
        public CreationImageUrlsParser(CreationImageUrlsDto fallbackValue = null)
        {
            this.fallbackValue = fallbackValue;
        }

        override protected bool CanParseValue(JSONNode json)
        {
            return json.IsObject;
        }

        override protected ParsingResult<CreationImageUrlsDto> ParseValue(JSONNode json)
        {
            var errors = new List<ParsingError>();
            var imageUrls = new CreationImageUrlsDto();
            var linksJson = json["links"];

            linksJson.ParseAndAssignString("original", o => imageUrls.original = o, errors);
            linksJson.ParseAndAssignString("full_view", fv => imageUrls.fullView = fv, errors);
            linksJson.ParseAndAssignString("list_view_retina", lvr => imageUrls.listViewRetina = lvr, errors);
            linksJson.ParseAndAssignString("list_view", lv => imageUrls.listViewRetina = lv, errors);
            linksJson.ParseAndAssignString("matrix_view_retina", mvr => imageUrls.matrixViewRetina = mvr, errors);
            linksJson.ParseAndAssignString("matrix_view", mv => imageUrls.matrixView = mv, errors);
            linksJson.ParseAndAssignString("gallery_mobile", gm => imageUrls.galleryMobile = gm, errors);
            linksJson.ParseAndAssignString("explore_mobile", em => imageUrls.exploreMobile = em, errors);
            linksJson.ParseAndAssignString("share", sh => imageUrls.share = sh, errors);

            if (errors.Any())
            {
                return new ParsingResult<CreationImageUrlsDto>(errors);
            }

            return new ParsingResult<CreationImageUrlsDto>(imageUrls);
        }
    }
}

