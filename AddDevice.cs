using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace Vnext.Function
{
    public class AddDevice
    {
        private readonly IDeviceAsset _deviceAsset;
        public AddDevice(IDeviceAsset deviceAsset)
        {
            _deviceAsset = deviceAsset;

        }

        [FunctionName("AddDevice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] BodyRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IEnumerable<ResponseBody> assetIds = await _deviceAsset.GetAssetIdsForDevices(req.devices);
            IEnumerable<Devices> assignDevicesToAssetIds = _deviceAsset.AssignAssetIdToDevices(req.devices, assetIds);
            IEnumerable<Devices> recordsAddedToDatabase = _deviceAsset.AddToDatabase(assignDevicesToAssetIds);
            return new OkObjectResult(recordsAddedToDatabase);

        }
    }
}
