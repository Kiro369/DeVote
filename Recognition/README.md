# Python environment
Setup environment for authentication process, personality verification, reading ID card info.

# SetUp
Make sure you have python interpreter version 3.7.9 Or 3.7.11 installed.
You can download windows interpreter from [here](https://www.python.org/ftp/python/3.7.9/python-3.7.9-amd64.exe)

1. Install requirements
```bash
  pip install -r requirements.txt
```
in windows you maybe face a problem during installation because of dlib toolkit.
you can download and install it separately from [here](http://dlib.net/)

2. Set environment paths for the wrapper engine.
in [appsettings](https://github.com/Kiro369/DeVote/blob/master/DeVote/appsettings.json) file change these variables.
```bash
  PythonDLLPath: "path/to/python/interpreter/dll",
  SitePackagesPath: "path/to/site packages",
```

3. set cameras or scanners.
for id ocr we use [ip-webcam](https://play.google.com/store/apps/details?id=com.pas.webcam&hl=ar&gl=US) as a scanner.
use any droidcam, ip-cam or scanner you prefere.
Make sure you have a reference to that cam or scanner and change these variables in [IDForm](https://github.com/Kiro369/DeVote/blob/master/DeVotingApp/IDForm.cs) file
```bash
  readonly OpenCvSharp.VideoCapture _capture = new("the/reference/to/cam/or/scanner");
  .
  .
  .
  readonly OpenCvSharp.VideoCapture _capture = new("the/reference/to/cam/or/scanner");
```

and for face recognition we use laptop webcam you can use any camera you prefere
change this variable in [FaceRecognition](https://github.com/Kiro369/DeVote/blob/master/DeVotingApp/FaceRecognition.cs) file
use "0" as a parameter for using laptop webcam
```bash
  readonly OpenCvSharp.VideoCapture _capture = new("reference/to/video/stream/camera");
```
