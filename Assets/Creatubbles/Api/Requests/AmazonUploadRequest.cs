//  AmazonUploadRequest.cs
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
using System.Linq;
using Creatubbles.Api.Data;

namespace Creatubbles.Api.Requests
{
    public class AmazonUploadRequest: Request
    {
        public AmazonUploadRequest(byte[] data, CreationUploadConfiguration configuration)
        {
            AbsoluteUrl = configuration.post_url;
            Method = HttpMethod.POST;
            Authorization = AuthorizationType.None;
            IgnoreHeaders = true;

            var requiredFilename = "creationFile";

            AddField("name", requiredFilename);
            foreach (var credential in configuration.post_credentials)
            {
                AddField(credential.Key, credential.Value);
            }
            AddField("Content-Type", configuration.content_type);
            AddBinaryData("file", data, requiredFilename, configuration.content_type);
        }

        public override void HandleResponse()
        {
            base.HandleResponse();

            // NOTE: a workaround for an issue with WWW returning an invalid status of S3 upload request (e.g. 200 where actual is 403)
            //       resulting in a failed request being treated as a successful one
            if (TextResponse.Contains("<Error>"))
            {
                Errors.Add(new RequestError(TextResponse, ErrorType.Http));
                UnityEngine.Debug.Log("Reporting Amazon errors: " + Errors.Count);
            }
        }
    }
}

