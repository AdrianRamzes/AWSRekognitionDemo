using Emgu.CV;
using Emgu.CV.Structure;
using AWSRekognitionDemo.Helpers;
using AWSRekognitionDemo.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Amazon.Rekognition.Model;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace AWSRekognitionDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WebCamService _webCamService;
        private RekognitionService _rekognitionService;

        private Bitmap _frame;
        public Bitmap Frame
        {
            get
            {
                return _frame;
            }

            set
            {
                if (_frame != value)
                {
                    _frame = value;
                    RaisePropertyChanged(() => Frame);
                }
            }
        }

        private Bitmap _photo;
        public Bitmap Photo
        {
            get
            {
                return _photo;
            }

            set
            {
                if (_photo != value)
                {
                    _photo = value;
                    RaisePropertyChanged(() => Photo);
                    RaisePropertyChanged(() => IsRecognitionAvailable);
                }
            }
        }

        public ObservableCollection<RecognitionItem> RecognitionItems { get; set; }

        public bool IsRecognitionAvailable
        {
            get { return Photo != null; }
        }

        private ICommand _toggleWebServiceCommand;
        public ICommand ToggleWebCamServiceCommand
        {
            get
            {
                return _toggleWebServiceCommand;
            }

            private set { }
        }

        private ICommand _recognizeCommand;
        public ICommand RecognizeCommand
        {
            get
            {
                return _recognizeCommand;
            }
            private set { }
        }

        public MainWindowViewModel()
        {
            RecognitionItems = new ObservableCollection<RecognitionItem>();

            InitializeServices();
            InitializeCommands();
            ToggleWebServiceExecute();
        }

        private void InitializeServices()
        {
            _webCamService = new WebCamService();
            _webCamService.ImageChanged += _webCamService_ImageChanged;

            _rekognitionService = new RekognitionService();
        }

        private void InitializeCommands()
        {
            _toggleWebServiceCommand = new DelegateCommand(ToggleWebServiceExecute);
            _recognizeCommand = new DelegateCommand(RecognizeExecute);
        }

        private void _webCamService_ImageChanged(object sender, Image<Bgr, Byte> image)
        {
            var frame = image;
            frame.Draw(new Rectangle(0, 0, frame.Width - 1, frame.Height - 1), new Bgr(Color.Red), 10);
            Frame = frame.Bitmap;
        }

        private void ToggleWebServiceExecute()
        {
            if (!_webCamService.IsRunning)
            {
                _webCamService.RunServiceAsync();
                Photo = null;
            }
            else
            {// take a picture
                _webCamService.CancelServiceAsync();
                var photo = _webCamService.GetCurrentFrame();
                Photo = photo.Bitmap;
            }
        }

        private void RecognizeExecute()
        {
            if (Photo != null)
            {
                var result = _rekognitionService.FakeRecognize(Photo);

                if (result.FaceDetails.Count > 0)
                {
                    var face = result.FaceDetails[0];

                    RecognitionItems.Clear();
                    var items = GetRecognitionItems(face);
                    items.ForEach(i => RecognitionItems.Add(i));

                    RecognitionItems.Add(new RecognitionItem()
                    {
                        Name = "Age Range",
                        Confidence = 100,
                        Value = $"{face.AgeRange.Low}-{face.AgeRange.High}",
                    });

                    foreach (var emotion in face.Emotions)
                    {
                        RecognitionItems.Add(new RecognitionItem()
                        {
                            Confidence = emotion.Confidence,
                            Name = emotion.Type.Value,
                            Value = ((int)emotion.Confidence).ToString(),
                        });
                    }

                    using (var gr = Graphics.FromImage(Photo))
                    {
                        gr.DrawRectangle(new Pen(Color.Red, 3), face.BoundingBox.Left * Photo.Width, face.BoundingBox.Top * Photo.Height, face.BoundingBox.Width * Photo.Width, face.BoundingBox.Height * Photo.Height);

                        foreach (var mark in face.Landmarks)
                        {
                            gr.DrawEllipse(new Pen(Color.Aqua, 3), mark.X * Photo.Width, mark.Y * Photo.Height, 1, 1);
                        }
                        gr.Save();
                    }
                    RaisePropertyChanged(() => Photo);
                }

            }
        }

        private List<RecognitionItem> GetRecognitionItems(FaceDetail face)
        {
            var result = new List<RecognitionItem>()
            {
                new RecognitionItem()
                {
                    Name = "Beard",
                    Value = face.Beard.Value.ToString(),
                    Confidence = face.Beard.Confidence
                },
                new RecognitionItem()
                {
                    Name = "Eyeglasses",
                    Value = face.Eyeglasses.Value.ToString(),
                    Confidence = face.Eyeglasses.Confidence
                },
                new RecognitionItem()
                {
                    Name = "EyesOpen",
                    Value = face.EyesOpen.Value.ToString(),
                    Confidence = face.EyesOpen.Confidence
                },
                new RecognitionItem()
                {
                    Name = "MouthOpen",
                    Value = face.MouthOpen.Value.ToString(),
                    Confidence = face.MouthOpen.Confidence
                },
                new RecognitionItem()
                {
                    Name = "Mustache",
                    Value = face.Mustache.Value.ToString(),
                    Confidence = face.Mustache.Confidence
                },
                new RecognitionItem()
                {
                    Name = "Smile",
                    Value = face.Smile.Value.ToString(),
                    Confidence = face.Smile.Confidence
                },
                new RecognitionItem()
                {
                    Name = "Sunglasses",
                    Value = face.Sunglasses.Value.ToString(),
                    Confidence = face.Sunglasses.Confidence
                }
            };

            return result;
        }
    }

    public class RecognitionItem
    {
        public string Name { get; set; }

        public float Confidence { get; set; }

        public string Value { get; set; }
    }
}
