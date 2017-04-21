//
//  UserDto.cs
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
    /// User data transfer object.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#user-details.
    /// </remarks>
    public class UserDto
    {
        public string type;
        public string id;
        // attributes
        public string username;
        public string display_name;
        public string list_name;
        public string name;
        public string role;
        public string age;
        public string gender;
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
        public string last_bubbled_at;
        public string last_commented_at;
        public bool signed_up_as_instructor;
        public string what_do_you_teach;
        public string interests;
        public bool home_schooling;
        public string created_at;
        public string updated_at;

        public override string ToString()
        {
            return "type: " + type
                + "\nid: " + id
                + "\nusername: " + username
                + "\ndisplay name: " + display_name
                + "\nlist name: " + list_name
                + "\nname: " + name
                + "\nrole: " + role
                + "\nage: " + age
                + "\ngender: " + gender
                + "\ncountry code: " + country_code
                + "\ncountry name: " + country_name
                + "\navatar url" + avatar_url
                + "\nshort url" + short_url
                + "\nadded bubbles count: " + added_bubbles_count
                + "\nactivities count: " + activities_count
                + "\nbubbles count: " + bubbles_count
                + "\ncomments count: " + comments_count
                + "\ncreations count: " + creations_count
                + "\ncreators count: " + creators_count
                + "\ngalleries count: " + galleries_count
                + "\nmanagers count: " + managers_count
                + "\nlast bubbled at: " + last_bubbled_at
                + "\nlast commented at: " + last_commented_at
                + "\nsigned up as instructor: " + signed_up_as_instructor
                + "\nwhat do you teach: " + what_do_you_teach
                + "\ninterests: " + interests
                + "\nhome schooling: " + home_schooling
                + "\ncreated at: " + created_at
                + "\nupdated at: " + updated_at;
        }
    }
}
