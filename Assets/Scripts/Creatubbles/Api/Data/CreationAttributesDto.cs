//
//  CreationAttributesDto.cs
//  CreatubblesApiClient
//
//  Copyright (c) 2016 Creatubbles Pte. Ltd.
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

namespace Creatubbles.Api
{
    [Serializable]
    public class CreationAttributesDto
    {
        public string name;
        public bool approved;
        public string approval_status; // TODO - enum - Possible values: “approved”, “unapproved”, “rejected”
        public string created_at_age;
        // JsonUtility doesn't support dictionaries yet, need to use different deserializer to support below
//        public created_at_age_per_creator;
//        {
//            "oCRxzp9F": "at 14y",
//            "vff379a2": "at 9y"
//        },
        public string image; // TODO - expand to object - see iOS
        public int image_status; // TODO - enum - 1: “empty”; 2: “processing”; 3: “ready”
        public int bubbles_count;
        public int comments_count;
        public int views_count;
        public string last_bubbled_at; // TODO - DateTime
        public string last_commented_at; // TODO - DateTime
        public string last_submitted_at; // TODO - DateTime
        public string short_url;
        public string created_at; // TODO - DateTime
        // TODO - for more attributes see https://stateoftheart.creatubbles.com/api/#creation-details
    }
}

