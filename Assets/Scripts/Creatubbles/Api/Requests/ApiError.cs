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

namespace Creatubbles.Api
{
    /// <summary>
    /// Class representing an error returned by Creatubbles API.
    /// <list type="bullet">
    ///     <listheader><term>More info at:</term></listheader>
    ///     <item><see href="https://partners.creatubbles.com/api/#errors">https://partners.creatubbles.com/api/#errors</see>/</item>
    ///     <item><see href="http://jsonapi.org/format/#error-objects">http://jsonapi.org/format/#error-objects</see>/</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ApiError
    {
        // TODO - localize
        private const string ErrorTitleAuthenticationFailed = "Authentication failed";
        private const string ErrorDetailsNoDetails = "No more details are available at the moment. Please try again.";
        private const string ErrorDetailsCreationUploadCancelled = "Your creation upload was cancelled. Please re-upload again, or add new creation.";

        // Defaults
        private const int DefaultStatus = -6000;
        private const string DefaultDomain = "com.creatubbles.apiclient.errordomain";
        private const string DefaultCode = "creatubbles-apiclient-default-code";
        private const string DefaultTitle = "creatubbles-apiclient-default-title";
        private const string DefaultSource = "creatubbles-apiclient-default-source";
        private const string DefaultDetail = "creatubbles-apiclient-default-detail";
        private const string DefaultAuthenticationCode = "authentication-error";

        // Error codes
        public const int UnknownStatus = -6001;
        public const int LoginStatus = -6002;
        public const int UploadCancelledStatus = -6003;

        public int status;
        public string code;
        /// <summary>
        /// Human readable title. Respects request's Accept-Language header.
        /// </summary>
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

        public static ApiError GenericLoginError()
        {
            return GenericError(
                status: LoginStatus,
                code: DefaultCode,
                title: ErrorTitleAuthenticationFailed,
                source: "https://www.creatubbles.com/api/v2/users",
                detail: ErrorDetailsNoDetails
            );
        }

        public static ApiError GenericUploadCancelledError()
        {
            return GenericError(
                status: UploadCancelledStatus,
                code: "upload-cancelled",
                title: "Upload cancelled",
                source: DefaultSource,
                detail: ErrorDetailsCreationUploadCancelled
            );
        }

        public static ApiError GenericError(int? status, string code, string title, string source, string detail, string domain = DefaultDomain)
        {
            return new ApiError(
                status: status ?? DefaultStatus,
                code: code ?? DefaultCode,
                title: title ?? DefaultTitle,
                source: source ?? DefaultSource,
                detail: detail ?? DefaultDetail,
                domain: domain
            );
        }
    }
}
