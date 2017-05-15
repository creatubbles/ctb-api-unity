﻿//  GetCreationRequest.cs
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

namespace Creatubbles.Api.Requests
{
    /// <summary>
    /// Request for fetching a creation from API.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#get-specific-creation.
    /// </remarks>
    public class GetCreationRequest: DataRequest<CreationDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.Requests.GetCreationRequest"/> class.
        /// </summary>
        /// <param name="creationId">ID of creation to fetch. REQUIRED.</param>
        public GetCreationRequest(string creationId): base(new CreationParser(), "data")
        {
            if (string.IsNullOrEmpty(creationId))
            {
                throw new ArgumentNullException("creationId");
            }

            Path = "/creations/" + creationId;
            Method = HttpMethod.GET;
            Authorization = AuthorizationType.Private;
        }
    }
}
