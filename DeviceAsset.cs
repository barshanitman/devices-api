using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Vnext.Function
{

    public interface IDeviceAsset
    {
        public Task<IEnumerable<ResponseBody>> GetAssetIdsForDevices(IEnumerable<Devices> devices);

        public IEnumerable<Devices> AssignAssetIdToDevices(IEnumerable<Devices> listOfDevices, IEnumerable<ResponseBody> assetids);

        public IEnumerable<Devices> AddToDatabase(IEnumerable<Devices> devices);

        public Task<string> MakeRequest(string url, SemaphoreSlim semaphore, HttpClient httpClient);


    }

    public class DeviceAsset : IDeviceAsset
    {
        private readonly HttpClient _httpClient;

        private readonly IGenericRepo<Devices> _devices;

        public DeviceAsset(HttpClient httpClient, IGenericRepo<Devices> devices)
        {
            _httpClient = httpClient;
            _devices = devices;

        }


        public IEnumerable<Devices> AddToDatabase(IEnumerable<Devices> devices)
        {

            _devices.AddMultiple(devices);
            _devices.SaveChanges();

            return devices;

        }

        public IEnumerable<Devices> AssignAssetIdToDevices(IEnumerable<Devices> listOfDevices, IEnumerable<ResponseBody> assetids)
        {
            foreach (Devices device in listOfDevices)
            {

                device.AssetId = assetids.Where(item => item.deviceId == ("DVID" + device.Id)).First().assetId;


            }

            return listOfDevices;

        }

        public async Task<IEnumerable<ResponseBody>> GetAssetIdsForDevices(IEnumerable<Devices> devices)
        {


            _httpClient.DefaultRequestHeaders.Add("x-functions-key", Environment.GetEnvironmentVariable("X_KEY"));
            // var response = await _httpClient.GetAsync($"http://tech-assessment.vnext.com.au/api/devices/assetId/{deviceId}");
            List<string> urls = new List<string>();
            foreach (Devices device in devices)
            {

                urls.Add(Environment.GetEnvironmentVariable("API_URL") + "DVID" + device.Id);

            }

            List<string> finalResults = new List<string>();

            int numberOfRequests = urls.Count;
            int maxParallelRequests = 100;
            var semaphoreSlim = new SemaphoreSlim(maxParallelRequests, maxParallelRequests);

            var tasks = new List<Task<string>>();

            for (int i = 0; i < numberOfRequests; ++i)
            {
                tasks.Add(MakeRequest(urls[i], semaphoreSlim, _httpClient));
            }

            var responses = await Task.WhenAll(tasks);


            var finalResponses = responses.Select(res =>

            JsonConvert.DeserializeObject<ResponseBody>(res, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
             );
            return finalResponses;

        }


        public async Task<string> MakeRequest(string url, SemaphoreSlim semaphore, HttpClient httpClient)
        {
            try
            {
                await semaphore.WaitAsync();
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                var response = await httpClient.SendAsync(request);

                // Add an optional delay for further throttling:
                await Task.Delay(TimeSpan.FromMilliseconds(3000));

                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }


    }


}