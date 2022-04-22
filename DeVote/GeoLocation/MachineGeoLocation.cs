using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Device.Location;
using System.Net.Http;

/** Must turn on
   - Allow access to location on the device.
   - Allow apps to access location.
**/

namespace DeVote.VMachineGeoLocation
{
    public class MachineGeoLocation
    {
        public GeoCoordinateWatcher Watcher;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Title = Environment.MachineName;
        public string ID = Constants.MachineID;

        // Start acquiring location data.
        public void StartGeoWatcher()
        {
            Watcher = new GeoCoordinateWatcher();
            Watcher.Start();
        }

        public GeoPositionPermission GetPermissionStatus()
        {
            GeoPositionPermission Permission = GeoPositionPermission.Unknown;
            switch (Watcher.Permission)
            {
                case GeoPositionPermission.Unknown:
                    // Console.WriteLine("Location permission is unknown");
                    Permission = GeoPositionPermission.Unknown;
                    break;

                case GeoPositionPermission.Denied:
                    // Console.WriteLine("Location permission is denied");
                    Permission = GeoPositionPermission.Denied;
                    break;

                case GeoPositionPermission.Granted:
                    // Console.WriteLine("Location permission is granted");
                    Permission = GeoPositionPermission.Granted;
                    break;
            }
            return Permission;
        }

        public void TryGetLocation()
        {
            //  When watcher has started, the location data does not become available instantly.
            //  The geoCoordinate's IsUnknown property can be checked to determine if location data is available. 
            GeoCoordinate geoCoordinate = Watcher.Position.Location;

            // keep trying to acquire location data as long as geoCoordinate contains no data.
            while (geoCoordinate.IsUnknown == true)
            {
                // keep tracking the permission's status to access location data.
                GeoPositionPermission Permission = this.GetPermissionStatus();

                // if permission is denied, set both latitude and longitude values as 0.
                if (Permission == GeoPositionPermission.Denied)
                {
                    Console.WriteLine("Location permission is denied");
                    this.Latitude = 0;
                    this.Longitude = 0;
                    return;
                };
                // try to acquire location data only when permission is granted.
                if (Permission == GeoPositionPermission.Granted)
                {
                    geoCoordinate = Watcher.Position.Location;
                    this.Latitude = geoCoordinate.Latitude;
                    this.Longitude = geoCoordinate.Longitude;
                }
            }
            // latitude and longitude data is available.
            if (geoCoordinate.IsUnknown != true)
            {
                Console.WriteLine($"Lat: {this.Latitude}, Long: {this.Longitude}");
                return;
            }
        }

        public Task SendLocation(bool isTest = false)
        {
            string endpoint = "http://localhost:3000/vms";

            var values = new Dictionary<string, string> { };
            values["id"] = this.ID;
            values["name"] = this.Title;
            values["lat"] = this.Latitude.ToString();
            values["lng"] = this.Longitude.ToString();

            // for the sake of testing and avoiding endpoint error.
            if (isTest)
            {
                Random rnd = new Random();
                values["id"] += rnd.Next(1, 100);
                values["name"] += rnd.Next(1, 100);
                endpoint = "https://devote-explorer-backend.herokuapp.com/vms";
            }

            using (var client = new HttpClient())
            {
                var requestBody = new StringContent(JsonConvert.SerializeObject(values).ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(endpoint, requestBody).Result;
                if (response.IsSuccessStatusCode) Console.WriteLine("Machine's location sent and added successfully");

                else
                {
                    Console.WriteLine("Sending machine's location failed");
                    Console.WriteLine(response.StatusCode.ToString());
                    //string responseString = response.Content.ReadAsStringAsync().Result;
                    //JObject responseObj = JObject.Parse(responseString);
                    //Console.WriteLine(responseObj.SelectToken("errors[0].detail"));
                }
            }

            return Task.CompletedTask;
        }

        public void StopGeoWatcher()
        {
            Watcher.Stop();
        }
    }
}