//  CreationUploadSetupRequest.cs
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
using Creatubbles.Api.Data;
using Creatubbles.Api.Parsers;
using Creatubbles.Api.Requests;

namespace Creatubbles.Api.Requests
{
    /// <summary>
    /// Request to retrieve information from the API, required to upload a creation file.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#create-creation-upload.
    /// </remarks>
    public class CreationUploadSetupRequest: DataRequest<CreationUploadConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.Requests.CreationUploadSetupRequest"/> class.
        /// </summary>
        /// <param name="creationId">ID of creation for which to retrieve upload information.</param>
        /// <param name="extension">Extension of file to be uploaded.</param>
        public CreationUploadSetupRequest(string creationId, UploadExtension uploadExtension)
            : base(new CreationUploadConfigurationParser(), "data")
        {
            Path = "/creations/" + creationId + "/uploads";
            Method = HttpMethod.POST;
            Authorization = AuthorizationType.Private;

            AddField("extension", uploadExtension.StringValue());
        }
    }
}

