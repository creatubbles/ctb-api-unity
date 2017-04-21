//
//  CreationUploadConfiguration.cs
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
using UnityEngine;


namespace Creatubbles.Api.Data
{
    public class CreationUploadConfiguration
    {
        public string content_type;
        public string ping_url;
        public string post_url;
        public Dictionary<string, string> post_credentials;

        public override string ToString()
        {
            if (post_credentials == null)
            {
                Debug.LogError("CREDENTIALS ARE NULL!!! But " + ping_url + ", " + post_url + ", " + content_type);
                return null;
            }

            return "content_type: " + content_type + "\nping_url: " + ping_url + "\npost_url: " + post_url + post_credentials.Select(item => "\n" + item.Key + ": " + item.Value);
        }
    }
}

