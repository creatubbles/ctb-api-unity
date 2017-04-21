﻿//
//  CreationDto.cs
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

namespace Creatubbles.Api.Data
{
    /// <summary>
    /// Creation data transfer object.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#creation-details.
    /// </remarks>
    public class CreationDto
    {
        public string id;
        public string type;
        // attributes
        public string name;
        public bool approved;
        public string approval_status;
        public string created_at_age;
        public string image;
        public int image_status;
        public int bubbles_count;
        public int comments_count;
        public int views_count;
        public string last_bubbled_at;
        public string last_commented_at;
        public string last_submitted_at;
        public string short_url;
        public string created_at;

        public override string ToString()
        {
            return "type: " + type
                + "\nid: " + id
                + "\nname: " + name
                + "\napproved: " + approved
                + "\nlistapproval_status: " + approval_status
                + "\ncreated_at_age: " + created_at_age
                + "\nimage: " + image
                + "\nimage_status: " + image_status
                + "\nbubbles_count: " + bubbles_count
                + "\ncomments_count: " + comments_count
                + "\nviews_count: " + views_count
                + "\nlast_bubbled_at" + last_bubbled_at
                + "\nlast_commented_at" + last_commented_at
                + "\nlast_submitted_at: " + last_submitted_at
                + "\nshort_url: " + short_url
                + "\ncreated_at: " + created_at;
        }
    }
}
