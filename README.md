# Banner app

.NET Core web service and utils around it

## How to run
0. Download and install .net core sdk
1. install & run mongodb (go to install dir, run mongod, remember port)
2. Configure mongodb connection string: check appsetting.json file (Db->connectionString)
3. Configure web service url in hosting.json
4. Run web service in console 'dotnet BannerWebApp.dll'


## REST Samples (e.g. for postman):
Consider server url is 'localhost:5000;
1. POST 
URL: http://localhost:5000/api/banners

BODY:

```json
{
    "id": 5,
    "html": "<div id=\"content\">\r\n    <h1>Hello world<\/h1>\r\n    <i>Hi everybody<\/i>\r\n<\/div>\r\n<button class=\"download\">Download<\/button>\r\n\r\n\r\n<script src=\"http:\/\/code.jquery.com\/jquery-1.11.2.min.js\"><\/script>\r\n<script>\r\n    $('.download').on('click', function(){\r\n       $('<a \/>').attr({\r\n              download: 'export.html', \r\n              href: \"data:text\/html,\" + $('#content').html() \r\n       })[0].click()\r\n    });\r\n<\/script>",
    "created": "2018-07-17T11:35:06.8959631+02:00",
    "modified": "2018-07-17T11:35:06.8959631+02:00"
}
```

2. PUT
URL: http://localhost:5000/api/banners/5

``` json
BODY:
{
    "html": "<div>updated</div>",
    "created": "2018-07-17T11:35:06.8959631+02:00",
    "modified": "2018-07-17T11:35:06.8959631+02:00"
}
```

3. DELETE
URL: http://localhost:5000/api/banners/5

4. GET ALL
URL: http://localhost:5000/api/banners?skip=0&take=2

6. GET RENDERING
URL: http://localhost:5000/api/banners/render/2


## Loading testing
run dotnet LoadTesting.dll Run <postThreadsCount> <getThreadsCount> <putThreadsCount> <operationsPerSecondForEachThread> for simple load testing