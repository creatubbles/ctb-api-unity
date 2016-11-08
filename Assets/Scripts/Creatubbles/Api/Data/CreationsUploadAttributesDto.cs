//
//  CreationsUploadAttributesDto.cs
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
    public class CreationsUploadAttributesDto
    {
        public string url;
        public string post_url;
        public string content_type; // TODO - set upload request Content-Type header to this
        public string ping_url; // TODO - ping this URL after upload finished
        public string completed_at; // TODO - DateTime
        public bool aborted;
        // "aborted_with": null, // TODO - what data type goes here?
        public bool processing_completed;
        // "processing_details": null, // TODO - what data type goes here?
        public string created_at; // TODO - DateTime
        public string updated_at; // TODO - DateTime
    }
}

