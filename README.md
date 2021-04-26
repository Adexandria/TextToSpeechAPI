# TextToSpeechAPI
##### A REST API that converts a text image to a mp3 file. The text image can either be in Jpeg(image/jpeg) or png(image/png).
## Application
- ##### Azure Computer Vison
- ##### Azure Translate Speech
- ##### Azure Storage Account(Blob storage)
- ##### Swagger

## Packages
- ##### Microsoft.AspNetCore
- ##### Newtonsoft.Json
- ##### MimeMapping
- ##### Swashbackle.Aspnetcore
- ##### Azure.Storage.Blobs

## Documentation
##### The REST API consists of one function.The function is used to upload an image for processing. check out the [demo website](https://adetext.azurewebsites.net/index.html)
##
#### How it works
- ##### A text image is uploaded using the post method, it's uploaded into the blob storage then downloaded into byte data. It's sent to the computer vision to interpret and the computer receives the data for processing.
- ##### The processed data is sent as a ssml file to the azure translate speech to convert the data to an mp3 file.

###### Thank You.