using Amazon.Rekognition.Model;
using Amazon.Rekognition;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AWSRekognitionDemo.Services
{
    public class RekognitionService
    {
        AmazonRekognitionClient _client;

        public RekognitionService()
        {
            _client = new AmazonRekognitionClient();
        }

        public DetectFacesResponse FakeRecognize(Bitmap image)
        {
            var rekognitionResultPath = "Resources/result.json";

            var jsonString = File.ReadAllText(rekognitionResultPath);

            var deserialized = JsonConvert.DeserializeObject<DetectFacesResponse>(jsonString);

            return deserialized;
        }

        public DetectFacesResponse Recognize(Bitmap image)
        {
            MemoryStream memoryStream = new MemoryStream();

            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            var result = _client.DetectFaces(new DetectFacesRequest()
            {
                Attributes = new List<string> { "ALL" },
                Image = new Amazon.Rekognition.Model.Image() { Bytes = memoryStream }
            });

            var serialized = JsonConvert.SerializeObject(result);

            File.WriteAllText("real_result.json", serialized);

            return result;
        }
    }
}
