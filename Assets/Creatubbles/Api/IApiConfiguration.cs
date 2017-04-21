//
// IApiConfiguration.cs
// Creatubbles API Client Unity SDK
//
// Copyright (c) 2017 Creatubbles Pte. Ltd.
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
//
using System;

namespace Creatubbles.Api
{
    /// <summary>
    /// Configuration for Creatubbles API.
    /// </summary>
    public interface IApiConfiguration
    {
        /// <summary>
        /// Base API URL.
        /// </summary>
        /// <value>Should be set to https://api.staging.creatubbles.com in <c>Debug</c> builds and to https://api.creatubbles.com in <c>Release</c> builds.</value>
        string BaseUrl { get; }

        /// <summary>
        /// Personal application identifier.
        /// </summary>
        /// <remarks>
        /// Please contact support@creatubbles.com to obtain it.
        /// </remarks>
        string AppId { get; }

        /// <summary>
        /// Personal application secret.
        /// </summary>
        /// <remarks>
        /// Please contact support@creatubbles.com to obtain it.
        /// </remarks>
        string AppSecret { get; }

        /// <summary>
        /// API version.
        /// </summary>
        /// <value>Should contain a valid version string like "v1" or "v2".</value>
        string ApiVersion { get; }

        /// <summary>
        /// Locale code used for getting localized responses from the server.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#locales.
        /// </remarks>
        /// <value>Should contain values like “en”, “pl”, “de”. Can be <c>null</c>.</value>
        string Locale { get; }
    }
}
