//using Microsoft.AspNetCore.Hosting.Server;
//using System.Text.Json;

//namespace Bb.Extensions
//{

//    public static class HttpResponseExtensions
//    {

//        /// <summary>
//        /// Pushes a response to the http response.
//        /// </summary>
//        /// <param name="self">HttpResponse</param>
//        /// <param name="httpCode">http code to return</param>
//        /// <param name="body">body write</param>
//        /// <returns></returns>
//        public static async Task WriteResponse(this Task<HttpResponse> self, int httpCode, string body)
//        {

//            var response = await self.ConfigureAwait(false);

//            if (response != null)
//            {
//                response.StatusCode = httpCode;
//                var bytes = System.Text.Encoding.UTF8.GetBytes(body);
//                await response.BodyWriter.WriteAsync(bytes);
//            }

//            await Task.Yield();

//        }

//        /// <summary>
//        /// Pushes a JSON response to the http response.
//        /// </summary>
//        /// <param name="self">HttpResponse</param>
//        /// <param name="httpCode">http code to return</param>
//        /// <param name="body">body to serialize</param>
//        /// <param name="indented">the serialization is indented</param>
//        /// <returns></returns>
//        public static async Task WriteResponse(this Task<HttpResponse> self, int httpCode, object body, bool indented = false)
//        {
//            await self.WriteResponse(httpCode, body, new JsonSerializerOptions() { WriteIndented = indented });
//        }

//        /// <summary>
//        /// Pushes a JSON response to the http response
//        /// </summary>
//        /// <param name="self">HttpResponse</param>
//        /// <param name="httpCode">http code to return</param>
//        /// <param name="body">body to serialize</param>
//        /// <param name="options">serialization options</param>
//        /// <returns></returns>
//        public static async Task WriteResponse(this Task<HttpResponse> self, int httpCode, object body, JsonSerializerOptions? options = default)
//        {

//            var response = await self.ConfigureAwait(false);

//            if (response != null)
//            {

//                if (options == null)
//                    options = new JsonSerializerOptions();

//                string jsonString = JsonSerializer.Serialize(body, options);

//                response.StatusCode = httpCode;
//                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
//                await response.BodyWriter.WriteAsync(bytes);
//            }

//            await Task.Yield();

//        }


//    }
//}
