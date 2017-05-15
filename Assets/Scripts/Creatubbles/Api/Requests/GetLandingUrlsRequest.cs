﻿//  GetLandingUrlsRequest.cs
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
//
using System;
using Creatubbles.Api.Data;
using Creatubbles.Api.Parsers;
using System.Collections.Generic;

namespace Creatubbles.Api.Requests
{
    /// <summary>
    /// Request for retrieving landing URLs.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#list-landing-urls.
    /// </remarks>
    public class GetLandingUrlsRequest: DataRequest<IList<LandingUrlDto>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.Requests.GetLandingUrlsRequest"/> class.
        /// </summary>
        /// <param name="authorizationType">
        ///     <list type="bullet">
        ///         <item><c>Public</c> will return application specific URLs.</item>
        ///         <item><c>Private</c> will return user specific URLs (user must be logged in first).</item>
        ///         <item><c>None</c> will cause request to fail.</item>
        ///     </list>
        /// </param>
        public GetLandingUrlsRequest(AuthorizationType authorizationType): base(new ArrayParser<LandingUrlDto>(new LandingUrlParser()), "data")
        {
            Path = "/landing_urls";
            Method = HttpMethod.GET;
            Authorization = authorizationType;
        }
    }
}

