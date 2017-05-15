﻿//
//  NewCreationRequest.cs
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Creatubbles.Api.Data;
using Creatubbles.Api.Parsers;

namespace Creatubbles.Api.Requests
{
    /// <summary>
    /// Request for initializing a new creation on the API.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#create-creation.
    /// </remarks>
    public class NewCreationRequest: DataRequest<CreationDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.Requests.NewCreationRequest"/> class.
        /// </summary>
        /// <param name="creationData">Data of the creation to initialize. REQUIRED.</param>
        public NewCreationRequest(NewCreationData creationData): base(new CreationParser(), "data")
        {
            if (creationData == null)
            {
                throw new NullReferenceException("creationData");
            }

            Path = "/creations";
            Method = HttpMethod.POST;
            Authorization = AuthorizationType.Private;

            var creatorIds = creationData.creatorIds != null ? String.Join(",", creationData.creatorIds) : null;

            AddFieldIfNotNull("name", creationData.name);
            AddFieldIfNotNull("creator_ids", creatorIds);
            AddFieldIfNotNull("created_at_month", creationData.creationMonth.ToString());
            AddFieldIfNotNull("created_at_year", creationData.creationYear.ToString());
            AddFieldIfNotNull("reflection_text", creationData.reflectionText);
            AddFieldIfNotNull("reflection_video_url", creationData.reflectionVideoUrl);
        }
    }
}

