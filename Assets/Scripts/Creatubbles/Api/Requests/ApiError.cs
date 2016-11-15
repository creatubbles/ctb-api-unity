//
// ApiError.cs
// CreatubblesApiClient
//
// Copyright (c) 2016 Creatubbles Pte. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

/// <summary>
/// Contains classes and interfaces representing API requests and responses.
/// </summary>
namespace Creatubbles.Api.Requests
{
    /// <summary>
    /// Class representing an error returned by Creatubbles API.
    /// </summary>
    /// <remarks>
    /// More info at https://partners.creatubbles.com/api/#errors and http://jsonapi.org/format/#error-objects.
    /// </remarks>
    [Serializable]
    public class ApiError
    {
        // Defaults
        private const string DefaultDomain = "com.creatubbles.apiclient.errordomain";
        private const string DefaultCode = "creatubbles-apiclient-default-code";
        private const string DefaultTitle = "creatubbles-apiclient-default-title";
        private const string DefaultSource = "creatubbles-apiclient-default-source";
        private const string DefaultDetail = "creatubbles-apiclient-default-detail";
        private const string DefaultAuthenticationCode = "authentication-error";

        // Error codes
        public const int DefaultStatus = -6000;
        public const int UnknownStatus = -6001;
        public const int LoginStatus = -6002;
        public const int UploadCancelledStatus = -6003;

        public int status;
        public string code;

        /// <summary>
        /// Human readable title.
        /// </summary>
        /// <remarks>
        /// Respects request's <c>Accept-Language</c> header.
        /// </remarks>
        public string title;

        public string source;
        public string detail;
        public string domain;

        public ApiError(int status, string code, string title, string source, string detail, string domain = DefaultDomain)
        {
            this.status = status;
            this.code = code;
            this.title = title;
            this.source = source;
            this.detail = detail;
            this.domain = domain;
        }
    }
}
