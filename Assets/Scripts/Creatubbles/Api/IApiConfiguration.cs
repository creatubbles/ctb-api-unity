//
// ICreatubblesApiClientConfiguration.cs
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
        /// API base URL.
        /// <list type="bullet">
        ///     <listheader>
        ///         <term>Should contain:</term>
        ///     </listheader>
        ///     <item>
        ///         <description><see href="https://api.staging.creatubbles.com">https://api.staging.creatubbles.com</see> for <c>Debug</c> builds</description>
        ///     </item>
        ///     <item>
        ///         <description><see href="https://api.creatubbles.com">https://api.creatubbles.com</see> for <c>Release</c> builds</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <value>The base API URL.</value>
        string BaseUrl { get; }

        /// <summary>
        /// Personal application identifier. Please contact support@creatubbles.com to obtain it.
        /// </summary>
        /// <value>The app identifier.</value>
        string AppId { get; }

        /// <summary>
        /// Personal application secret. Please contact support@creatubbles.com to obtain it.
        /// </summary>
        /// <value>The app secret.</value>
        string AppSecret { get; }

        /// <summary>
        /// API version string. For example "v2".
        /// </summary>
        /// <value>The API version.</value>
        string ApiVersion { get; }

        /// <summary>
        /// Locale code used for getting localized responses from servers. Example values: “en”, “pl”, “de”. Can be null.
        /// More info at <see href="https://stateoftheart.creatubbles.com/api/#locales">https://stateoftheart.creatubbles.com/api/#locales</see>.
        /// </summary>
        /// <value>The locale.</value>
        string Locale { get; }
    }
}
