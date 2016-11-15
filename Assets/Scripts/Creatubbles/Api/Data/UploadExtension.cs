//
//  UploadExtension.cs
//  Creatubbles API Client Unity SDK
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

namespace Creatubbles.Api.Data
{
    /// <summary>
    /// Enum representing file extensions accepted by Creatubbles API.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#supported-file-formats.
    /// </remarks>
	public enum UploadExtension
	{
		PNG,
		JPG,
		JPEG,
		H264,
		MPEG4,
		WMV,
		WEBM,
		FLV,
		OGG,
		OGV,
		MP4,
		M4V,
		MOV,
		UZPB
	}

    /// <summary>
    /// UploadExtension methods.
    /// </summary>
    public static class UploadExtensionMethods
    {
        /// <summary>
        /// Transforms <paramref name="uploadExtension"/> into a <c>string</c>.
        /// </summary>
        /// <returns>string representing extension for known extension values, otherwise <c>null</c></returns>
        /// <param name="uploadExtension">Upload extension.</param>
        public static string StringValue(this UploadExtension uploadExtension)
        {
            switch (uploadExtension)
            {
                case UploadExtension.PNG:
                    return "png";
                case UploadExtension.JPG:
                    return "jpg";
                case UploadExtension.JPEG:
                    return "jpeg";
                case UploadExtension.H264:
                    return "h264";
                case UploadExtension.MPEG4:
                    return "mpeg4";
                case UploadExtension.WMV:
                    return "wmv";
                case UploadExtension.WEBM:
                    return "webm";
                case UploadExtension.FLV:
                    return "flv";
                case UploadExtension.OGG:
                    return "ogg";
                case UploadExtension.OGV:
                    return "ogv";
                case UploadExtension.MP4:
                    return "mp4";
                case UploadExtension.M4V:
                    return "m4v";
                case UploadExtension.MOV:
                    return "mov";
                case UploadExtension.UZPB:
                    return "uzpb";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Transforms <paramref name="stringValue"/> into <see cref="UploadExtension"/>.
        /// </summary>
        /// <returns>enum for strings representing known extensions, otherwise <c>null</c>.</returns>
        /// <param name="stringValue">String value.</param>
        public static UploadExtension? FromString(string stringValue)
        {
            switch (stringValue)
            {
                case "png": 
                    return UploadExtension.PNG;
                case "jpg": 
                    return UploadExtension.JPG;
                case "jpeg":  
                    return UploadExtension.JPEG;
                case "h264": 
                    return UploadExtension.H264;
                case "mpeg4": 
                    return UploadExtension.MPEG4;
                case "wmv": 
                    return UploadExtension.WMV;
                case "webm":  
                    return UploadExtension.WEBM;
                case "flv": 
                    return UploadExtension.FLV;
                case "ogg": 
                    return UploadExtension.OGG;
                case "ogv": 
                    return UploadExtension.OGV;
                case "mp4": 
                    return UploadExtension.MP4;
                case "m4v": 
                    return UploadExtension.M4V;
                case "mov": 
                    return UploadExtension.MOV;
                case "uzpb": 
                    return UploadExtension.UZPB;

                default: 
                    return null;
            }
        }
    }
}