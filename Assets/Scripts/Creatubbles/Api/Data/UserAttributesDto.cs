//
//  UserAttributesDto.cs
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
    // TODO - DateTime needs to be parsed separately
    [Serializable]
    public class UserAttributesDto
    {
        public string username;
        public string display_name;
        public string list_name;
        public string name;
        public string role; // TODO - enum
        public int age;
        public string gender; // TODO - enum
        public string country_code;
        public string country_name;
        public string avatar_url;
        public string short_url;
        public int added_bubbles_count;
        public int activities_count;
        public int bubbles_count;
        public int comments_count;
        public int creations_count;
        public int creators_count;
        public int galleries_count;
        public int managers_count;
        public DateTime last_bubbled_at;
        public DateTime last_commented_at;
        public bool signed_up_as_instructor;
        public string what_do_you_teach;
        public string interests;
        public bool home_schooling;
        public DateTime created_at;
        public DateTime updated_at;
    }
}
