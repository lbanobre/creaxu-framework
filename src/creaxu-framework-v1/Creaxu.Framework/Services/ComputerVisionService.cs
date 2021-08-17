using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Creaxu.Framework.Services
{
    public interface IComputerVisionService
    {
        Task<string> ExtractTextAsync(Stream image, int left = 0, int top = 0, int width = 0, int height = 0);
    }

    public class ComputerVisionService : IComputerVisionService
    {
        private readonly ComputerVisionClient _computerVision;
        private readonly IConfiguration _configuration;

        public ComputerVisionService(IConfiguration configuration)
        {
            _configuration = configuration;

            _computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_configuration["ComputerVision:SubscriptionKey"]), new System.Net.Http.DelegatingHandler[] { });
            _computerVision.Endpoint = _configuration["ComputerVision:Endpoint"];
        }

        public async Task<string> ExtractTextAsync(Stream image, int left = 0, int top = 0, int width = int.MaxValue, int height = int.MaxValue)
        {
            //var textHeaders = await _computerVision.BatchReadFileInStreamAsync(image);

            //var operationId = textHeaders.OperationLocation.Substring(textHeaders.OperationLocation.Length - 36);

            //var result = await _computerVision.GetReadOperationResultAsync(operationId);

            //int i = 0;
            //int maxRetries = 10;
            //while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            //{
            //    await Task.Delay(1000);
            //    result = await _computerVision.GetReadOperationResultAsync(operationId);
            //}

            //var sb = new StringBuilder();

            //var recResults = result.RecognitionResults;
            //foreach (var recResult in recResults)
            //{
            //    foreach (var line in recResult.Lines)
            //    {
            //        if (line.BoundingBox[0] > left && line.BoundingBox[1] > top && line.BoundingBox[2] < (left + width) && line.BoundingBox[3] < (top + height))
            //        {
            //            sb.Append(line.Text);
            //            sb.Append(" ");
            //        }
            //    }
            //}

            //return sb.ToString().Trim();

            return "";
        }
    }
}
